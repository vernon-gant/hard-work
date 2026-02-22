# 1. It is not a bug

### Timezone

Backend uses OData protocol for REST communication. In the OData there is an explicit configuration that the datetime objects are treated in the UTC format. The client of course sends the local datetime. In the database I see the local datetime with appended Z. But because the product is used only in one timezone nobody cares.

### Remove Nav Properties

Because backend uses OData and there is no a layer of a business logic to which we submit the data through endpoints, we operate with database models. Here comes the problem with EfCore, when we use the disconnected updated an updated an existing entity for which the nav proeprties are already set(and they are set because client sends everything), then EfCore generates the update statement for the navigation properties as well. Imagine such an updated statement for 5 tables when updating a single entity... Workaround is to explicitly set those nav entities in code before adding to EfCore to null. Same as with setters in the example - it would require to rewrite the whole code base to introduce DTOs and remove full model submission to the controllers. But we can simply set the nav properties to null and system will work as expected.


# 2. It is not a bug, but something else...

## List replacer

Project uses Word as a template file and then using controls offered by word in combination with adding metadat to them using "tags" the enging then replaced the content of these controls with the actual data. There is a custom "Replacement Engine" which does that using OpenXML SDK. An important point here are "list" control structures. We can wrap such a list around the table and then replacer will use the elements inside for data placeholders.

And here comes the BUG. Or not :) I opened the replacer and of course I have no idea about the OpenXML domain and the types inside, but the code in list replacer looked very weird to me. I opened the resulting XML and saw that instead of generating `n` items for the list the code generates `n` lists with 1 items inside. The XML was insanely polluted with all these lists with single element. The system works as expected, but the efficiency and potential problems when a lot of data will be used are obvious. Flaky implementation... Fortunately team lead told me : take your time, understand SDK and rewrite it!

## Type casts

```cs
if (!Modules.Any(m => m.Id == EntityIdConstants.ModuleIdCrm))
{
    Modules.Add(new() { Id = EntityIdConstants.ModuleIdCrm, ...});
    this.SaveChanges();
}

if (!Modules.Any(m => m.Id == EntityIdConstants.ModuleIdCrm))
{
    Modules.Add(new() { Id = EntityIdConstants.ModuleIdParticipant, ...});
    this.SaveChanges();
}

if (!Modules.Any(m => m.Id == EntityIdConstants.ModuleIdCrm))
{
    Modules.Add(new() { Id = EntityIdConstants.ModuleIdGroupEvent, ... });
    this.SaveChanges();
}
```

All three if conditions check for ModuleIdCrm, but blocks 2 and 3 are adding completely different modules (Participant and GroupEvent). Obviously in production all these modules already exist in the database and were seeded long ago by migrations in earlier runs. And later the logic was just copy pasted for other modules. On a fresh database only the block 1 would add a new module and the rest will be never executed. For copy pasting there must be a strict punishment!


# 3. Misleading

## Double polling

After submission of participant skills to the labor market API the client starts polling the server for updated. The confusing part is that there is a background service which does the update of the submitted entities aka polling using IDS provided by the service which sumbmitted them. But at the same time this service starts also polling using the fire and forget before returning response to the user. This is very confusing.

## Relevant for external provider

The most confusing part is that when we submit participant skills again to the labor market API then we do not filter out the ParticipantSkills by the `RelevantForExternalProvider` flag. It is just written in comments to ignore it. The name implies a bit different...


# 4.Fragile

## Event Class Names

In the project using Event Sourcing my colleauge accidently or deliberately changed the name of some event class. There we no build or testing errors, but after he merged the changes the dev was broken. It turned out that you should not change names of event classes because they are used then in the de/serialization. I swear there must be a different approach how to do this...

## Null checks

```cs
await AddOrEditReportQuests(reportQuestConfigurations, participant,
                    participantFromDbLocalWithCases,
                    advisorChanged,
                    participant.CurrentCase!.ParticipantCase!.ExitDate != null &&
                    participantFromDbLocalWithCases.CurrentCase!.ParticipantCase!.ExitDate == null,
                    participant.CurrentCase!.ParticipantCase!.ExitDate == null &&
                    participantFromDbLocalWithCases.CurrentCase!.ParticipantCase!.ExitDate != null);
```

These null forgiving operators is a real evil. It's okay when we use them to disable warning in the case when we set the variable 100% in code, but because it is in the testing class with its own initialiation method, compiler can not infer it. This is one of the drawbacks of this procedural programming with models. We load the entity directly from the database using `Include()` and we know that at this point the value it not null. But if someone changes the query above, because OCP is ignored anyways? A good type system on the domain layer solve so many problems...


# 5. It violates safety

## Giant double switch

We have a giant switch for 16 different types of components which are unified in one `FormField` component to which we just pass the enum "Type of the field" and then set parameters we want. But the thing is that this component defines a horizontal and vertical layouts, and because of duplication/not keeping in head that 200 lines below there is a duplicate switch with another 16 statements, it is very easy to get bugs where we updated the first one and did not updated the second one. The code just promotes you for that...

## Using both System.Text.Json and Newtonsoft

In the code I found the usage of two libraries in the startup class. Honestly, the first wish was to delete the Newtonsoft registration, because it did not make any sense to me. But it turned out that some models use attributes from System.Text.Json and some from the Newtonsoft. Some methods using one serializer and some other. This is very dangerous, because it introduces inconsistency and lets you fall into a trap when you use attributes from one library, but they are ignored because some method uses serializer from anothe library.