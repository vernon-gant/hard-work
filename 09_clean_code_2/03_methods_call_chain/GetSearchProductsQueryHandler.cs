// BEFORE

https://github.com/grandnode/grandnode2/blob/main/src/Business/Grand.Business.Catalog/Queries/Handlers/GetSearchProductsQueryHandler.cs

// AFTER

public interface IQueryFilter
{
    bool ShouldApply(GetSearchProductsQuery request);

    IQueryable<Product> Apply(GetSearchProductsQuery request, IQueryable<Product> query);
}

public class CategoryFilter : IQueryFilter
{
    public bool ShouldApply(GetSearchProductsQuery request) => request.CategoryIds != null && request.CategoryIds.Any();

    public IQueryable<Product> Apply(GetSearchProductsQuery request, IQueryable<Product> query)
    {
        if (request.FeaturedProducts.HasValue)
            return query.Where(x => x.ProductCategories.Any(c => request.CategoryIds.Contains(c.CategoryId) && c.IsFeaturedProduct == request.FeaturedProducts.Value));

        return query.Where(x => x.ProductCategories.Any(c => request.CategoryIds.Contains(c.CategoryId)));
    }
}

public interface IQueryOrderingStrategy
{
    bool ShouldApply(GetSearchProductsQuery request);

    IQueryable<Product> Apply(GetSearchProductsQuery request, IQueryable<Product> query);
}

public class OrderByNameAscStrategy : IQueryOrderingStrategy
{
    private readonly CatalogSettings _catalogSettings;

    public OrderByNameAscStrategy(CatalogSettings catalogSettings) => _catalogSettings = catalogSettings;

    public bool ShouldApply(GetSearchProductsQuery request) => request.OrderBy == ProductSortingEnum.NameAsc;

    public IQueryable<Product> Apply(GetSearchProductsQuery request, IQueryable<Product> query) =>
        _catalogSettings.SortingByAvailability ? query.OrderBy(p => p.LowStock).ThenBy(p => p.Name) : query.OrderBy(p => p.Name);
}

public class SearchPipeline
{
    private readonly IEnumerable<ISearchPipelineStep> _steps;

    public SearchPipeline(IEnumerable<ISearchPipelineStep> steps)
    {
        _steps = steps;
    }

    public async Task<SearchContext> ExecuteAsync(SearchContext context, CancellationToken cancellationToken)
    {
        foreach (var step in _steps.Where(s => s.ShouldExecute(context)))
            context = await step.ExecuteAsync(context, cancellationToken);

        return context;
    }
}

public interface ISearchPipelineStep
{
    bool ShouldExecute(SearchContext context);

    Task<SearchContext> ExecuteAsync(SearchContext context, CancellationToken cancellationToken);
}

public class CleanUpStep : ISearchPipelineStep
{
    public bool ShouldExecute(SearchContext context) => context.Request.CategoryIds?.Any() == true;

    public Task<SearchContext> ExecuteAsync(SearchContext context, CancellationToken cancellationToken)
    {
        context.Request.CategoryIds.Remove("");

        return Task.FromResult(context);
    }
}

public class BasicFilterStep : ISearchPipelineStep
{
    private readonly IEnumerable<IQueryFilter> _queryFilters;

    public BasicFilterStep(IEnumerable<IQueryFilter> queryFilters)
    {
        _queryFilters = queryFilters;
    }

    public bool ShouldExecute(SearchContext context) => _queryFilters.Any();

    public Task<SearchContext> ExecuteAsync(SearchContext context, CancellationToken cancellationToken)
    {
        var query = context.FilterableQuery;

        foreach (var filter in _queryFilters.Where(f => f.ShouldApply(context.Request)))
        {
            query = filter.Apply(context.Request, query);
        }

        return Task.FromResult(context with { FilterableQuery = query });
    }
}

public class GetSearchProductsQueryHandler : IRequestHandler<GetSearchProductsQuery, (IPagedList<Product> products, IList<string> filterableSpecificationAttributeOptionIds)>
{
    private readonly IRepository<Product> _productRepository;
    private readonly SearchPipeline _searchPipeline;

    public GetSearchProductsQueryHandler(IRepository<Product> productRepository, SearchPipeline searchPipeline)
    {
        _productRepository = productRepository;
        _searchPipeline = searchPipeline;
    }

    public async Task<(IPagedList<Product> products, IList<string> filterableSpecificationAttributeOptionIds)> Handle(GetSearchProductsQuery request, CancellationToken cancellationToken)
    {
        var specificationQuery = _productRepository.Table.AsQueryable();
        var context = new SearchContext(request, specificationQuery);

        var results = await _searchPipeline.ExecuteAsync(context, cancellationToken);

        return (results.Results, results.FilterableSpecificationAttributeOptionIds);
    }
}