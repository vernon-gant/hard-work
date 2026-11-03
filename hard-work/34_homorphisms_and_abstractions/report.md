# Building hierarchy to early

There are not that much good hierarchies in our project and those which I found are somehow artificial "so that they just exist", but I will try. There is a service responsible for creating a time recording, which looks like this :

```c#
  public abstract class TimeRecordingServiceBase<TEntity>(ILogger<TimeRecordingServiceBase<TEntity>> logger)
        where TEntity : BaseEntity
    {
        public abstract TimeRecording ConvertToTimeRecording(TEntity entity, Guid relatedProjectId);

        protected abstract Task CheckTimeAvailability(TEntity entity, Guid relatedProjectId, DbOperationMode mode);

        protected abstract Task<Guid> GetProjectIdFromEntity(IAmpolDbContext dbContext, TEntity entity);

        public async Task CreateWithTimeCheck(IAmpolDbContext dbContext, TEntity entity, bool throwExceptionsOnlyForOverlaps = false)
        {
            ...
        }
        
        public async Task UpdateWithTimeCheck(IAmpolDbContext dbContext, TEntity entity, bool throwExceptionsOnlyForOverlaps = false)
        {
            ...
        }

        public async Task DeleteTimeRecording(IAmpolDbContext dbContext, Guid entityId)
        {
            ...
        }

        private protected async Task DoCheckTimeAvailability(TEntity entity, Guid projectIdFromEntity, DbOperationMode mode, bool throwExceptionsOnlyForOverlaps) ...
        
        private protected async Task UpdateTimeRecording(IAmpolDbContext dbContext, TEntity entity, Guid relatedProjectId) ...
    }
```

Their idea was probably that we want to work with some entity which produces then a time recording, subclasses then overwrite how the TimeRecording is created from this type(we could just use higher order functions here, of course) and then there is a suspicious

```c#
 public abstract class TimeRecordingServiceBaseWithTimeBooking<TEntity>(ILogger<TimeRecordingServiceBase<TEntity>> logger) : TimeRecordingServiceBase<TEntity>(logger)
        where TEntity : BaseEntity, ITimeBooking
{
  ...
}
```

It does not really matter what is inside, they just wanted to extend the behavior for the entity which is also a time booking, however this mid class is exactly like described in the lesson in terms of just having a nice abstract class so that we could use the upper one for entities which are not time booking. The problem is, that it is used nowhere and the upper one is used only with entities which are `ITimeBooking`. This also makes sense from the operations defined, if we want to check time availability, then with 90% I can say that it sounds like it is some sort of a time related entity, in the case of our domain it is a booking. So this additional layer can be eliminated. The first two methods of the very base class can be at all trasnferred to the `ITimeRecordingRules<T>` even where T is not typed to `ITimeBooking` if you do not want to couple to it. This would be the more less good direction to adding new behavior to a type encapsulated into a separate ADT.


# Best work around for one edge case

```cs
public abstract class ExportServiceBase<TEntity, TId>(
    IDbContextFactory<AmpolDbContext> dbContextFactory,
    ILogger<ExportServiceBase<TEntity, TId>> logger,
    DownloadConfiguration downloadConfig,
    TemplateConfiguration templateConfiguration)
    where TEntity : class, IIdEntity<TId>, new()
{
    ...
}

public abstract class ExportServiceBase<TEntity>(
    IDbContextFactory<AmpolDbContext> dbContextFactory,
    ILogger<ExportServiceBase<TEntity, Guid>> logger,
    DownloadConfiguration downloadConfig,
    TemplateConfiguration templateConfiguration)
    : ExportServiceBase<TEntity, Guid>(dbContextFactory, logger, downloadConfig, templateConfiguration)
    where TEntity : class, IIdEntity, new();
```

This is the last example which I found, but also a good one. The middle class has an empty body, and its only job is to fix `TId = Guid`. `TId` is never anything else in the system. The five exports skip the middle class pass Guid too. They skip it only because` ApplicationUser` implements `IIdEntity<Guid>` but not the non-generic IIdEntity that the shortcut requires.


# Interface dispatch for time booking

In C# there is no interface dispatch, but to implement the idea from the lesson we need an ADT with a small set of operations and then standalone functions over them aka extension methods. The first example as I mentioned at the end can be refactored, so that we define this small subset of operations into `ITimeRecodrdingRules<TEntity>`, which was previously handled by overrides in each concrete sertive. We could do something like

```c#
public interface ITimeRecordingRules<TEntity>
{
    TimeRecording ToTimeRecording(TEntity entity, Guid projectId);
    Task CheckAvailability(TEntity entity, Guid projectId, DbOperationMode mode);
    Task<Guid> GetProjectId(IAmpolDbContext dbContext, TEntity entity);
}
```

and then extract the code in the base class into standalone extension methods.

```c#
public static class TimeRecordingOperations
{
    public static async Task CreateWithTimeCheck<TEntity>(this ITimeRecordingRules<TEntity> rules, ...) where TEntity : BaseEntity
    {
        var projectId = await rules.GetProjectId(dbContext, entity);
        await rules.DoCheckAvailability(entity, projectId, Create, throwExceptionsOnlyForOverlaps);

        dbContext.Set<TEntity>().Add(entity);
        dbContext.TimeRecordings.Add(rules.ToTimeRecording(entity, projectId));
        await dbContext.SaveChangesAsync();
    }

    public static async Task<TEntity> UpdateDateById<TEntity>(
        this ITimeRecordingRules<TEntity> rules, IAmpolDbContext dbContext, ILogger logger, UpdateDateByIdDto dto)
        where TEntity : BaseEntity, ITimeBooking
    {
        ...
    }
}
```

Still looks ugly when we know that ports and adapters exist, but at least we remove the three level hierarchy and it becomes six standalone implementations of one interface + functions over this interface.
