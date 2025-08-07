# Introduction

I want to get back to my favourite grandnode2 project. If I understood the task correctly, then I though that it will take much more time at first. Because conrol flow patterns did not seem as something that widespread. After research, however, I came across these 3 cases(one of them was already present in one of my reports) and it turned out that we indeed can extract the control flow abstraction. However, in all 3 cases this was similar to the "with" pattern as I see now and I did not invent something new :) Is it bad? I do not think so, because if I undertood the task correctly - then the main purpose that we shift as much as possible on the level above in terms of some repeating control structure logic and leave open space for custom code. "With" in python will all its elegance does exaclty this - pull the control flow of resources to the construct itself and then leave space for the ***real logic***. FP starts looking more and more attractive for me :) Somehow it gives you that much power where you really need it...

## Querying

I have already partially covered this case in one of my previous reports. If we look again at the way how the guys handle filtering we will recognize this pattern - they are following the DDD model and have commands and queries with appropriate handlers. Some handlers return "bool" results, but we are more interested in the "get" queries where concrete entity is filtered and the its queryable is returned.

1. [Say this one](https://github.com/vernon-gant/grandnode2/blob/main/src/Business/Grand.Business.Checkout/Queries/Handlers/Orders/GetGiftVoucherQueryHandler.cs)
2. [Or this one](https://github.com/vernon-gant/grandnode2/blob/main/src/Business/Grand.Business.Checkout/Queries/Handlers/Orders/GetMerchandiseReturnQueryHandler.cs)

The control pattern in both of them and many others is simple - define the source, filter it(conditionally), sort and return a task because this is a mediator request handler. So at least 2 commands : defining the source and returning a task can be pulled up. Then we can also rework this ugly way of filtering - even without classes now. And we get

```c#
using Grand.Data;
using Grand.Domain;
using MediatR;
using System.Linq.Expressions;

namespace Grand.Business.Checkout.Queries.Handlers.Orders;

public abstract class BaseQueryHandler<TRequest, TEntity> : IRequestHandler<TRequest, IQueryable<TEntity>>
    where TRequest : IRequest<IQueryable<TEntity>>
    where TEntity : BaseEntity
{
    private readonly IRepository<TEntity> _repository;
    private readonly List<(Func<TRequest, bool> condition, Func<TRequest, Expression<Func<TEntity, bool>>> predicate)> _filters = new();

    protected BaseQueryHandler(IRepository<TEntity> repository)
    {
        _repository = repository;
        RegisterFilters();
    }

    public Task<IQueryable<TEntity>> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var query = _repository.Table;

        foreach (var (condition, predicate) in _filters)
        {
            if (condition(request))
                query = query.Where(predicate(request));
        }

        query = ApplyOrdering(request, query);

        return Task.FromResult(query);
    }

    protected abstract void RegisterFilters();
    protected abstract IQueryable<TEntity> ApplyOrdering(TRequest request, IQueryable<TEntity> query);

    protected FilterBuilder Filter(Expression<Func<TEntity, bool>> predicate) => new(r => predicate, _filters);

    protected FilterBuilder Filter(Func<TRequest, Expression<Func<TEntity, bool>>> predicateBuilder) => new(predicateBuilder, _filters);

    protected class FilterBuilder
    {
        private readonly Func<TRequest, Expression<Func<TEntity, bool>>> _predicate;
        private readonly List<(Func<TRequest, bool>, Func<TRequest, Expression<Func<TEntity, bool>>>)> _filters;

        internal FilterBuilder(
            Func<TRequest, Expression<Func<TEntity, bool>>> predicate,
            List<(Func<TRequest, bool>, Func<TRequest, Expression<Func<TEntity, bool>>>)> filters)
        {
            _predicate = predicate;
            _filters = filters;
        }

        public void When(Func<TRequest, bool> condition) => _filters.Add((condition, _predicate));

        public void WhenNotEmpty(Func<TRequest, string> getValue) => When(r => !string.IsNullOrEmpty(getValue(r)));

        public void WhenHasValue<TValue>(Func<TRequest, TValue?> getValue) where TValue : struct => When(r => getValue(r).HasValue);
    }
}
```

and rewrite existing handlers

```
public class GetGiftVoucherQueryHandler : BaseQueryHandler<GetGiftVoucherQuery, GiftVoucher>
{
    public GetGiftVoucherQueryHandler(IRepository<GiftVoucher> repository) : base(repository) { }

    protected override void RegisterFilters()
    {
        Filter(r => gc => gc.Id == r.GiftVoucherId).WhenNotEmpty(r => r.GiftVoucherId);

        Filter(r => gc => gc.PurchasedWithOrderItem.Id == r.PurchasedWithOrderItemId).WhenNotEmpty(r => r.PurchasedWithOrderItemId);

        Filter(r => gc => r.CreatedFromUtc.Value <= gc.CreatedOnUtc).WhenHasValue(r => r.CreatedFromUtc);

        Filter(r => gc => r.CreatedToUtc.Value >= gc.CreatedOnUtc).WhenHasValue(r => r.CreatedToUtc);

        Filter(r => gc => gc.IsGiftVoucherActivated == r.IsGiftVoucherActivated.Value).WhenHasValue(r => r.IsGiftVoucherActivated);

        Filter(r => gc => gc.Code != null && gc.Code == r.Code.ToLowerInvariant()).WhenNotEmpty(r => r.Code);

        Filter(r => c => c.RecipientName != null && c.RecipientName.Contains(r.RecipientName)).When(r => !string.IsNullOrWhiteSpace(r.RecipientName));
    }

    protected override IQueryable<GiftVoucher> ApplyOrdering(GetGiftVoucherQuery request, IQueryable<GiftVoucher> query) => query.OrderByDescending(gc => gc.CreatedOnUtc);
}
```

and the second one

```c#
public class GetMerchandiseReturnQueryHandler : BaseQueryHandler<GetMerchandiseReturnQuery, MerchandiseReturn>
{
    public GetMerchandiseReturnQueryHandler(IRepository<MerchandiseReturn> repository) : base(repository) { }

    protected override void RegisterFilters()
    {
        Filter(r => rr => r.StoreId == rr.StoreId).WhenNotEmpty(r => r.StoreId);
        Filter(r => rr => r.CustomerId == rr.CustomerId).WhenNotEmpty(r => r.CustomerId);
        Filter(r => rr => r.VendorId == rr.VendorId).WhenNotEmpty(r => r.VendorId);
        Filter(r => rr => r.OwnerId == rr.OwnerId).WhenNotEmpty(r => r.OwnerId);
        Filter(r => rr => rr.MerchandiseReturnStatusId == (int)r.Rs.Value).WhenHasValue(r => r.Rs);
        Filter(r => rr => rr.MerchandiseReturnItems.Any(x => x.OrderItemId == r.OrderItemId)).WhenNotEmpty(r => r.OrderItemId);
        Filter(r => rr => r.CreatedFromUtc.Value <= rr.CreatedOnUtc).WhenHasValue(r => r.CreatedFromUtc);
        Filter(r => rr => r.CreatedToUtc.Value >= rr.CreatedOnUtc).WhenHasValue(r => r.CreatedToUtc);
    }

    protected override IQueryable<MerchandiseReturn> ApplyOrdering(GetMerchandiseReturnQuery request, IQueryable<MerchandiseReturn> query) => query.OrderByDescending(rr => rr.CreatedOnUtc);
}
```

we concentrate only on the logic of filtering, no more repetitions. Fluent DSL syntax - okay this versio is just for example but we can definitely imrove it.

## CUD

Right after my dive into this project I saw many repetitve code in the services - more precisely for the "Insert", "Update" and "Delete" operations. The pattern is almost repetitive for most of the services, **with space for control flow** abstraction. What we are doing in all cases

1. Check the object for null(this is an old .NET project)
2. Manipulate the proeprties
3. Insert/Update/Delete from the repository
4. Notify with mediatr about the operation
5. Do something else

And most fall into 2 categories

1. [Have no custom logic at all](https://github.com/vernon-gant/grandnode2/blob/main/src/Business/Grand.Business.Catalog/Services/Brands/BrandService.cs#L112)
2. [With some custom logic](https://github.com/vernon-gant/grandnode2/blob/main/src/Business/Grand.Business.Catalog/Services/Categories/CategoryService.cs#L425)

So what we do is we extract the whole generic behavior and just leave space for derived classes to override the "Before/After" methods **if needed**.

```c#
using Grand.Data;
using Grand.Domain;
using Grand.Infrastructure.Extensions;
using MediatR;

namespace Grand.Business.Common.Services;

public class BaseCudService<TEntity> where TEntity : BaseEntity
{
    private readonly IRepository<TEntity> _repository;
    private readonly IMediator _mediator;

    protected BaseCudService(IRepository<TEntity> repository, IMediator mediator)
    {
        _repository = repository;
        _mediator = mediator;
    }

    public virtual async Task Insert(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await BeforeInsert(entity);

        await _repository.InsertAsync(entity);

        await AfterInsert(entity);

        await _mediator.EntityInserted(entity);
    }

    public virtual async Task Update(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await BeforeUpdate(entity);

        await _repository.UpdateAsync(entity);

        await AfterUpdate(entity);

        await _mediator.EntityUpdated(entity);
    }

    public virtual async Task Delete(TEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await BeforeDelete(entity);

        await _repository.DeleteAsync(entity);

        await AfterDelete(entity);

        await _mediator.EntityDeleted(entity);
    }

    protected virtual Task BeforeInsert(TEntity entity) => Task.CompletedTask;

    protected virtual Task AfterInsert(TEntity entity) => Task.CompletedTask;

    protected virtual Task BeforeUpdate(TEntity entity) => Task.CompletedTask;

    protected virtual Task AfterUpdate(TEntity entity) => Task.CompletedTask;

    protected virtual Task BeforeDelete(TEntity entity) => Task.CompletedTask;

    protected virtual Task AfterDelete(TEntity entity) => Task.CompletedTask;
}
```

Yeah, the methods in the base class are "virtual" and return by default just a normal Task. I know, that we should not declare virtual methods that way, however the default implementation gives us a huge benefit that we do not have to implement the methods in subclasses. We could play with the design of whether this must be an abstract class, whether it must implement some interface or whatever. However the idea is to lift the general control structure and then leave the subclasses define the custom logic.

```c#
public virtual async Task InsertBrand(Brand brand) => await Insert(brand);

public virtual async Task UpdateBrand(Brand brand) => await Update(brand);

public virtual async Task DeleteBrand(Brand brand) => await Delete(brand);
```

because I did not change the interfaces too much, the result is that we just inherit from the BaseCudService - yes we can name it also differently :) - and because we do not have any custom logic here, we just call the base method. Here we need to override the methods left exactly for custom logic

```c#
public virtual Task InsertCategory(Category category) => Insert(category);
public virtual Task UpdateCategory(Category category) => Update(category);
public virtual Task DeleteCategory(Category category) => Delete(category);

protected override Task BeforeInsert(Category entity)
{
   if (string.IsNullOrEmpty(entity.ParentCategoryId))
       entity.ParentCategoryId = "";
   return Task.CompletedTask;
}

protected override async Task AfterInsert(Category entity)
{
   await _cacheBase.RemoveByPrefix(CacheKey.CATEGORIES_PATTERN_KEY);
   await _cacheBase.RemoveByPrefix(CacheKey.PRODUCTS_PATTERN_KEY);
}

protected override async Task BeforeUpdate(Category entity)
{
   if (string.IsNullOrEmpty(entity.ParentCategoryId))
       entity.ParentCategoryId = "";

   var parentCategory = await GetCategoryById(entity.ParentCategoryId);
   while (parentCategory != null)
   {
       if (entity.Id == parentCategory.Id)
       {
           entity.ParentCategoryId = "";
           break;
       }
       parentCategory = await GetCategoryById(parentCategory.ParentCategoryId);
   }
}

protected override async Task AfterUpdate(Category entity)
{
   await _cacheBase.RemoveByPrefix(CacheKey.CATEGORIES_PATTERN_KEY);
   await _cacheBase.RemoveByPrefix(CacheKey.PRODUCTS_PATTERN_KEY);
}

protected override async Task BeforeDelete(Category entity)
{
   var subcategories = await GetAllCategoriesByParentCategoryId(entity.Id, true);
   foreach (var subcategory in subcategories)
   {
       subcategory.ParentCategoryId = "";
       await Update(subcategory);
   }
}

protected override async Task AfterDelete(Category entity)
{
   await _cacheBase.RemoveByPrefix(CacheKey.CATEGORIES_PATTERN_KEY);
}
```

## Abstracting document design

After fixing the incorrect behavior on the interface level from the previous lesson, I can still improve the code here

```c#
public class PdfReportGenerator : ReportGenerator
{
    private ReportPath _reportPath;

    public PdfReportGenerator(IOptions<ReportPath> reportPathOptions)
    {
        _reportPath = reportPathOptions.Value;
    }

    public void Generate(SingleTourReport report)
    {
        using var pdfWriter = new PdfWriter(_reportPath.Value);
        using var pdfDocument = new PdfDocument(pdfWriter);
        using Document document = new(pdfDocument);

        document.Add(new Paragraph("Tour Report").SetFontSize(20)
                                                 .SetBold()
                                                 .SetTextAlignment(TextAlignment.CENTER)
                                                 .SetMarginTop(20)
                                                 .SetMarginBottom(20));
        Table table = new Table(UnitValue.CreatePercentArray([1, 2])).UseAllAvailableWidth()
                                                                     .SetMarginBottom(20);

        AddTableRow(table, "ID:", report.TourId);
        ...

        document.Add(table);

        if (report.TourLogs.Count <= 0) return;

        document.Add(new Paragraph("Tour Logs").SetFontSize(18)
                                               .SetBold()
                                               .SetTextAlignment(TextAlignment.CENTER)
                                               .SetMarginTop(20)
                                               .SetMarginBottom(20));

        foreach (TourLogReportModel log in report.TourLogs)
        {
            Table tourLogTable = new Table(UnitValue.CreatePercentArray([1, 2])).UseAllAvailableWidth().SetMarginBottom(20);
            AddTableRow(tourLogTable, "Log Comment:", log.Comment);
            ...
        }
    }

    public void Generate(TourSummaryReport report)
    {
        using var pdfWriter = new PdfWriter(_reportPath.Value);
        using var pdfDocument = new PdfDocument(pdfWriter);
        using Document document = new(pdfDocument);
        document.Add(new Paragraph("Tour Summary Report").SetFontSize(20)
                                                         .SetBold()
                                                         .SetTextAlignment(TextAlignment.CENTER)
                                                         .SetMarginTop(20)
                                                         .SetMarginBottom(20));

        foreach (TourSummary summary in report.TourSummaries)
        {
            ....
        }
    }
}
```

Here and in all custom reports we see these 3 "using statements" and then work only with the last document. This can be for sure lifted up. And we also can work with sections - of course the requirements in our university project were not that complex, but in the other case I would refactory it differetnly. More convenient approach which eliminates the need
for repetitive initialization - update document structure and just allows us to leverage the fluent interfaces

```c#
public class ReportBuilder
{
    private readonly List<Action<Document>> _operations = new();

    public ReportBuilder WithTitle(string title)
    {
        _operations.Add(doc => doc.Add(new Paragraph(title)
            .SetFontSize(20)
            .SetBold()
            .SetTextAlignment(TextAlignment.CENTER)
            .SetMarginTop(20)
            .SetMarginBottom(20)));
        return this;
    }

    public ReportBuilder WithTable(Action<TableBuilder> configureTable)
    {
        var tableBuilder = new TableBuilder();
        configureTable(tableBuilder);
        _operations.Add(doc => doc.Add(tableBuilder.Build()));
        return this;
    }

    public ReportBuilder WithSection<T>(IEnumerable<T> items, Action<Document, T> addItem)
    {
        _operations.Add(doc =>
        {
            foreach (var item in items)
                addItem(doc, item);
        });
        return this;
    }

    public void GeneratePdf(string path)
    {
        using var pdfWriter = new PdfWriter(path);
        using var pdfDocument = new PdfDocument(pdfWriter);
        using var document = new Document(pdfDocument);

        foreach (var operation in _operations)
            operation(document);
    }
}

public class TableBuilder
{
    private readonly Table _table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 2 }))
        .UseAllAvailableWidth()
        .SetMarginBottom(20);

    public TableBuilder AddRow(string property, string value)
    {
        _table.AddCell(new Cell().Add(new Paragraph(property).SetBold()));
        _table.AddCell(new Cell().Add(new Paragraph(value)));
        return this;
    }

    public Table Build() => _table;
}
```

And this is the minimum we should do. After some adjustment now we do not need any initilization here - control is lifted and we just put our custom code like in the python "with"

```c#
public void Generate(SingleTourReport report)
{
    new ReportBuilder()
        .WithTitle("Tour Report")
        .WithTable(t => t
            .AddRow("ID:", report.TourId)
            .AddRow("Tour Name:", report.TourName)
            .AddRow("Description:", report.TourDescription)
            .AddRow("Start Location:", report.StartLocation)
            .AddRow("End Location:", report.EndLocation))
        .WithSection(report.TourLogs, (doc, log) =>
        {
            var table = new TableBuilder()
                .AddRow("Log Comment:", log.Comment)
                .AddRow("Difficulty:", log.Difficulty)
                .AddRow("Total Distance:", log.Distance)
                .Build();
            doc.Add(table);
        })
        .GeneratePdf(_reportPath.Value);
}
```