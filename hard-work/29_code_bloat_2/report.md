# Important condition

```cs
if (f2fDto.UserId != null)
{
    var userProjects = _dbContext.UserProjects.Where(u => u.UserId == f2fDto.UserId!)
        .Include(u => u.Project)
        .Include(u => u.User)
        .Where(u => u.Project!.ProjectEnd == null || u.Project!.ProjectEnd.Value.Date >= f2fDto.CutOffDate.Date)
        .ToList();

    foreach (var userProject in userProjects)
    {
        int count = 0;
        int countf2f = 0;
        int countAbsence = 0;
        foreach (var participant in participants.Where(p => p.Project!.Id == userProject.ProjectId))
        {
            // The real gatekeeper of the whole evaluation: GetParticipantStateAtDate reconstructs the
            // participant's state at the cutoff and only participants whose computed state is in the requested set even enter the counters below.
            if (f2fDto.ParticipantStates!.Any(p => p == ParticipantStateExtension.GetParticipantStateAtDate(participant, f2fDto.CutOffDate)))
            {
                count++;
                if (WasF2F(f2fDto, participant))
                    countf2f++;

                if (WasParticipantAbsent(f2fDto, participant))
                    countAbsence++;
            }
        }

        result.Add(new F2FEvaluationDto()
        {
            ...
        });
    }
}
```

# Small but important computation

```cs
public async Task<List<AmsApiSkillInfo>> Handle(GetRelevantParticipantSkillsForAms request, CancellationToken cancellationToken)
{
    var participantSkillInfo = await dbContext.ParticipantSkills.AsNoTracking()
        ...
        .ToListAsync(cancellationToken: cancellationToken);

    var participantSkillIds = participantSkillInfo.Select(y => y.Id).ToList();

    var participantSubSkillInfo = await dbContext.ParticipantSubSkills.AsNoTracking()
        .Where(...)
        .ToListAsync(cancellationToken: cancellationToken);

    var joined = participantSkillInfo
        .GroupJoin(participantSubSkillInfo, skill => skill.Id, subSkill => subSkill.ParticipantSkillId,
            (skill, subSkills) => (Skill: skill, SubSkills: subSkills.ToList()))
        .ToList();

    var certifications = joined
        .Where(x => x.Skill.MainCategory == AmsSkillCategory.CertificatesAndQualifications)
        ...

    var professionalSkills = joined.Where(x => x.Skill.MainCategory == AmsSkillCategory.ProfessionalSkills).ToList();

    // Pins the participant's overall rating to the highest present. Single VeryAdvanced beats
    // 10 Advanced, null if neither are present. This single value drives how the participant is graded.
    var targetRating = professionalSkills.Any(x => x.Skill.Reason == ParticipantSkillAssignmentReason.Current
                                                && x.Skill.Rating == ParticipantSkillRating.VeryAdvanced
                                                || x.SubSkills.Any(s => s.Rating == ParticipantSkillRating.VeryAdvanced))
    ? ParticipantSkillRating.VeryAdvanced
    : professionalSkills.Any(x => x.Skill.Reason == ParticipantSkillAssignmentReason.Current
                               && x.Skill.Rating == ParticipantSkillRating.Advanced ||
                               x.SubSkills.Any(s => s.Rating == ParticipantSkillRating.Advanced))
            ? ParticipantSkillRating.Advanced
            : (ParticipantSkillRating?)null;

    var filteredProfessionalSkills = targetRating is null
        ? []
        : professionalSkills
            .Where(x => x.Skill.Reason == ParticipantSkillAssignmentReason.Current && x.Skill.Rating == targetRating || x.SubSkills.Any(s => s.Rating == targetRating))
            .SelectMany(x =>
            {
                var subSkills = x.SubSkills.Where(s => s.Rating == targetRating).Select(s => new AmsApiSkillInfo(s.Tnr));
                return subSkills.Concat(x.Skill.Reason == ParticipantSkillAssignmentReason.Current && x.Skill.Rating == targetRating
                        ? [new AmsApiSkillInfo(x.Skill.Tnr)]
                        : []);
            }).ToList();

    return [..certifications, ..filteredProfessionalSkills];
}
```

# Validations

```cs
public List<ValidationMessageDto> ValidateCasesGeneral(Participant oldParticipant, Participant newParticipant)
{
    var messages = new List<ValidationMessageDto>();

    if (newParticipant.ParticipantCases.Any(p =>
            p != newParticipant.ParticipantCases[0] && p.ExitDate == null && p.RefrainDate == null))
    {
        messages.Add(new ValidationMessageDto(Message: "Vergangene Beratungsverläufe müssen abgeschlossen bleiben",
            Field: ""));
    }

    // Integrity rule: a project phase cannot be attached to more than one participant case.
    List<Guid?> projectPhases = newParticipant.ParticipantCases
        .Where(p => p.ParticipantProjectPhaseId != null)
        .Select(p => p.ParticipantProjectPhaseId)
        .ToList();
    if (projectPhases.Count != projectPhases.Distinct().Count())
    {
        messages.Add(new ValidationMessageDto(
            Message: "Eine Projektphase kann nicht mit mehreren Beratungsverläufen verknüpft sein", Field: ""));
    }

    for (var i = 0; i < newParticipant.ParticipantCases.Count; i++)
    {
        var oldCase = oldParticipant.ParticipantCases[i];
        var newCase = newParticipant.ParticipantCases[i];

        // The whole evaluation system stands on this invariant: a participant's cases must form one clean
        // non-overlapping timeline, so "which case is active at date X" always has exactly one answer
        // (GetRelevantCaseAtDate and ParticipantState rely on it).
        (var futureBoundary, var pastBoundary) =
            ParticipantCaseExtension.GetCaseDateBoundaries(newParticipant.ParticipantCases, newCase);
        bool areCasesOverlapping = !((newCase.RefrainDate == null ||
                                      ((pastBoundary == null ||
                                        newCase.RefrainDate.Value.Date >= pastBoundary.Value.Date) &&
                                       (futureBoundary == null || newCase.RefrainDate.Value.Date <= futureBoundary))) &&
                                     (pastBoundary == null || newCase.EntryDate == null ||
                                      newCase.EntryDate.Value.Date >= pastBoundary.Value.Date) &&
                                     (futureBoundary == null || newCase.ExitDate == null ||
                                      newCase.ExitDate.Value.Date <= futureBoundary.Value.Date));

        string caseName = CaseName(oldCase);

        if (areCasesOverlapping)
        {
            messages.Add(new ValidationMessageDto(
                Message:
                $"{caseName}: Zugebucht-, Eintritt-, Nicht-Eintritt- und Austrittsdaten dürfen nicht mit anderen Beratungsverläufen überlappen",
                Field: ""));
        }

        messages.AddRange(ValidateSingleCase(oldCase, newCase));
    }

    if (newParticipant.ParticipantCases.Any(p => p.ExitDate != null) && newParticipant.AdvisorId == null)
    {
        messages.Add(new ValidationMessageDto(Message: "Berater*in muss gesetzt sein, wenn ein Fall abgeschlossen wird",
            Field: ""));
    }

    return messages;
}
```

# Weird at first glance

```cs
private protected async Task DoCheckTimeAvailability(TEntity entity, Guid projectIdFromEntity, DbOperationMode mode, bool throwExceptionsOnlyForOverlaps)
{
    try
    {
        await CheckTimeAvailability(entity, projectIdFromEntity, mode);
    }
    catch (TimeRecordingInvalidException ex)
    {
        if (!throwExceptionsOnlyForOverlaps)
            throw;

        // Policy boundary between "reject" and "proceed": a real schedule overlap has to
        // block the operation, but warnings must not. But warning also throw, that's why we rethrow again...
        if (ex.Overlaps.Any(x => x.OverlappingTimeRecordingState == OverlappingTimeRecordingState.Overlap))
            throw new TimeRecordingInvalidException(ex.Overlaps.Where(x =>
                x.OverlappingTimeRecordingState == OverlappingTimeRecordingState.Overlap).ToList());
    }
}
```

# Silent important condition

```cs
foreach (var bookingWithActivity in bookingsWithActivity)
{
    try
    {
        await timeRecordingService.CreateWithTimeCheck(dbContext, bookingWithActivity.Activity, true);
        
        // If the advisor created the booking for himself, then we do not need to set the participant activity, because this is an internal - should not be recorded
        // And we also do not need time slot booking for that
        if (bookingWithActivity.Booking.Booking.AdvisorId == bookingWithActivity.Booking.Booking.CreatedUserId)
            dbContext.TimeSlotBookings.Remove(bookingWithActivity.Booking.Booking);
        else
            bookingWithActivity.Booking.Booking.ParticipantActivityId = bookingWithActivity.Activity.Id;
            
        dbContext.TimeRecordings.Remove(bookingWithActivity.Booking.TimeRecording);
    }
    catch (TimeRecordingInvalidException)
    {
        logger.LogWarning("Couldn't reconcile timeSlotBooking at {Date:yyyy-MM-dd} of participant {Participant} to participantActivity because of overlaps", bookingWithActivity.Booking.Booking.StartDateTime, participant.Key);
    }
}
```
