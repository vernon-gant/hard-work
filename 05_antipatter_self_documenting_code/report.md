# NEventStore

I like the idea that comments is a good way to be polite for other developers. However indeed not all comments can be considered useful - when I only started my internship, the amount on investigation on the backend side was insane. Of course I tried to understand everything at once, however not this was the actual problem. Our project used NEventStore library which delivered abstractions for working with event based systems. There were parts of the code which were just hard to read - like poller implementation. However what I mostly struggled with is how the actual ADTs/components are used in the system iteself. Because the project has not even been launched yet, the documentation was only in the colleagues heads and I sometimes even did not know where to start reading investigating. However I clearly see the solution for this now - if I would see not such a short comment explaining the design level integration of this entity say here(although the property explanations are pretty good)

```c#
namespace NEventStore
{
    /// <summary>
    ///     Represents a series of events which have been fully committed as a single unit and which apply to the stream indicated.
    /// </summary>
    public interface ICommit
    {
        /// <summary>
        ///     Gets the value which identifies bucket to which the stream and the commit belongs.
        /// </summary>
        string BucketId { get; }

        /// <summary>
        ///     Gets the value which uniquely identifies the stream to which the commit belongs.
        /// </summary>
        string StreamId { get; }

        /// <summary>
        ///     Gets the value which indicates the revision of the most recent event in the stream to which this commit applies.
        /// </summary>
        int StreamRevision { get; }

        /// <summary>
        ///     Gets the value which uniquely identifies the commit within the stream.
        /// </summary>
        Guid CommitId { get; }

        /// <summary>
        ///     Gets the value which indicates the sequence (or position) in the stream to which this commit applies.
        /// </summary>
        int CommitSequence { get; }

        /// <summary>
        ///     Gets the point in time at which the commit was persisted.
        /// </summary>
        DateTime CommitStamp { get; }

        /// <summary>
        ///     Gets the metadata which provides additional, unstructured information about this commit.
        /// </summary>
        IDictionary<string, object> Headers { get; }

        /// <summary>
        ///     Gets the collection of event messages to be committed as a single unit.
        /// </summary>
        ICollection<EventMessage> Events { get; }

        /// <summary>
        /// The checkpoint that represents the storage level order.
        /// </summary>
        Int64 CheckpointToken { get; }
    }
}
```

but something like

```
Represents a logically atomic batch of domain events(from one to many) that has been persisted to the event store. Each commit is immutable and scoped to a single event stream, forming the abstraction of the append-only storage model. Each commit is produced by persisting a CommitAttempt through ICommitEvents, which abstracts durable interaction with the underlying store.
```

We could add even more relations between entities, because this would give the reader an idea of how ADTs relate to each other. Of course I got it later myself - however I had to figure it out  through code. And of course it took me a lot of time. After reading this we can quickly jump to say ICommitEvents, to its implementation, look for CommitAttempt and how to work with it - because we know the high level design. I wish I had it 2 years ago...

# grandnode2 - 1

I took this one becase I use it for my thesis, however I do not use it on the architecture level - more on the implementation level 2. But I found a good candidate with a misleading name for the ecommerce project

```c#
public class OrderBuilder<T>
{
    private readonly List<(Expression<Func<T, object>> selector, bool value, string fieldName)> _list = new();

    protected OrderBuilder() { }

    public IEnumerable<(Expression<Func<T, object>> selector, bool value, string fieldName)> Fields => _list;

    public static OrderBuilder<T> Create()
    {
        return new OrderBuilder<T>();
    }

    public OrderBuilder<T> Ascending(Expression<Func<T, object>> selector)
    {
        _list.Add((selector, true, ""));

        return this;
    }

    public OrderBuilder<T> Ascending(string fieldName)
    {
        _list.Add((null, true, fieldName));

        return this;
    }

    public OrderBuilder<T> Descending(Expression<Func<T, object>> selector)
    {
        _list.Add((selector, false, ""));

        return this;
    }

    public OrderBuilder<T> Descending(string fieldName)
    {
        _list.Add((null, false, fieldName));

        return this;
    }
}
```

As we know the code should know nothing about the program itself. However without knowing what Order they mean here - I swear I could not understand how ascending and descending relate to the Order - because I was thinking about the normal order which is placed by the customer, without knowing what components it interacts with, it is very hard to get the idea. So after some investigation

```c#
Builder for defining sort order for database queries. Used to construct composite ordering over fields when creating indexes or fetching ordered results. Consumed by components like IDatabaseContext.CreateIndex. Main idea is to expresses intent declaratively and hides low-level indexing mechanics.
```

Same idea - we describe how this component integrates into the system architecture. Of course we do not duplicate the architecture of the system - we just descirbe the integration to the parts where it is used or is supposed to be used. Like here - we mention that the idea is to build ordering for queries declaratively. We also menion where it is used. Plain and simple.


# grandnode2 - 2

Last example I want to choose is regarding two concepts which confused me in the project - Plugin and Module. I was confused because I saw the modules solution and plugins solution and then accidently opened this class

```c#
public interface IPlugin
{
    /// <summary>
    ///     Gets or sets the plugin info
    /// </summary>
    PluginInfo PluginInfo { get; set; }

    /// <summary>
    ///     Gets a configuration URL
    /// </summary>
    string ConfigurationUrl();

    /// <summary>
    ///     Install plugin
    /// </summary>
    Task Install();

    /// <summary>
    ///     Uninstall plugin
    /// </summary>
    Task Uninstall();
}
```

Yes we see the repetition of method names in the comments, however it is not clear how this component integrates into the system. And I was interested excaly in this. So after some investigation I figured out what the difference is. Plugins are like installable feature components - for example app provides us ShippingPointRatePlugin, SliderWidgetPlugin, FixedRateTaxPlugin and other plugins. They implement the contract and can be added or removed dynamically. On opposite, modules are larger feature blocks loaded at runtime based on the configuration. They can not be installed or uninstalled. This brings us to

```c#
Defines a contract for dynamically loaded, self-contained features that can be installed, uninstalled, and configured at runtime. Plugins extend the system with optional functionality (payments, integrations) and are managed through metadata in PluginInfo. Unlike modules, plugins follow a controlled lifecycle and can be conditionally included without application redeployment.
```