To continue research on the functional concepts using language-ext and previous lessons, I will use the already introduced category theory objects, actually one of it - monad. Semigroup, monoids and groups
are interesting of course, but this requires even bigger mental shift to thinking about types, how we combine the two values of A under some operation f() so that we get A, whether there is an identity
and so on. With monads we think about how to combine computation steps which carry the same kind of context. And because we already covered a lot of them, practicing how to see them in code and use
their "precise" mathematical properies like associativity of bind rather than thinking on the code level is a good choice!

Hand rolling custom abstraction instead like the provided "IMEI for a phone" is for sure not a beginnger and even mid level dev task, of course in terms of finding a good one :) Hand rolled leaky abstractions can be found almost everywhere encapsulated into interfaces, but we now know that working with an interface does not strictly mean that we are working with an abstraction. We could, but in most cases we not. Hand rolling a custom monad from the domain is complicated for me right now, so I will stick to covered monads.

# Time Slots using List

```cs
private static List<TimeSlotResponseDtoWithCreatedUser> SubtractTimeSlots(List<TimeSlotResponseDtoWithCreatedUser> timeSlots, TimeSlotResponseDtoWithCreatedUser subtrahend, int minTime)
{
   foreach(var timeSlot in timeSlots.ToList())
   {
        if(timeSlot.StartDateTime <= subtrahend.EndDateTime && timeSlot.EndDateTime >= subtrahend.StartDateTime)
        {
            if (timeSlot.StartDateTime < subtrahend.StartDateTime && timeSlot.EndDateTime > subtrahend.EndDateTime)
            {
                // split time slot into two parts
                var clone = (TimeSlotResponseDtoWithCreatedUser) timeSlot.Clone();

                timeSlot.EndDateTime = subtrahend.StartDateTime;

                clone.StartDateTime = subtrahend.EndDateTime;
                timeSlots.Add(clone);
            }
            else if (timeSlot.StartDateTime >= subtrahend.StartDateTime && timeSlot.EndDateTime <= subtrahend.EndDateTime)
            {
                timeSlots.Remove(timeSlot);
                continue;
            }
            else if (timeSlot.StartDateTime < subtrahend.StartDateTime)
            {
                timeSlot.EndDateTime = subtrahend.StartDateTime;
            }
            else if (timeSlot.EndDateTime > subtrahend.EndDateTime)
            {
                timeSlot.StartDateTime = subtrahend.EndDateTime;
            }
        }

        if((timeSlot.EndDateTime.TimeOfDay - timeSlot.StartDateTime.TimeOfDay).Duration().TotalMinutes < minTime)
        {
            // if time slot is too short, remove it
            timeSlots.Remove(timeSlot);
        }
   }

   return timeSlots;
}
```

Very long method doing ad hoc in place mutations. The idea is that we want to subtract from all given availabilities of the advisor the blocker. The processing is however done per slot base, this makes sense because this is the smalles atomic operation. We get a slot and a blocker and decide what to return. It may return no available slot, if blocker covers input time slot, split time slot if blocer is in between, trimmed from end or start if they intersect on edge or just the time slot if they do not overlap. Because we are interested in subtracting many blockers, where each previous result is fed to the next blocker, because each blocker needs to be taken into account, I have just described the usage of List monad and its bind operation. The 09:00-17:00 may be split to 09:00-12:00 and 13:00-14:00 by the first blocker and this must be fed to the next blocker.

```cs
static Seq<Slot> Subtract(Slot slot, Slot blocker) =>
    Overlaps(slot, blocker)
        ? (StartsBeforeBlocker: slot.Start < blocker.Start,
           EndsAfterBlocker:    slot.End   > blocker.End) switch
          {
              (true,  true)  => [slot with { End   = blocker.Start },
                                 slot with { Start = blocker.End   }],
              (true,  false) => [slot with { End   = blocker.Start }],
              (false, true)  => [slot with { Start = blocker.End   }],
              (false, false) => []
          }
        : [slot];

static Seq<Slot> SubtractAll(Seq<Slot> slots, Seq<Slot> blockers) => blockers.Fold(slots, (surviving, blocker) => surviving.Bind(s => Subtract(s, blocker)));
```

The only operation which we had to preserve from the domain was that we need to apply the next blocker to what survided the previous one. The previous blocker could survive 0, 1 or 2. Concatenating the results is exactly the bind of the list. The single subtract is associative so yeah list is the way to go here!


# 2. Reader for look ups

```cs
private async Task<Participant> CopySingleParticipant(
    ParticipantFollowUpProjectDto dto,
    Participant participantToCopy,
    GlobalProjectCache globalProjectCache,   // read by everything
    ParticipantMappings participantMappings) // read by everything
{
    ...
    CopyParticipantCaregivingInformation(participantToCopy, globalProjectCache, participant);
    CopyJournals(participantToCopy, globalProjectCache, ...);
    CopySustainabilityQuest(globalProjectCache, participantMappings, participant);
    ...
}
```

I mean this is an easy one because the requirement in the domai is that some data in this case the global cache and mapping are read by multiple operations during creation of copy of a participant. Reader is the answer here! Environment is provided once at the edge and vanishes from the signatures. is extracted once and only needed data is supplied to the methods **using exact same value**. Environment is read only and can not be thus corrupted.


# 3. Writer for file revert

```cs
catch (Exception)
{
    DeleteAlreadyAddedParticipantDocuments(...);
    DeleteAlreadyAddedParticipantProfilePictures(...);
    await transaction.RollbackAsync();
    throw;
}
```

Method is quite long spagetti code so I will not paste it fully, but the idea is that we are creating participants in the follow up project and in the method there are two effects - the database side and the file system side for created documents. They accumulate the `newParticipants` only for this purpose, to extract the releavnt information from every participant in the delete functions. Database rollback of course comes for free. So the requirement can be formulated as that every file operation must have a matching undo record. For document files this is the document path, for profile pictures the id of the picture. This is exactly the writer's log append behavior which we need here.

```cs
public abstract record CopyParticipantFileUndo
{
    public sealed record DeleteDocumentFile(string RelativePath)  : CopyParticipantFileUndo;
    public sealed record DeleteProfilePicture(Guid ParticipantId) : CopyParticipantFileUndo;
}

static Writer<Seq<Compensation>, Unit> CopyDocuments(Participant source, Participant target) =>
    from copied in liftIO(participantDocumentService.CloneParticipantDocuments(target.Id, source.Documents))
    from _      in tell(copied.Values.Map(document => new DeleteDocumentFile(document.FilePath)).ToSeq())
    select unit;

static Writer<Seq<Compensation>, Unit> CopyProfilePicture(Participant source, Participant target) =>
    from _  in liftIO(profilePictureService.CloneParticipantProfilePicture(source, target))
    from __ in tell(Seq<Compensation>(new DeleteProfilePicture(target.Id)))
    select unit;
```


# 4. State for the growing look-up table

```cs
// maybe entries of the same participant have been created during this import
var existingEntries = existingParticipantsByExternalId.GetValueOrDefault(
    externalId,
    createdParticipants.Where(x => x.ExternalId == externalId).ToList());
...
var persKey = existingEntries.FirstOrDefault()?.PersKey ?? 0;
createdParticipants.Add(newParticipant);
```

The domain another import subsystem with subtle different behavior. When we process a single entry from the import file we must take into account the previously processed values. The not intuitive code above demostrates exactly this. The `byExternalId` mapping is fetched once from the database and the `createdParticipants` grows after each row processing. Because when we meet the row with the same id in the import, then we update it and do not ignore or throw an exception. State is the answer! We use its "state threading" behavior to get the access to the updated state after each row processing, more precisely in the update after create. The code logically follows from the design :) The domain operation property is preserved and everybody is happy:

```cs
static State<Map<string, Seq<Participant>>, Participant> Resolve(MoniModel model) =>
    from table   in get<Map<string, Seq<Participant>>>()
    let entries  = table.Find(model.ExternalId).IfNone(Seq<Participant>())
    from result  in entries.Find(entry => entry.CaseExternalId == model.CaseExternalId)
                           .Match(Some: entry => Update(model, entry),
                                  None: ()    => Create(model, entries))
    select result;

static State<Map<string, Seq<Participant>>, Participant> Create(MoniModel model, Seq<Participant> entries) =>
    from created in lift(() => CreateParticipant(model, entries))
    from _       in modify<Map<string, Seq<Participant>>>(table => table.AddOrUpdate(model.ExternalId, entries.Add(created)))
    select created;
```

# 5. Either for the save preconditions

```cs
public async Task<CompanyAcquisitionJournal> UpdateWithTimeCheck(Guid id, CompanyAcquisitionJournal journal, byte[]? newFileContent = null)
{
    CompanyAcquisitionJournal? entityFromDb = await dbContext.CompanyAcquisitionJournals.FindAsync(id);
    if (entityFromDb is null)
        throw new EntityNotFoundException($"Entity with id {id} not found.");

    (Guid companyConsultationActId, Guid projectId, Guid companyId) = GetConsultationActInformation(journal);

    if (journal.State == CompanyAcquisitionJournalState.Done)
        await CheckTimeAvailability(journal, projectId, false);

    ...
    await dbContext.CompanyConsultationActs.Where(...).ExecuteDeleteAsync();
    entityFromDbEntry.CurrentValues.SetValues(journal);
    await UpdateTimeRecording(journal, projectId, companyConsultationActId);
    await dbContext.SaveChangesAsync();
    ...
    dbContext.TimeRecordings.Add(ConvertToTimeRecording(...));
```

This is the main problem of exceptions, that method signatures lie and we can not see from the code itself that say `GetConsulatantActInformation` will throw on some condition. And there are a lot of conditions in general, because the domain is that acquision journal may be saved only if the journal it claims to update exists, the acquisition is belongs to still belongs to the consultation act, the advisor is still assigned to it and many other things. All these preconditions are for preventing the save. Current code encodes them differently as I saw, some with explcit exceptions, some with implicit through `First()` which throws where element is missing or we get some `ArgumentOutOfRange` in the state pattern matchign when invalid state transition was produced. So the domain actually wants to encode, all checks, then all writes and consistent reason return if save can not be performed. So we get the `Or(rejection, approve for update)` which is exactly the `Either<Error,A>` or in modern langauge-ext this is the `Fin<A>`.

```cs
public static class JournalErrors
{
    public static readonly Error NotFound             = Error.New(4040, "Acquisition journal not found");
    public static readonly Error ConsultationActGone  = Error.New(4041, "Acquisition has no consultation act");
    public static readonly Error NoAdvisorAssigned    = Error.New(4220, "No advisor assigned to the journal");
    public static Error TimeOverlaps(Seq<Overlap> blocking) =>
        Error.New(4090, $"Advisor is busy in {blocking.Count} recording(s)", new OverlapException(blocking));
}

static Fin<CompanyAcquisitionJournal> ExistingJournal(Guid id) =>
    dbContext.CompanyAcquisitionJournals.Find(id) is { } found ? found : JournalErrors.NotFound;

static Fin<ConsultationActInformation> ActInformation(CompanyAcquisitionJournal journal) =>
    QueryActInformation(journal.CompanyAcquisitionId) is { } info ? info : JournalErrors.ConsultationActGone;

static Fin<Guid> Advisor(CompanyAcquisitionJournal journal) =>
    journal.AdvisorId is { } advisorId ? advisorId : JournalErrors.NoAdvisorAssigned;

static Fin<Unit> TimeIsFree(Seq<Overlap> overlaps) =>
    overlaps.IsEmpty ? unit : JournalErrors.TimeOverlaps(overlaps);

static Fin<ApprovedUpdate> Approve(Guid id, CompanyAcquisitionJournal incoming, Seq<Overlap> overlaps) =>
    from existing  in ExistingJournal(id)
    from info      in ActInformation(incoming)
    from advisorId in Advisor(incoming)
    from _         in incoming.State is CompanyAcquisitionJournalState.Done ? TimeIsFree(overlaps) : Pure(unit)
    select new ApprovedUpdate(existing, incoming, info, advisorId);
```

I deliberately return the `ApproveUpdate` because this is the separate type, may even with transformed initial data, so that we clearly signalize that this is the approved update and not jsut return raw `Journal`. So in this case our abstraction retains the property of "permit with what was resolved" the either's *Rigth* or the refusal aka *Left*. Maybe not really abstraction abstraction like the list monad in the first example, but it clearly makes our reasoning preciser when we reason in exclusice terms which can never intersect by defintion and not now someone programmed it with exception handling.

# 6. Djkstra

I understand this exactly as in the article, we have the dirty domain with requirements, which how we can see(even in a simplified manner with ad hoc monad examples :) can be modelled with pure math world using the proven properties and not hand rolled assumptions. We just need to look at the problem at the right angle, holding the properties of these abstractions so that our initial requirement is fulfilled by it. It is complicated :) But potentially it gives such a precise code, that we can really look at it and just be sure what it will do at the end. Or not if someone still throws some very small exception somewhere at the bottom....