# Refactoring

The idea that refactoring is not just creating a "trash" class which contains all the logic transferred from another class was obvious, but as usual hidden. I have to confess, I was thinking about refactoring also this way. Interesting point is that our senior devs told me that firstly they just "throw what they have in the head into the VS code" and then "refactor it". This is how perfect programming looks like... And in this case refactoring means exactly what was described in the note.

For this task I decided to refer to the note on "Extracting good abstractions for interfaces". I have to admit, this was hard. But I managed to handle one refactoring, actually continuing one of my previous improvements. Then I continued investigating how I could use the cool composable technique even further... 

## Composite querying

Ability to form a good composite is a sign of a good interface, better I would not even say :) This counterintuitive idea is actually mind blowing. Althoug we can and we even should represent the "object tree structures" using this pattern, this was a bit misleading that all guides on the internet refer to this use case as the only one and do not mention anything about recursion, although trees and recursion are very tightly coupled. Anyway, a good abstraction is really composable, I do not know why but it is! I even remembered that during my import module implementation at work I implemented the `IParser` with one method parse using another interface `IParserComponent` in the `ComponentParser` implementation. Although this was nothing but a composite parser where we could put the `EntityParser` and `CustomFieldsParser`. Amazing...

I decided to continue working on querying. Filters are perfect candidates for a good composite - we can easily combine them! We could even have one implementation for composite and one for the DSL based. Finally to bring this all together we need some sort of configuration. If we bring it all together we get

```c#
public interface IQueryFilter<in TRequest, TEntity> where TEntity : BaseEntity
{
    IQueryable<TEntity> Apply(IQueryable<TEntity> query, TRequest request);

    bool ShouldApply(TRequest request);
}

public abstract class QueryFilterConfiguration<TRequest, TEntity> where TEntity : BaseEntity
{
    private readonly IServiceProvider _sp;
    private readonly List<IQueryFilter<TRequest, TEntity>> _filters = new();

    protected QueryFilterConfiguration(IServiceProvider serviceProvider) => _sp = serviceProvider;

    protected QueryFilterConfiguration<TRequest, TEntity> With(IQueryFilter<TRequest, TEntity> filter)
    {
        _filters.Add(filter);
        return this;
    }

    protected QueryFilterConfiguration<TRequest, TEntity> With<TFilter>()
        where TFilter : class, IQueryFilter<TRequest, TEntity>
    {
        _filters.Add(_sp.GetRequiredService<TFilter>());
        return this;
    }

    protected QueryFilterConfiguration<TRequest, TEntity> WithComposite(Action<QueryFilterConfiguration<TRequest, TEntity>> inner)
    {
        ...
    }

    protected abstract void Configure();

    public IQueryFilter<TRequest, TEntity> Build()
    {
        ...
    }
}

public abstract class InPlaceQueryFilter<TRequest, TEntity> : IQueryFilter<TRequest, TEntity> where TEntity : BaseEntity
{
    private readonly List<(Func<TRequest, bool> condition, Func<TRequest, Expression<Func<TEntity, bool>>> predicate)> _filters = new();

    protected InPlaceQueryFilter()
    {
        RegisterFilters();
    }

    public IQueryable<TEntity> Apply(IQueryable<TEntity> query, TRequest request)
    {
        foreach (var (condition, predicate) in _filters)
        {
            if (condition(request))
                query = query.Where(predicate(request));
        }

        return query;
    }

    public bool ShouldApply(TRequest request) => _filters.Any(f => f.condition(request));

    protected abstract void RegisterFilters();

    // Simple filter registration
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

public class CompositeQueryFilter<TRequest, TEntity> : IQueryFilter<TRequest, TEntity> where TEntity : BaseEntity
{
    private readonly List<IQueryFilter<TRequest, TEntity>> _filters = new();

    public void Add(IQueryFilter<TRequest, TEntity> filter)
    {
        _filters.Add(filter);
    }

    public bool ShouldApply(TRequest request) => true;

    public IQueryable<TEntity> Apply(IQueryable<TEntity> query, TRequest request) =>
        _filters.Where(filter => filter.ShouldApply(request)).Aggregate(query, (current, filter) => filter.Apply(current, request));
}
```

This is actually everyting what we need, it took enormous amount of time to come up with it for me with my current level, but I like it now. Because if we want to rewrite this thing now

```c#
public virtual async Task<IPagedList<Brand>> GetAllBrands(string brandName = "", string storeId = "", int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
{
    var query = from m in _brandRepository.Table select m;

    if (!showHidden)
        query = query.Where(m => m.Published);

    if (!string.IsNullOrWhiteSpace(brandName))
        query = query.Where(m => m.Name != null && m.Name.ToLower().Contains(brandName.ToLower()));

    if (!_accessControlConfig.IgnoreAcl || (!string.IsNullOrEmpty(storeId) && !_accessControlConfig.IgnoreStoreLimitations))
    {
        if (!showHidden && !_accessControlConfig.IgnoreAcl)
        {
            //Limited to customer groups rules
            var allowedCustomerGroupsIds = _contextAccessor.WorkContext.CurrentCustomer.GetCustomerGroupIds();
            query = from p in query
                    where !p.LimitedToGroups || allowedCustomerGroupsIds.Any(x => p.CustomerGroups.Contains(x))
                    select p;
        }

        if (!string.IsNullOrEmpty(storeId) && !_accessControlConfig.IgnoreStoreLimitations)
            //Limited to stores rules
            query = from p in query
                    where !p.LimitedToStores || p.Stores.Contains(storeId)
                    select p;
    }

    query = query.OrderBy(m => m.DisplayOrder).ThenBy(m => m.Name);
    return await PagedList<Brand>.Create(query, pageIndex, pageSize);
}
```

nothing stops us from creating

```c#
public interface IHasShowHidden { bool ShowHidden { get; } }
public sealed record GetMenuCategoriesQuery : IRequest<IQueryable<Category>>, IHasShowHidden;

public sealed class PublishedAndMenuFilter : IQueryFilter<GetMenuCategoriesQuery, Category>
{
    public bool ShouldApply(GetMenuCategoriesQuery _) => true;

    public IQueryable<Category> Apply(IQueryable<Category> q, GetMenuCategoriesQuery _) => q.Where(c => c.Published && c.IncludeInMenu);
}

// Will be reusable across other components. Maybe I could abstract away it even futher with some proxy or decorator, this will be the next step.
public sealed class CustomerGroupAclFilter<TRequest, TEntity> : IQueryFilter<TRequest, TEntity>
    where TEntity : BaseEntity, IGroupLinkEntity
    where TRequest : IHasShowHidden
{
    private readonly IContextAccessor _context;
    private readonly AccessControlConfig _cfg;

    public CustomerGroupAclFilter(IContextAccessor contextAccessor, IOptions<AccessControlConfig> options)
    { _context = contextAccessor; _cfg = options.Value; }

    public bool ShouldApply(TRequest r) => !_cfg.IgnoreAcl && !r.ShowHidden;

    public IQueryable<TEntity> Apply(IQueryable<TEntity> q, TRequest _)
    {
        var groups = _context.WorkContext.CurrentCustomer.GetCustomerGroupIds();
        return q.Where(p => !p.LimitedToGroups || groups.Any(id => p.CustomerGroups.Contains(id)));
    }
}

public sealed class StoreLimitationAclFilter<TRequest, TEntity> : IQueryFilter<TRequest, TEntity>
    where TEntity : BaseEntity, IStoreLinkEntity
{
    private readonly IContextAccessor _context;
    private readonly AccessControlConfig _cfg;

    public StoreLimitationAclFilter(IContextAccessor contextAccessor, IOptions<AccessControlConfig> options)
    { _context = contextAccessor; _cfg = options.Value; }

    public bool ShouldApply(TRequest _)
    {
        var storeId = _context.WorkContext.CurrentStore?.Id;
        return !_cfg.IgnoreStoreLimitations && !string.IsNullOrEmpty(storeId);
    }

    public IQueryable<TEntity> Apply(IQueryable<TEntity> q, TRequest _)
    {
        var storeId = _context.WorkContext.CurrentStore!.Id;
        return q.Where(p => !p.LimitedToStores || p.Stores.Contains(storeId));
    }
}
```

and we get out perfect

```c#
public sealed class GetMenuCategoriesFilterConfiguration : QueryFilterConfiguration<GetMenuCategoriesQuery, Category>
{
    public GetMenuCategoriesFilterConfiguration(IServiceProvider sp) : base(sp) { }

    protected override void Configure()
    {
        With<PublishedAndMenuFilter>();
        With<CustomerGroupAclFilter<GetMenuCategoriesQuery, Category>>();
        With<StoreLimitationAclFilter<GetMenuCategoriesQuery, Category>>();
    }
}
```

which is a strongly typed filter because Category must be in this case `IStoreLinkEntity` and `IGroupLinkEntity`.
Then we just implement the ordering and that's it

```c#
public sealed class GetMenuCategoriesQueryHandler : BaseQueryHandler<GetMenuCategoriesQuery, Category>
{
    public GetMenuCategoriesQueryHandler(IRepository<Category> repo, GetMenuCategoriesFilterConfiguration cfg) : base(repo, cfg) { }

    protected override IQueryable<Category> ApplyOrdering(GetMenuCategoriesQuery _, IQueryable<Category> query)
        => query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name);
}
```

Boudaries in the case of rewriting how we approach querying are out interfaces `IQueryFilter`, `BaseQueryHandler` and `QueryFilterConfiguration`. We operate within these abstractions and can create new queries, access control filterings and filters in general much easier. Modification of existing filter configuration is also way easier now. This code does not affect other code parts directly, but allows us to rewrite all existing queries aka `Mediatr` request implementations much efficiently and also rewrite existing service methods which are also nothing but queries. Thus we end up having really "commands" and "queries" in the whole MVC landscape :) A simple integration test for retrieving a category menu could look like this

```c#
[Fact]
public async Task UseCase_Get_MenuCategories_And_Enforce_Published_Menu_ACL_Store_And_Ordering()
{
    // Arrange
    var store1 = new Store { Id = "store-1" };
    var customer = new Customer(); customer.SetGroups(new[] { "REGULAR" });

    var data = new[]
    {
        new Category { Name="Z AllOk", Published=true, IncludeInMenu=true, DisplayOrder=2 },
        new Category { Name="A First", Published=true, IncludeInMenu=true, DisplayOrder=1 },

        new Category { Name="NotPublished", Published=false, IncludeInMenu=true, DisplayOrder=0 },
        new Category { Name="NotInMenu",   Published=true,  IncludeInMenu=false, DisplayOrder=0 },

        new Category { Name="GroupLimited_NoAccess", Published=true, IncludeInMenu=true,
                           LimitedToGroups=true, CustomerGroups=new List<string>{"VIP"}, DisplayOrder=3 },

        new Category { Name="StoreLimited_OtherStore", Published=true, IncludeInMenu=true,
                           LimitedToStores=true, Stores=new List<string>{"store-2"}, DisplayOrder=4 },

        new Category { Name="StoreLimited_MatchingStore", Published=true, IncludeInMenu=true,
                           LimitedToStores=true, Stores=new List<string>{store1.Id}, DisplayOrder=5 },
    };

    var sp1 = BuildProvider(data, new TestWorkContext(customer, store1), new AccessControlConfig { IgnoreAcl = false, IgnoreStoreLimitations = false });

    // Act
    var handler1 = sp1.GetRequiredService<GetMenuCategoriesQueryHandler>();
    var q1 = await handler1.Handle(new GetMenuCategoriesQuery(ShowHidden: false), CancellationToken.None);
    var list1 = q1.ToList();

    // Assert
    var names1 = list1.Select(c => c.Name).ToArray();
    Assert.Equal(new[] { "A First", "Z AllOk", "StoreLimited_MatchingStore" }, names1);
}
```

## Composite model enrichment

When you find an already existinb abstraction, it is quite easy to judge if it is composable or not - I totally agree with it. However *in order to find this abstraction* I spent some time even looking at [this masterpiece](https://github.com/vernon-gant/grandnode2/blob/main/src/Web/Grand.Web/Features/Handlers/Products/GetProductDetailsPageHandler.cs). Enrichment is a good word for that. Because single enrichments will work on concrete model properties or very closely related properties and we can compose multiple enrichments, I thought it could be a good option for refactoring. I also keep in my head the sentence from the note that "we can not really rewrite everything from scratch the project." This was my problem when I started looking for refactorings first time when approaching some design changes earlier - I tried to rewrite everything. Sometimes it is impossible and probably not even sometimes but in most case. So a viable refactoring could look like

```c#
public interface IModelEnricher<TModel, in TContext>
{
    Task EnrichAsync(TModel model, TContext context, CancellationToken cancellationToken);

    bool ShouldEnrich(TContext context) => true;
}

public class CompositeModelEnricher<TModel, TContext> : IModelEnricher<TModel, TContext>
{
    private readonly IEnumerable<IModelEnricher<TModel, TContext>> _enrichers;

    public CompositeModelEnricher(IEnumerable<IModelEnricher<TModel, TContext>> enrichers)
    {
        _enrichers = enrichers ?? Enumerable.Empty<IModelEnricher<TModel, TContext>>();
    }

    public async Task EnrichAsync(TModel model, TContext context, CancellationToken cancellationToken)
    {
        foreach (var enricher in _enrichers.Where(e => e.ShouldEnrich(context)))
        {
            await enricher.EnrichAsync(model, context, cancellationToken);
        }
    }

    public bool ShouldEnrich(TContext context) => true;
}

public class BaseModelHandler<TRequest, TModel> : IRequestHandler<TRequest, TModel> where TRequest : IRequest<TModel>
{
    private readonly IMapper _mapper;
    private readonly IModelEnricher<TModel, TRequest> _compositeEnricher;

    protected BaseModelHandler(IMapper mapper, IEnumerable<IModelEnricher<TModel, TContext>> enrichers)
    {
        _mapper = mapper;
        _compositeEnricher = new CompositeModelEnricher<TModel,TRequest>(enrichers);
    }

    public async Task<TModel> Handle(TRequest request, CancellationToken cancellationToken)
    {
        var model = _mapper.Map<TModel>(request);

        await _compositeEnricher.EnrichAsync(model, context,cancellationToken);

        return model;
    }
}

public class BrandModelEnricher : IModelEnricher<ProductDetailsModel, ProductDetailsBuildingContext>
{
    public bool ShouldEnrich(ProductDetailsBuildingContext context) => !string.IsNullOrEmpty(context.Product.BrandId);

    public async Task EnrichAsync(ProductDetailsModel model, ProductDetailsBuildingContext context, CancellationToken cancellationToken)
    {
        ...
    }
}
```

Of course I will not rewrite all billion enrichers, but in the context of really big models used in this application our code affects a lot of handlers updating properties on models like Category, Brand, Blog and other fat models. The borders of the new code are actually defined by the interface `IModelEnricher` and by the `BaseModelHandler`. Automapper here is a viable solution. That's it. The new code does not affect the system globally, only part which is responsible for filling the models with data when showing to the user. Generics allow us not to bother a lot about the configuration, because everything will be resolved from the DI container and then converted into a CompositeEnricher. How convenient is this "composite"... Thank you very much!

## Composite formatters

What I supposed first, but remembered last was the price calculation. I assumed that price calculation is a multistep process with mutltiple rules. And again a good abstraction is always represented using a composite! [Here](https://github.com/vernon-gant/grandnode2/blob/main/src/Business/Grand.Business.Catalog/Services/Prices/PricingService.cs) we have again a pricing rule which can be either applied or not, based on the condition. This also shapes borders of our refactoring - we are not refactoring or redesigning cross module code. This is just one small aspect - price calculation. This does not affect the system globally at all - only locally in regard to price calculation design. But I think it still makes sense to work on it as long as I am practicing with composite. Here we use an immutable PriceInput and mutable price state which is mutated in the single rules.

```c#
public interface IPricingRule
{
    bool CanApply(PriceInput input);

    Task ApplyAsync(PriceInput input, PriceState state);
}

public readonly record struct PriceInput(
    Product Product,
    Customer Customer,
    Store Store,
    Currency Currency,
    int Quantity,
    bool IncludeDiscounts,
    double AdditionalCharge,
    DateTime? RentalStartDate,
    DateTime? RentalEndDate);

public sealed class PriceState
{
    public double Price { get; set; }

    public double DiscountAmount { get; set; }

    public List<ApplyDiscount> AppliedDiscounts { get; } = new();

    public TierPrice? PreferredTierPrice { get; set; }
}

public sealed class CompositePricingRule : IPricingRule
{
    private readonly IEnumerable<IPricingRule> _children;

    public CompositePricingRule(IEnumerable<IPricingRule> children) => _children = children;

    public bool CanApply(in PriceInput input) => _children.Any(r => r.CanApply(input));

    public async Task ApplyAsync(PriceInput input, PriceState state)
    {
        foreach (var rule in _children)
        {
            if (rule.CanApply(input))
                await rule.ApplyAsync(input, state);
        }
    }
}

public sealed class ReservationRule : IPricingRule
{
    public bool CanApply(in PriceInput input)
        => input.Product.ProductTypeId == ProductType.Reservation
           && input.RentalStartDate.HasValue && input.RentalEndDate.HasValue;

    public Task ApplyAsync(PriceInput input, PriceState state)
    {
        var days = input.Product.IncBothDate
            ? (input.RentalEndDate!.Value - input.RentalStartDate!.Value).TotalDays + 1
            : (input.RentalEndDate!.Value - input.RentalStartDate!.Value).TotalDays;

        state.Price *= days;
        return Task.CompletedTask;
    }
}

public sealed class TierPricingRule : IPricingRule
{
    public bool CanApply(in PriceInput input) => input.Quantity > 0;

    public Task ApplyAsync(PriceInput input, PriceState state)
    {
        ...
    }
}
```

And we just continue to implement all the rules iteratively and then end up with this code intead of 100 line monster.

```c#
var input = new PriceInput(...);
var state = new PriceState();

if (_pricing.CanApply(input))
    await _pricing.ApplyAsync(input, state);

return state;
```

The borders are clearly defined and it is our `IPricingRule` interface. Only its implementations shape this small domain. This allows us to concentrate on concrete use cases in the tests as well

```c#
[Fact]
public async Task UseCase_PricingComposite_Combines_All_Rules_To_CorrectFinalPrice()
{
    // Arrange
    var product = new Product
    {
        Price = 120.0,
        ProductTypeId = ProductType.Reservation,
        IncBothDate = true,
        ProductPrices = new List<ProductPriceRecord>
        {
            new ProductPriceRecord { CurrencyCode = "USD", Price = 120.0 }
        },
        TierPrices = new List<TierPrice>
        {
            new TierPrice { Quantity = 1, Price = 120 },
            new TierPrice { Quantity = 3, Price = 100 }
        }
    };
    var customer = new Customer();
    customer.SetGroups(new[] { "REG" });
    var store = new Store { Id = "store-X" };
    var currency = new Currency { CurrencyCode = "USD" };
    DateTime startDate = new DateTime(2025, 11, 1), endDate = new DateTime(2025, 11, 2);
    var input = new PriceInput(product, customer, store, currency, 3, true, 5.0, startDate, endDate);
    var composite = new CompositePricingRule(new IPricingRule[]
    {
        new BasePriceRule(new TestCurrencySvc()),
        new TierPricingRule(),
        new AdditionalChargeRule(),
        new ReservationRule(),
        new DiscountRule(new TestDiscountSvc()),
        new RoundingRule(new ShoppingCartSettings { RoundPrices = true })
    });
    var state = new PriceState();

    // Act
    await composite.ApplyAsync(input, state);

    // Assert
    // base 120 → tier 100 → +5 → 105 → 2 days → 105 * 2 = 210 → discount 10 = 200 → rounding = 200
    Assert.Equal(200.0, state.Price);
    Assert.Equal(10.0, state.DiscountAmount);
    Assert.Single(state.AppliedDiscounts);
}
```