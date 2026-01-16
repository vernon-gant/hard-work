// C# does not support multiple inehritance, only implementing multiple interfaces is supported. But conceptually we could represent
// our enclosed hierarch this way

public interface IEventStoreEvent
{
    public Guid EventId { get; }

    public Guid ActorId { get; }

    public DateTimeOffset CreatedAt { get; }

    public DateTimeOffset ChangedAt { get; }
}

public interface IEventStoreCreatedEvent : IEventStoreEvent
{
    public Guid Id { get; }
}

public interface IResourceEventVisitor
{
    public ValueTask Visit(EquipmentCreated event);

    public ValueTask Visit(PersonCreated event);

    public ValueTask Visit(PlaceCreated event);
}

public abstract class ResourceEvent<T> where T : IEventStoreEvent
{
    public async ValueTask Visit(T visitor)
    {
        await visitor.Visit(this);
    }
}

public class EquipmentCreated<T> : ResourceEvent<T> where T : IResourceEventVisitor;
public class PersonCreated<T> : ResourceEvent<T> where T : IResourceEventVisitor;
public class PlaceCreated<T> : ResourceEvent<T> where T : IResourceEventVisitor;

public class EventValidationVisitor : IResourceEventVisitor
{
    public async ValueTask Visit(EquipmentCreated event)
    {
        if (event is None)
        {
            ...
        }
        ...
        // to be able to work with None polymorphically we need to add an explicit check for it, otherwise we get an exception
        // altough visitor assumes that we dispatch operation on various types which are assumed to be valid types, i still wanted to add this example because this is
        // how working with None really looks like - in every such method we check if current object is not None and only then perform the operation.
    }
}

public sealed class None : EquipmentCreated, PersonCreated, PlaceCreated, IEventStoreCreatedEvent, EventValidationVisitor;