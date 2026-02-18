# OpenXML SDK

Not so long time ago I had to dig into OpenXML SDK from Microsoft for working with reports generated
using a word template. Current implementation uses a replacer which goes through the document, extracts the elements wiht special tags and replaces their values. However, in the replacement code
I saw that we mutate the nodes of the document itself. They are exposed using the `RootElement` property which is an `OpenXmlElement`. Each element links with a parent, next element and also exposes its child elements

```c#
public OpenXmlElement? Parent { get; internal set; }

internal OpenXmlElement? Next { get; set; }

public OpenXmlElementList ChildElements => new OpenXmlElementList(this);
```

This is actually the mutable tree described in the lesson. And here we get everything described from the lesson where a tree may appear not a tree but a graph due to circular references everything thanks to mutability. The `OpenXmlElement` API is mutable and plays around pointers like in any circular data structure. The part which I got interested with was the actual replacement of the value when we get to it in our replacer. We just mutate its state

```c#
textElementParent.Append(new Text(line));
```

And although I dealt with it before reading the lesson, it looked suspicious for me that we can mutate anything in this shared world. We can putate parent, next element, child elements, whatever we want. Of course internally these replacements will rearrange the tree correctly, but having an immutable world here would really simplify the general model. Because now we have side effects, shared state and all that stuff.

Of course I am not that smart yet to design such a big library and functionality with all that lazy loading, parsing and tree restructuring, but I take a look on it only from the perspective of the course. Firstly, we could remove the `Parent` pointer, so that we do not have to create a new parent when we say "replaced" a sibling node. We just have an undirectional tree from top to bottom where parents know their children.

```c#
public abstract record OpenXmlNode
{
    public ImmutableList<OpenXmlNode> Children { get; init; }
}
```

We also make it a record type, because there is no need to distinguish single nodes using some identity and two nodes are conceptually equal when all their fields are equal. Practially more important that now there can not be any side effects coming from some method call which changes something inside one node. We are explicit in the best manner of immutable world :)

# Cloning

I noticed suspicious cloning method around all the entities in the project. Basically every model in the project implements such methods

```c#
public object Clone()
{
  var participantPreparatoryAction = (ParticipantPreparatoryAction)MemberwiseClone();
  participantPreparatoryAction.Participant = null;
  return participantPreparatoryAction;
}

public ParticipantPreparatoryAction CloneWithParticipantId(Guid participantId)
{
  var participantPreparatoryAction = (ParticipantPreparatoryAction)Clone();
  participantPreparatoryAction.Id = Guid.Empty;
  participantPreparatoryAction.ParticipantId = participantId;
  participantPreparatoryAction.AttendanceEntries
  participantPreparatoryAction.AttendanceEntries
            .Select(x => x.CloneWithPrepAction(participantPreparatoryAction)).ToList();
  return participantPreparatoryAction;
}
```

As it turned out later they were operating with the EfCore models almost everywhere, and because EfCore does not work well with immutable types because of its internal ChangeTracking mechanism which maintains one single object for one identity entity they need to be comparably using a reference. But this introduces problems like here

```c#
private void EditHeader()
{
  _preparatoryActionBeforeEdit = (ParticipantPreparatoryAction) _preparatoryAction!.Clone();
  _editMode = EditMode.Header;
}

private async Task SaveBaseData()
{
  _attendanceEntriesLoading = true;
  try
  {
      var preparatoryActionCopy = (ParticipantPreparatoryAction) _preparatoryAction!.Clone();
      preparatoryActionCopy.AttendanceEntries = [];

      _preparatoryAction = await ParticipantPreparatoryActionDataService.SaveBaseData(preparatoryActionCopy);
      PreparatoryActionId = _preparatoryAction!.Id;
      await LoadPreparatoryAction();
      _editMode = EditMode.AttendanceEntries;
      _isNew = false;
      ...
  }
}

private async Task CancelHeader()
{
  _preparatoryAction = (ParticipantPreparatoryAction) _preparatoryActionBeforeEdit!.Clone();
  _editMode = EditMode.AttendanceEntries;
  await InvokeAsync(StateHasChanged);
}
```

In the Component we maintain two `ParticipantPreparatoryAction` entities to track the state before edit. And everywhere we clone the whole entity which contains like 15+ fields just because they both are mutable and Blazor two way binding will change both objects. So when user cancels the form we need to clone, otherwise he sees same fields . Immutable model would make the process easier. To achieve that, we move from the database model on the frontend to just a read only `View`. Of course we need to add a mutable form for this component with displayed fields, but the process of memorizing the previous state get straightforward

```c#
public record PreparatoryActionView(
    Guid Id,
    Guid ParticipantId,
    DateTime StartDate,
    DateTime EndDate,
    PreparatoryActionOverrideState? OverrideState,
    IReadOnlyList<AttendanceEntryView> AttendanceEntries);
    
public class HeaderForm
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public PreparatoryActionOverrideState? OverrideState { get; set; }

    public void Apply(PreparatoryActionView v)
    {
        StartDate = v.StartDate;
        EndData = v.EndDate;
        OverrideState = v.OverrideState;
    }

    public PreparatoryActionView Create(PreparatoryActionView original) =>
        original with { StartDate = StartDate, EndDate = EndDate, OverrideState = OverrideState };
}

private void EditHeader()
{
    _beforeEdit = _action;
    _form.Apply(_action);
}

private async Task SaveBaseData()
{
    _action = _form.ApplyTo(_action);
    _action = await DataService.Save(_action);
    _form = null;
    _beforeEdit = null;
}

private void CancelHeader()
{
    _action = _beforeEdit;
}
```

No cloning at all, pure C# language capabilities!

# Fluent Validation

```c#
/// <summary>
/// Information about rulesets
/// </summary>
public class RulesetMetadata {

		/// <summary>
		/// Creates a new RulesetMetadata
		/// </summary>
		/// <param name="name"></param>
		/// <param name="rules"></param>
		public RulesetMetadata(string name, IEnumerable<IValidationRule> rules) {
			Name = name;
			Rules = rules;
		}

		/// <summary>
		/// Ruleset name
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Rules in the ruleset
		/// </summary>
		public IEnumerable<IValidationRule> Rules { get; }
}
```

following the last practical advice in the lesson - do not allow any setters and only assign a new value to the variable - we can conclude that in the example above there is no need to add the mutability overhead. The object above is just a structure for ruleset metadata. Logically two metadata objects are equal if they contain same rules(same identity objects) and have the same name. Records solve this problem perfectly, although they do not implement the behavior we want for collections, on the conceptual level replacing class with record improves the model if we want later somehow differentiate the metadata objects.