I even do not need to make up a case, because last week I had exactly a problem with this shape and this dictionary approach ruled it out. Because I could push onto the FP in our project
and team lead was not that agressive, I will use not classes, but records. But the idea however remains the same, because even if we expose the data we do not always want
to add a new field, this will make the type not wrong but something like misleading. For my import a new requirement was to add for each participant whose case was closed count of all hist
appointments which will be deleted. So we have following types for classification:

```cs
public abstract record ParticipantClassification
{
    private protected ParticipantClassification() { }

    public sealed record New : ParticipantClassification;

    public sealed record Existing(Guid ParticipantId, ExistingChange Change) : ParticipantClassification;

    public sealed record NoChange(Guid ParticipantId) : ParticipantClassification;

    public sealed record Corrupted : ParticipantClassification;
}

public abstract record ExistingChange
{
    private protected ExistingChange() { }

    public sealed record CoreOnly(ParticipantCoreChanges Core) : ExistingChange;

    public sealed record CaseOnly(CaseUpdate Case) : ExistingChange;

    public sealed record Both(ParticipantCoreChanges Core, CaseUpdate Case) : ExistingChange;

    public static Option<ParticipantCoreChanges> CoreOf(ExistingChange change) =>
        change switch
        {
            CoreOnly coreOnly => Some(coreOnly.Core),
            Both both => Some(both.Core),
            CaseOnly => None,
            _ => throw new UnreachableException()
        };

    public static Option<CaseUpdate> CaseOf(ExistingChange change) =>
        change switch
        {
            CaseOnly caseOnly => Some(caseOnly.Case),
            Both both => Some(both.Case),
            CoreOnly => None,
            _ => throw new UnreachableException()
        };
}

public abstract record CaseUpdate
{
    private protected CaseUpdate() { }

    public sealed record Terminated(
        DateTime BookedDate,
        DateTime? EntryDate,
        Option<ValueUpdate<DateTime>> Planned,
        DateTime ExitDate) : CaseUpdate;

    public sealed record NewEnrollment(
        ValueUpdate<DateTime> BookedDate,
        Option<ValueUpdate<DateTime>> EntryDate,
        Option<ValueUpdate<DateTime>> Planned) : CaseUpdate;

    public sealed record TransferFromExternalProject(
        ValueUpdate<DateTime>.Change BookedDate,
        Option<ValueUpdate<DateTime>> EntryDate,
        Option<ValueUpdate<DateTime>> Planned,
    ) : CaseUpdate;

    public sealed record UpdateCurrentCase(
        Option<ValueUpdate<DateTime>> BookedDate,
        Option<ValueUpdate<DateTime>> Planned) : CaseUpdate;
}
```

and the classification is then returned in the form of a dictionary where the key is social number `IDictionary<string, ParticipantClassification>`. And this additional computation, that for all terminated cases we compute the number of deleted appointments, does not really fit into the `CaseUpdate.Terminated`, because it is responsible for shaping new case for, which will be different for all these cases. Maybe if these additional computations would repeat, I would do some sort of a context for each such case typed with `T`, but YAGNI and just for single additional "field" for the `Terminated` case using just a dictionary of `string, int` is the easiest solution. We do not change any existing computations, just add a new function which retrieves the data based on the general set of classifications and returns it us. Because from the data perspective, this dict is just data and there is no need to "bind" it to the terminated case, we just want to have it. And this is really a perfect case for that!

`public record ClassificationResult(Dictionary<string, ParticipantClassification> Classifications, Dictionary<Guid, int> AppointmentsToDelete);`

I think this is definitely a very good tool for such cases where it definitely simplifies the data model, because adding these typed context objects for one field in one case is an overkill :) But spawning
10 dictionaries which hold dictionaries is also not the way to go, for heavy data retrieval tasks probably some other data structures should be used. The answer is always : it depends...