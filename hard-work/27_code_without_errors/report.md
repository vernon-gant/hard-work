# Introduction

I found the idea proposed in the lesson very promising if we can say so. Only after quickly reading the paper I got the idea of `unique`, `immutable` and `shared`. After reading about this `unique` alias
and that we want to prevent the already closed open file I though about this super fancy rust type borrowing system for the smartest programmers in the world. That if according to the paper the method
of the state if `state changing`

```java
state OpenFile extends File {
  private CFilePtr filePtr;
  public int read() { ... }
  public void close() [OpenFile>>ClosedFile]
  { ... }
}
```

then when we do something like

```java
unique OpenFile openFile = File.Open("data.txt");
unique ClosedFile closed = openFile.Close();
```

we move the ownership like Rust does this. And then we can not call the `openFile.Read()`. Anyways, encoding into the method signature even just that it either transits to a new state or not is mad. Because if this method is compiler enforced and guarantees that `f` will be in the closed state, then I do not know how to unsee it... We literally can not `close` it after opening, compilation error. Mad.

```
int readFromFile(ClosedFile f) {
  openHelper(f);
  int x = computeBase() + f.read();
  f.close();
  return x;
}
```

Very interesting, but we need to go back to C#. I read about `State` pattern in the articles, but for some reason I do not like this pattern at all. Throwing exceptions when the cart is empty and we want to compute the total looks weak. Implementing the normal state pattern where we just declare the `Execute` method, do something and return new state does not solve the problem of offering to the client only those operations which he may perform. Although in cases, where we literally can only `Execute` something and depending on the current state there must happen something different, I like it! Implementing it with `Stateless` library is even better, we just need to define the states and one trigger. In this case every state is reentrant, so when we perform the `Execute` then we do not change the state(for example).

```cs
public enum State
{
    Created,
    Completed,
    Pending,
    Cancelled,
    Reviewing
}

public enum Trigger
{
    Execute
}

var phoneCall = new StateMachine<State, Trigger>(State.Created);

phoneCall.Configure(State.Created)
    .PermitReentry(Trigger.Execute)
    .OnEntry(...);

phoneCall.Configure(State.Completed)
    .PermitReentry(Trigger.Execute)
    .OnEntry(...);

phoneCall.Configure(State.Pending)
    .PermitReentry(Trigger.Execute)
    .OnEntry(...);
```

I like this approach, it works, can be used, but it again does not solve the problem with `allow clients to have only those operations which the state allows`. I understand, that in C# with its type system we can not achieve what the paper was describing.

Here comes also another question, what if we want to have different behaviors depending on the state. We might want to have different behavior for validation, reporting, some processing, whatever, and concrete state will emit different behavior. Partial answer to both questions, could be the `Visitor`. But only to some extent, of course.

```cs
public class CompanyConsultationJournal
{
    public Guid CompanyConsultationPhaseId { get; set; }
    public Guid? AdvisorId { get; set; } = null!;
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter<CompanyConsultationJournalState>))]
    public CompanyConsultationJournalState State { get; set; }
    ...
}

public enum CompanyConsultationJournalState
{
    Scheduled = 1,
    Done = 2,
    CancelledByAdvisor = 3,
    CancelledByCompany = 4,
}
```

here I want to mention that using normal visitor approach will not work and we need the [asyclic visitor](https://github.com/Stepami/visitor-net) through interfaces. Just on the surface - we could extract the base class and then derive concrete classes which represent the state. I do not like that we have everywhere getters and setters, because we work directly on models, but this is how they did it... Anyway:

```cs
public abstract class CompanyConsultationJournal : ...
{
    public Guid CompanyConsultationPhaseId { get; set; }
    public DateTime StartDateTime { get; set; }
    public Guid CompanyConsultationJournalTypeId { get; set; }
    public string? Content { get; set; }
}

public abstract class ActiveJournal : CompanyConsultationJournal
{
    public Guid AdvisorId { get; set; }            // not nullable anymore
    ...
}

public sealed class ScheduledJournal : ActiveJournal, IVisitable<ScheduledJournal>
{
    public override TReturn Accept<TReturn>(IVisitor<CompanyConsultationJournal, TReturn> visitor) 
        => Accept(visitor);
    public TReturn Accept<TReturn>(IVisitor<ScheduledJournal, TReturn> visitor) 
        => visitor.Visit(this);
}

public sealed class DoneJournal : ActiveJournal, IVisitable<DoneJournal>
{
    public TReturn Accept<TReturn>(IVisitor<DoneJournal, TReturn> visitor) 
        => visitor.Visit(this);
}

public sealed class CancelledJournal : CompanyConsultationJournal, IVisitable<CancelledJournal>
{
    public CancellationParty CancelledBy { get; set; }
    public Guid? CancelledByAdvisorId { get; set; }
    public string? CancellationReason { get; set; }
    
    public TReturn Accept<TReturn>(IVisitor<CancelledJournal, TReturn> visitor) 
        => visitor.Visit(this);
}
```

The idea was, that now we write visitors, each conrete visitor will implement only those `IVisitor<T>` for states which are allowed. So for this method:

```cs
private async Task CheckTimeAvailability(CompanyConsultationJournal companyConsultationJournal, Guid projectId, bool isNew = false)
{
    if (companyConsultationJournal.State is CompanyConsultationJournalState.Done or CompanyConsultationJournalState.Scheduled)
    {
        await mediator.Send(new CheckTimeAvailabilityQuery(...));
    }
}
```

we write a visitor:

```cs
public sealed class TimeAvailabilityCheckVisitor(IMediator mediator, Guid projectId, bool isNew) :
    IVisitor<ScheduledJournal, Task>,
    IVisitor<DoneJournal,      Task>
{
    public Task Visit(ScheduledJournal j) => mediator.Send(new CheckTimeAvailabilityQuery(
        j.AdvisorId, projectId, null, j.StartDateTime, j.EndDateTime, null,
        isScheduling: true, isNew ? [] : [j.Id]));
    
    public Task Visit(DoneJournal j) => mediator.Send(new CheckTimeAvailabilityQuery(
        j.AdvisorId, projectId, null, j.StartDateTime, j.EndDateTime, null,
        isScheduling: false, isNew ? [] : [j.Id]));
}
```

and get

```cs
public async Task<CompanyConsultationJournal> CreateWithTimeCheck(ActiveJournal journal, ...) 
{
    var visitor = new TimeAvailabilityCheckVisitor(mediator, projectId, isNew: true);
    await journal.Accept(visitor);  // can not call with CancelledJournal
}
```

Why I like this visitor approach, is because we could write completely different visitors for completely different concerncs. Why is this so elegant, because it separates data from operations like in FP :)

```cs
public class TimeRecording
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; init; }
    
    public AggregatedTimeRecordingState State { get; init; }
}

public enum AggregatedTimeRecordingState
{
    Scheduled = 1,
    Participated = 2,
    Absent = 3,
    Cancelled = 4,
    Disease = 5,
}

return style + timeRecording.State switch
{
    AggregatedTimeRecordingState.Scheduled => $"border-color: {GetScheduledTimeRecordingBorderColor(timeRecording.StartDateTime)};",
    AggregatedTimeRecordingState.Participated => $"border-color: {AmpolColorPalette.GreenHexValue}; ",
    AggregatedTimeRecordingState.Absent => $"border-color: {AmpolColorPalette.RedHexValue}; ",
    AggregatedTimeRecordingState.Cancelled => $"border-color: {AmpolColorPalette.OrangeHexValue}; ",
    _ => throw new ArgumentOutOfRangeException()
};
```

Translates into:

```cs
public sealed class BorderColorVisitor :
    IVisitor<ScheduledTimeRecordingState, string>,
    IVisitor<ParticipatedTimeRecordingState, string>,
    IVisitor<AbsentTimeRecordingState, string>,
    IVisitor<CancelledTimeRecordingState, string>
{
    public string Visit(ScheduledTimeRecordingState s) 
        => GetScheduledTimeRecordingBorderColor(s.StartDateTime);
    
    public string Visit(ParticipatedTimeRecordingState _) 
        => AmpolColorPalette.GreenHexValue;
    
    public string Visit(AbsentTimeRecordingState _) 
        => AmpolColorPalette.RedHexValue;
    
    public string Visit(CancelledTimeRecordingState _) 
        => AmpolColorPalette.OrangeHexValue;
}
```

This is less focused on operations restriction based on the current state, but shows that we can use our state types in completely different contexts. There are plenty of other state enums which I could rewrite, but they would look the same way. I will definitely use more often. Actually I came up with it while just writing the introduction :)