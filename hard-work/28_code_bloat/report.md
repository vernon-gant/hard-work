# Typestate Oriented Programming

```cs
public class CompanyConsultationJournal : BaseEntity, IHasAdvisor, IEntityDescription, IHasDocument
{
    public Guid CompanyConsultationPhaseId { get; set; }
    public Guid? AdvisorId { get; set; } = null!;
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public int DurationInMinutes { get; set; }
    public string? Content { get; set; }
    public Guid CompanyConsultationJournalTypeId { get; set; }
    public Guid? DocumentId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter<CompanyConsultationJournalState>))]
    public CompanyConsultationJournalState State { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter<CompanyConsultationContactType>))]
    public CompanyConsultationContactType ContactType { get; set; } = CompanyConsultationContactType.FaceToFace;
}

public enum CompanyConsultationJournalState
{
    Scheduled = 1,
    Done = 2,
    CancelledByAdvisor = 3,
    CancelledByCompany = 4,
}
```

Now I can name what I posted as example in the previous report with type state oriented programming - this is the code bloat! I was instantly finding that suspicious that team lead is always mentioning that you can not change or touch this field in this class, because it is used or set or interpreted somehwere else. It turns out this is a bloated code :) But this is a good example, and even better that I know understand how to approach it, because in the example above the `Content` is meaningless in the `Sceduled` state. The `DocumentId` is also meaningles when the state is `Cancelled`. We can transit it to more explicit states carrying non optional, or even optional information, but which will make sense and do not inroduce these hidden invariants.

```cs
public interface IJournalState
{
    TReturn Accept<TReturn>(ITotalVisitor<TReturn> visitor);
}

public interface IActiveState : IJournalState
{
    TReturn AcceptActive<TReturn>(IActiveVisitor<TReturn> visitor);
}

public interface ICompletedState : IActiveState
{
    TReturn AcceptCompleted<TReturn>(ICompletedVisitor<TReturn> visitor);
}

public interface IActiveVisitor<TReturn>
{
    TReturn Visit(ScheduledJournal j);
    TReturn Visit(DoneJournal j);
}

public interface ICompletedVisitor<TReturn>
{
    TReturn Visit(DoneJournal j);
}

public sealed record ScheduledJournal(
    Guid Id,
    DateTime PlannedStart,
    DateTime PlannedEnd,
    Guid AdvisorId
) : IActiveState, IPendingState
{
    public TReturn Accept<TReturn>(ITotalVisitor<TReturn> v) => v.Visit(this);
    public TReturn AcceptActive<TReturn>(IActiveVisitor<TReturn> v) => v.Visit(this);
    public TReturn AcceptPending<TReturn>(IPendingVisitor<TReturn> v) => v.Visit(this);
}

public sealed record DoneJournal(
    Guid Id,
    DateTime ActualStart,
    DateTime ActualEnd,
    string Content,
    Guid AdvisorId,
    Guid? DocumentId
) : ICompletedState, ITerminalState
{
    public TReturn Accept<TReturn>(ITotalVisitor<TReturn> v) => v.Visit(this);
    public TReturn AcceptActive<TReturn>(IActiveVisitor<TReturn> v) => v.Visit(this);
    public TReturn AcceptCompleted<TReturn>(ICompletedVisitor<TReturn> v) => v.Visit(this);
    public TReturn AcceptTerminal<TReturn>(ITerminalVisitor<TReturn> v) => v.Visit(this);
}
```

A bit too much interfaces here, because now we couple the state wiht its capabilities through explicitly saying which visitors it may accept. We can still implement total visitors and call them through reference to the `IJournalState`, this will apply the total visitor. But then we can do the `if e is IActiveState s` and call the `Accept` method which will work only with visitor accepting active states.
Drawbacks of this approach? On the database level this would require this EfCore table inheritance and that would bloat the database table. Is it that bad? Actually we shifted the bloat to the boundary and
I think this could be an acceptable trade off.

# Bloated fields

This was actually a nightmare when I had to just change that `ModuleSettings` now have not a single `BaseProjectId` but a one to many relation. Because I had to touch this page:

```cs
private ReentryDto? _reentryDto;
private ReentryDto _ueberlassungDto = null!;
private UeberlassungModuleSettings? _ueberlassungModuleSettings;

private bool _participantProjectHasFollowup;
private bool _ueberlassungActive;
private bool _canEditParticipant;
private bool _jobNetModuleActiveInParticipantProject;
private bool _jobNetModuleActiveInSelectedProject;

@if(IsBBEProject)
{

}
else if (_personBBEProjects.Count > 0)
{

}
else if (_ueberlassungDto.ProjectId is not null)
{

}

var newEntry = await CheckIfParticipantIsInProject(_reentryDto!.ProjectId);

if (newEntry)
{
    try
    {
        var reentryParticipant = await ParticipantDataService.Reentry(_reentryDto);
        DialogService.Close(reentryParticipant);
    }
}

...

if (newEntry)
{
    try
    {
        var reentryParticipant = await ParticipantDataService.Reentry(_ueberlassungDto);
        DialogService.Close(reentryParticipant);
    }               
}
```

This is exactly this Windows API pattern mentioned in the lesson but more spread in the class. We have three tabs when participant is sent to the new project : just new project, reentry the old project
and entry a project of another kind. Every this tab owns its own fields, which in turn depend on each other's state. Why do I write about this? Because 3 days ago my team lead came to me and said that
I changed the variable somewhere and the variable was not initialized in the tab because the tab was rendered and bla bla bla, because I stopped listening. No way I told him I would figure it out. And the reason for that is exactly the bloat, because I changed the field which was shared. Our customer for this lesson :) The more natural approach for these manipulations would be to split it into components with its own invariants.

```cs
<RadzenTabs>
    <Tabs>
        <RadzenTabsItem Text="Neueintritt">
            <NeueintrittTab ParticipantId="@ParticipantId"
                            ProjectId="@ProjectId"
                            IsCaseActive="@IsParticipantCaseActive"
                            OnDone="@(p => DialogService.Close(p))"
                            OnCancel="@(() => DialogService.Close())" />
        </RadzenTabsItem>
        <RadzenTabsItem Text="Übertritt">
            <UebertrittTab ParticipantId="@ParticipantId"
                           PersKey="@PersKey"
                           OnDone="..." OnCancel="..." />
        </RadzenTabsItem>
        <RadzenTabsItem Text="@_ueberlassungsType.GetDisplayName()" Visible="@_ueberlassungActive">
            <UeberlassungTab ParticipantId="@ParticipantId"
                             ProjectId="@ProjectId"
                             PersKey="@PersKey"
                             IsCaseActive="@IsParticipantCaseActive"
                             OnDone="..." OnCancel="..." />
        </RadzenTabsItem>
    </Tabs>
</RadzenTabs>
```

In this case each tab will initialize its own data and all external shared data is passed from the parent component which loads `Participant` and other stuff. I will not paste the code of the page, because the page is 500 lines...

# Quietly mutating tables

```cs
private async Task UpdateRequestStatus(AmpolDbContext dbContext, AmsSkillsApiRequestStatusEntry request)
{
    // ... HTTP poll, status mapping, parse response ...

    if (request.Status == AmsSkillsApiRequestStatus.Done)
    {
        var (actualStatus, errors) = await ProcessApiResponse(response);
        // ...
        if (errors.OutdatedAmsSkillTnrs.Count > 0)
        {
            request.OutdatedAmsSkillTnrs = errors.OutdatedAmsSkillTnrs;
            await dbContext.AmsSkills.Where(x => errors.OutdatedAmsSkillTnrs.Contains(x.Tnr) && !x.Outdated).ForEachAsync(x => x.Outdated = true);
            await dbContext.AmsSubSkills.Where(x => errors.OutdatedAmsSkillTnrs.Contains(x.Tnr) && !x.Outdated).ForEachAsync(x => x.Outdated = true);
        }
    }

    await dbContext.SaveChangesAsync();
}
```

Not that critical bloat how in two previous cases, but if we consider bloated code as the gap between what the code shows and what you must know in order to use it, then the `UpdateRequestStatus` is actually lying to us. But this is again about keeping in the head, that when we update the status of request then we must also set all the outdated skills because we get the response from the API
which contains bla bla bla. And the solution here is not to change the method or use the event pattern with Mediator, we should not care at all about these things when sending the information about participant, because this is what the endpoint is doing. I would create the nightly job which does this, because this is a complete separate concern and must not be tied to a specific set of skills which are assigned to a specific participant. We shoudl weekly update all skills which got outdated.