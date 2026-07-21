# 1.

I firstly could not grasp the idea presented in the lesson, but after I starting looking through the code, actually (if I got the idea correctly), then I was already partially following the approach
with the output driven design. Current ticket - import of participants and for existing participants there might be different cases when data is updated, fully different and current case is closed etc.
And I deciede to model this using DU so that I focus all my mapping/functions whatever towards this data structure:

```cs
public abstract record ParticipantForImport
{
    private protected ParticipantForImport() { }

    /// <summary>Not found anywhere in the system → create fresh.</summary>
    public sealed record New : ParticipantForImport;

    /// <summary>In the selected project, only identical data in the case → nothing to do.</summary>
    public sealed record NoChanges : ParticipantForImport;

    /// <summary>In the selected project, at least one equal field, the rest is different → update current case. </summary>
    public sealed record UpdatedCurrentCase(
        Guid ParticipantId,
        ValueChange<DateTime?> BookedDate,
        ValueChange<DateTime?> EntryDate,
        ValueChange<DateTime?> DecisionOfEntranceUntil,
        ValueChange<DateTime?> ExitDate) : ParticipantForImport;

    /// <summary>In the selected project, all data is different and current case is closed → new project case / new participant case.</summary>
    public sealed record CurrentCaseEnded(Guid ParticipantId) : ParticipantForImport;

    /// <summary>In the selected project, all data is different and current case is still open → close the current case / update current case.</summary>
    public sealed record CurrentCaseNotEndedButDataChanged(
        Guid ParticipantId,
        ValueChange<DateTime?> BookedDate,
        ValueChange<DateTime?> EntryDate,
        ValueChange<DateTime?> DecisionOfEntranceUntil,
        ValueChange<DateTime?> ExitDate) : ParticipantForImport;

    /// <summary>In another project where the user may update participant → reentry from another project / create new</summary>
    public sealed record ExternalProjectWithUpdateRights(Guid ParticipantId, string ProjectName) : ParticipantForImport;

    /// <summary>In another project where the user may not edit participants or user is not part of the project at all → create new</summary>
    public sealed record ExternalProjectCreateOnly(Guid ParticipantId, string ProjectName) : ParticipantForImport;
}
```

And somehow the proposed corecursive approach followed naturally from the good(I hope so :) data model

```cs
public static ParticipantForImport ResolveFromCurrentCase(Guid participantId, ParticipantCase existing, AmsParticipantImportModel toImport)
{
    ValueChange<DateTime?>[] changes =
    [
        ValueChange<DateTime?>.From(existing.BookedDate, toImport.BookedDate),
        ValueChange<DateTime?>.From(existing.EntryDate, toImport.EntryDate?.Date),
        ValueChange<DateTime?>.From(existing.DecisionOfEntranceUntil, toImport.DecisionOfEntranceOrInitialInterviewDate?.Date),
        ValueChange<DateTime?>.From(existing.ExitDate, toImport.Austritt?.Date)
    ];

    if (changes.All(c => c is ValueChange<DateTime?>.UnchangedNull or ValueChange<DateTime?>.UnchangedValue))
        return new ParticipantForImport.NoChanges();

    // At least one date matches on both sides, so the import is anchored to this case → the rest are updates onto it.
    if (changes.Any(c => c is ValueChange<DateTime?>.UnchangedValue))
        return new ParticipantForImport.UpdatedCurrentCase(participantId, BookedDate: changes[0], EntryDate: changes[1], DecisionOfEntranceUntil: changes[2], ExitDate: changes[3]);

    // At this point all data is different
    return (existing.ExitDate is not null) switch
    {
        true => new ParticipantForImport.CurrentCaseEnded(participantId),
        false => new ParticipantForImport.CurrentCaseNotEndedButDataChanged(participantId, BookedDate: changes[0], EntryDate: changes[1], DecisionOfEntranceUntil: changes[2], ExitDate: changes[3])
    };
}
```

Where I literally find cases for the output forms - when can the `NoChanges` happen or in which cases is the `UpdateParticipantCase`. I do not care that much here about the input data. I am not sure that this is still the masterpiece, but the thing is, that the output data form has some sort of a form here, not just nullable everything multiplied with nullable everything. Because this is what I thought when going through colleagues' code. Their data shape is just bad. And following a bad data shape in the output will not bring that much. I think this thinking technique is really powerful, but only if we care about the form of our output data. So the actual example:

```cs
public static (DateTimeOffset? futureBoundary, DateTimeOffset? pastBoundary) GetCaseDateBoundaries(List<ParticipantCase> participantCases, ParticipantCase currentCase)
{
    var sorted = participantCases.OrderByDescending(p => p.CaseCreatedAt).ToList();;
    var index = sorted.FindIndex(p => p.Id == currentCase.Id);

    // if index is not found or there is only the current case
    if (index == -1 || sorted.Count == 1)
        return (null, null);

    var previousCase = index > 0 ? sorted[index - 1] : null;
    var nextCase = index + 1 < sorted.Count ? sorted[index + 1] : null;

    var futureBoundary = GetEarliestRelevantDate(previousCase)?.Date;
    var pastBoundary = (nextCase?.ExitDate ?? nextCase?.RefrainDate)?.Date;

    return (futureBoundary, pastBoundary);
}

private static DateTimeOffset? GetEarliestRelevantDate(ParticipantCase? participantCase)
{
    if (participantCase == null) return null;
    return participantCase.EntryDate
           ?? participantCase.BookedDate
           ?? participantCase.InitialInterviewDate
           ?? participantCase.RefrainDate
           ?? participantCase.CaseCreatedAt;
}
```

I would propose not lying about the output shape of the data, make it more precise, write some sort of producers for each field. There are two fields in the returned `CaseDateBoundaries` - the `Future` and the `Past` where each of them can have three cases:

```cs
public abstract record CaseBoundary
{
    private protected CaseBoundary() { }

    /// No neighboring case on this side → truly unbounded.
    public sealed record Unbounded : CaseBoundary;

    /// A neighboring case pins the boundary to this date.
    public sealed record At(DateTimeOffset Date) : CaseBoundary;

    // A neighboring case exists, but carries no usable date.
    public sealed record NeighborWithoutDate(Guid NeighborCaseId) : CaseBoundary;
}

public sealed record CaseDateBoundaries(CaseBoundary Future, CaseBoundary Past);
```

And after we have a well shaped output, we can refactor the function:

```cs
public static Optional<CaseDateBoundaries> GetCaseDateBoundaries(IReadOnlyCollection<ParticipantCase> participantCases, ParticipantCase currentCase)
{
    if (participantCases.All(participantCase => participantCase.Id != currentCase.Id))
        return Optional<CaseBoundaries>.None;

    var otherCases = participantCases.Where(participantCase => participantCase.Id != currentCase.Id);

    return new CaseDateBoundaries(
        Future: NearestLaterCase(otherCases, currentCase).Pipe(GetFutureBoundary),
        Past: NearestEarlierCase(otherCases, currentCase).Pipe(GetPastBoundary));
}

private static ParticipantCase? NearestLaterCase(IEnumerable<ParticipantCase> cases, ParticipantCase current) =>
    cases.Where(candidate => candidate.CaseCreatedAt > current.CaseCreatedAt)
         .MinBy(candidate => candidate.CaseCreatedAt);

private static ParticipantCase? NearestEarlierCase(IEnumerable<ParticipantCase> cases, ParticipantCase current) =>
    cases.Where(candidate => candidate.CaseCreatedAt < current.CaseCreatedAt)
         .MaxBy(candidate => candidate.CaseCreatedAt);

private static CaseBoundary GetFutureBoundary(ParticipantCase? laterCase) =>
    laterCase switch
    {
        null => new CaseBoundary.Unbounded(),
        _    => new CaseBoundary.At(GetEarliestRelevantDate(laterCase).Date)
    };

private static CaseBoundary GetPastBoundary(ParticipantCase? earlierCase) =>
    (earlierCase, (earlierCase?.ExitDate ?? earlierCase?.RefrainDate)) switch
    {
        (null, _)            => new CaseBoundary.Unbounded(),
        (_, { } endDate)     => new CaseBoundary.At(endDate.Date),
        ({ } neighbor, null) => new CaseBoundary.NeighborWithoutDate(neighbor.Id)
    };
```

Now the whole flow is orieten towards the output structure shape. With patter matching on input only in individual helpers for output fields `Future` and `Past`


# 2.

```cs
public sealed record MonthlyWorkloadSummaryDto(
    double TotalHours,
    double BillableHours,
    int DaysWithEntries,
    IReadOnlyList<DateOnly> WorkdaysWithoutEntries);

public static MonthlyWorkloadSummaryDto GetMonthlySummary(
    IReadOnlyList<TimeRecording> recordings, int year, int month, IReadOnlySet<DateOnly> publicHolidays)
{
    double totalHours = 0;
    double billableHours = 0;
    var daysSeen = new HashSet<DateOnly>();

    foreach (var recording in recordings)
    {
        totalHours += recording.DurationHours;
        if (recording.IsBillable)
            billableHours += recording.DurationHours;
        daysSeen.Add(DateOnly.FromDateTime(recording.Date));
    }

    var missing = new List<DateOnly>();
    var day = new DateOnly(year, month, 1);
    while (day.Month == month)
    {
        if (day.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday)
            && !publicHolidays.Contains(day)
            && !daysSeen.Contains(day))
            missing.Add(day);
        day = day.AddDays(1);
    }

    return new MonthlyWorkloadSummaryDto(totalHours, billableHours, daysSeen.Count, missing);
}
```

Fully procedural way. If we concetrate on the output shape, then we get 4 reducers for each field which also accept different data, because we concentrate on the question - what do I need from the input
to compute this field. This turns the top level function into a record constructor. For three of four functions we just need the time recording list. Because we firstly look at what we need as output and then define based on that the input for this subpart and for 3 of 4 field producers we just need the list itself. This actually reduces the cognitive load! A bit :)

```cs
public static MonthlyWorkloadSummaryDto GetMonthlySummary(
    IReadOnlyList<TimeRecording> recordings, int year, int month, IReadOnlySet<DateOnly> publicHolidays) =>
    new(
        TotalHours: GetTotalHours(recordings),
        BillableHours: GetBillableHours(recordings),
        DaysWithEntries: GetDaysWithEntries(recordings),
        WorkdaysWithoutEntries: GetWorkdaysWithoutEntries(recordings, year, month, publicHolidays));

public static double GetTotalHours(IEnumerable<TimeRecording> recordings) =>
    recordings.Sum(recording => recording.DurationHours);

public static double GetBillableHours(IEnumerable<TimeRecording> recordings) =>
    recordings.Where(recording => recording.IsBillable)
              .Sum(recording => recording.DurationHours);

public static int GetDaysWithEntries(IEnumerable<TimeRecording> recordings) =>
    recordings.Select(recording => DateOnly.FromDateTime(recording.Date))
              .Distinct()
              .Count();

private static IReadOnlyList<DateOnly> GetWorkdaysWithoutEntries(
    IEnumerable<TimeRecording> recordings, int year, int month, IReadOnlySet<DateOnly> publicHolidays)
{
    var daysWithEntries = recordings
        .Select(recording => DateOnly.FromDateTime(recording.Date))
        .ToHashSet();

    return DaysOfMonth(new DateOnly(year, month, 1))
        .Where(day => day.DayOfWeek is not (DayOfWeek.Saturday or DayOfWeek.Sunday))
        .Where(day => !publicHolidays.Contains(day))
        .Where(day => !daysWithEntries.Contains(day))
        .ToList();
}
```

# 3. 

Maybe not a big one, but another confirmation that this technique works in combination with a good output type structure.

```cs
public static (string statusText, List<string> missingFields) GetExportStatus(Participant participant)
{
    var missing = new List<string>();
    if (participant.SocialNumber == null) missing.Add("SVNr");
    if (participant.CurrentCase?.EntryDate == null) missing.Add("Eintritt");
    if (participant.CurrentCase?.ParticipantProjectPhaseId == null) missing.Add("Phase");

    if (!participant.SendToMentrix)
        return ("Excluded", missing);          // missing list is meaningless here but returned anyway
    if (missing.Count > 0)
        return ($"Blocked: {string.Join(", ", missing)}", missing);
    return ("Ready", missing);
}
```

After having the type which describes the same but now eliminates the set of all potential strings from the potential values set, encoding this into DU

```cs
public abstract record ExportStatus
{
    private protected ExportStatus() { }
    public sealed record Ready : ExportStatus;
    public sealed record Excluded : ExportStatus;                     // SendToMentrix == false
    public sealed record Blocked(IReadOnlyList<string> MissingFields) : ExportStatus;
}
```

Same thinking process as in the first example - when can the eac concrete case happen and from what. Although we still pattern match the incoming data, its concrete shape and all different cases do not bother us. We only care about the output data shape. And again one producer for a field:

```cs
public static ExportStatus GetExportStatus(Participant participant) =>
    (participant.SendToMentrix, GetMissingFields(participant)) switch
    {
        (false, _)              => new ExportStatus.Excluded(),
        (true, { Count: > 0 } missing) => new ExportStatus.Blocked(missing),
        (true, _)               => new ExportStatus.Ready()
    };

private static IReadOnlyList<string> GetMissingFields(Participant participant) =>
    new (string Label, bool IsMissing)[]
    {
        ("SVNr",    participant.SocialNumber == null),
        ("Eintritt", participant.CurrentCase?.EntryDate == null),
        ("Phase",   participant.CurrentCase?.ParticipantProjectPhaseId == null)
    }
    .Where(field => field.IsMissing)
    .Select(field => field.Label)
    .ToList();
```