// BEFORE

public class ProductCollectionService : IProductCollectionService
{
    public ProductCollectionService(ICacheBase cacheBase, IRepository<Product> productRepository, IContextAccessor contextAccessor, IMediator mediator, AccessControlConfig accessControlConfig)
    {
        _cacheBase = cacheBase;
        _productRepository = productRepository;
        _contextAccessor = contextAccessor;
        _mediator = mediator;
        _accessControlConfig = accessControlConfig;
    }

    public virtual async Task<IPagedList<ProductsCollection>> GetProductCollectionsByCollectionId(string collectionId, string storeId, int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false)
    {
        var key = string.Format(CacheKey.PRODUCTCOLLECTIONS_ALLBYCOLLECTIONID_KEY, showHidden, collectionId, pageIndex, pageSize, _contextAccessor.WorkContext.CurrentCustomer.Id, storeId);
        return await _cacheBase.GetAsync(key, () =>
        {
            var query = _productRepository.Table.Where(x =>
                x.ProductCollections.Any(y => y.CollectionId == collectionId));

            if (!showHidden && (!_accessControlConfig.IgnoreAcl || !_accessControlConfig.IgnoreStoreLimitations))
            {
                if (!_accessControlConfig.IgnoreAcl)
                {
                    //ACL (access control list)
                    var allowedCustomerGroupsIds = _contextAccessor.WorkContext.CurrentCustomer.GetCustomerGroupIds();
                    query = from p in query
                        where !p.LimitedToGroups || allowedCustomerGroupsIds.Any(x => p.CustomerGroups.Contains(x))
                        select p;
                }

                if (!_accessControlConfig.IgnoreStoreLimitations && !string.IsNullOrEmpty(storeId))
                    //Store acl
                    query = from p in query
                    where !p.LimitedToStores || p.Stores.Contains(storeId)
                    select p;
            }

            var queryProductCollection = from prod in query
                from pm in prod.ProductCollections
                select new ProductsCollection {
                    Id = pm.Id,
                    ProductId = prod.Id,
                    DisplayOrder = pm.DisplayOrder,
                    IsFeaturedProduct = pm.IsFeaturedProduct,
                    CollectionId = pm.CollectionId
                };

            queryProductCollection = from pm in queryProductCollection
                where pm.CollectionId == collectionId
                orderby pm.DisplayOrder
                select pm;

            return Task.FromResult(new PagedList<ProductsCollection>(queryProductCollection, pageIndex, pageSize));
        });
    }
}

// AFTER

public interface Query<T>
{
    IQueryable<T> Queryable { get; }

    T First();

    Task<T> FirstAsync();

    T? FirstOrDefault();

    Task<T?> FirstOrDefaultAsync();

    C ToCollection<C>(Func<IQueryable<T>, C> collectionSelector) where C : ICollection<T>;

    Task<C> ToCollectionAsync<C>(Func<IQueryable<T>, C> collectionSelector) where C : ICollection<T>;

    IPagedList<T> ToPagedList(int pageIndex, int pageSize);
}

public class NormalQuery<T> : Query<T>
{
    private readonly IQueryable<T> _queryable;

    public NormalQuery(IQueryable<T> queryable)
    {
        _queryable = queryable;
    }

    public IQueryable<T> Queryable => _queryable;

    public T First()
    {
        return _queryable.First();
    }

    public Task<T> FirstAsync()
    {
        return Task.FromResult(_queryable.First());
    }

    public T FirstOrDefault()
    {
        return _queryable.FirstOrDefault();
    }

    public Task<T> FirstOrDefaultAsync()
    {
        return Task.FromResult(_queryable.FirstOrDefault());
    }

    public C ToCollection<C>(Func<IQueryable<T>, C> collectionSelector) where C : ICollection<T>
    {
        return collectionSelector(_queryable);
    }

    public Task<C> ToCollectionAsync<C>(Func<IQueryable<T>, C> collectionSelector) where C : ICollection<T>
    {
        return Task.FromResult(collectionSelector(_queryable));
    }

    public IPagedList<T> ToPagedList(int pageIndex, int pageSize)
    {
        return new PagedList<T>(_queryable, pageIndex, pageSize);
    }
}

public class CachedQuery<T> : Query<T>
{
    private readonly string _cacheKey;
    private readonly IQueryable<T> _query;
    public readonly ICacheBase _cache;

    public CachedQuery(string cacheKey, IQueryable<T> query, ICacheBase cacheBase)
    {
        _cacheKey = cacheKey;
        _query = query;
        _cache = cacheBase;
    }

    public IQueryable<T> Queryable => _query;

    public T First()
    {
        return _cache.Get(_cacheKey, () => _query.First());
    }

    public async Task<T> FirstAsync()
    {
        return await _cache.GetAsync(_cacheKey, () => _query.FirstAsync());
    }

    public T FirstOrDefault()
    {
        return _cache.Get(_cacheKey, () => _query.FirstOrDefault());
    }

    public async Task<T> FirstOrDefaultAsync()
    {
        return await _cache.GetAsync(_cacheKey, () => _query.FirstOrDefaultAsync());
    }

    public C ToCollection<C>(Func<IQueryable<T>, C> collectionSelector) where C : ICollection<T>
    {
        return _cache.Get(_cacheKey, () => collectionSelector(_query));
    }

    public IPagedList<T> ToPagedList(int pageIndex, int pageSize)
    {
        var key = $"{_cacheKey}_PagedList_{pageIndex}_{pageSize}";
        return _cache.Get(key, () => new PagedList<T>(_query, pageIndex, pageSize));
    }

    public async Task<C> ToCollectionAsync<C>(Func<IQueryable<T>, C> collectionSelector) where C : ICollection<T>
    {
        return await _cache.GetAsync(_cacheKey, () => Task.FromResult(collectionSelector(_query)));
    }
}

public class ProductCollectionService : IProductCollectionService
{
    private readonly IRepository<Product> _productRepository;
    private readonly IMediator _mediator;
    private readonly AccessControlConfig _accessControlConfig;

    public ProductCollectionService(IRepository<Product> productRepository, IMediator mediator, AccessControlConfig accessControlConfig)
    {
        _productRepository = productRepository;
        _mediator = mediator;
        _accessControlConfig = accessControlConfig;
    }

    public virtual Query<ProductsCollection> GetByCollectionId(string collectionId, string storeId, string[] allowedCustomerGroupsIds, bool showHidden)
    {
        var query = _productRepository.Table.Where(x => x.ProductCollections.Any(y => y.CollectionId == collectionId));

        if (!showHidden && !_accessControlConfig.IgnoreAcl)
        {
            query = from p in query where !p.LimitedToGroups || allowedCustomerGroupsIds.Any(x => p.CustomerGroups.Contains(x)) select p;
        }

        if (!showHidden && !_accessControlConfig.IgnoreStoreLimitations && !string.IsNullOrEmpty(storeId))
            query = from p in query where !p.LimitedToStores || p.Stores.Contains(storeId) select p;

        var queryProductCollection = from prod in query
            from pm in prod.ProductCollections
            select new ProductsCollection {
                Id = pm.Id,
                ProductId = prod.Id,
                DisplayOrder = pm.DisplayOrder,
                IsFeaturedProduct = pm.IsFeaturedProduct,
                CollectionId = pm.CollectionId
            };

        return new NormalQuery<ProductsCollection>(from pm in queryProductCollection where pm.CollectionId == collectionId orderby pm.DisplayOrder select pm);
    }
}

public class CachedProductCollectionService : IProductCollectionService
{
    private readonly ProductCollectionService _productCollectionService;
    private readonly ICacheBase _cacheBase;
    private readonly IContextAccessor _contextAccessor;

    public CachedProductCollectionService(ProductCollectionService productCollectionService, ICacheBase cacheBase, IContextAccessor contextAccessor)
    {
        _productCollectionService = productCollectionService;
        _cacheBase = cacheBase;
        _contextAccessor = contextAccessor;
    }

    public Query<ProductsCollection> GetByCollectionId(string collectionId, string storeId, string[] allowedCustomerGroupsIds, bool showHidden)
    {
        var normalQuery = _productCollectionService.GetByCollectionId(collectionId, storeId, allowedCustomerGroupsIds, showHidden);
        var key = string.Format(CacheKey.PRODUCTCOLLECTIONS_ALLBYCOLLECTIONID_KEY, false, collectionId, _contextAccessor.WorkContext.CurrentCustomer.Id, storeId);
        return new CachedQuery<ProductsCollection>(key, normalQuery.Queryable, _cacheBase);
    }

    public async Task InsertProductCollection(ProductCollection productCollection, string productId)
    {
        await _productCollectionService.InsertProductCollection(productCollection, productId);

        await _cacheBase.RemoveByPrefix(CacheKey.PRODUCTCOLLECTIONS_PATTERN_KEY);
        await _cacheBase.RemoveByPrefix(string.Format(CacheKey.PRODUCTS_BY_ID_KEY, productId));
    }

    public async Task UpdateProductCollection(ProductCollection productCollection, string productId)
    {
        await _productCollectionService.UpdateProductCollection(productCollection, productId);

        await _cacheBase.RemoveByPrefix(CacheKey.PRODUCTCOLLECTIONS_PATTERN_KEY);
        await _cacheBase.RemoveByPrefix(string.Format(CacheKey.PRODUCTS_BY_ID_KEY, productId));
    }

    public async Task DeleteProductCollection(ProductCollection productCollection, string productId)
    {
        await _productCollectionService.DeleteProductCollection(productCollection, productId);

        await _cacheBase.RemoveByPrefix(CacheKey.PRODUCTCOLLECTIONS_PATTERN_KEY);
        await _cacheBase.RemoveByPrefix(string.Format(CacheKey.PRODUCTS_BY_ID_KEY, productId));
    }
}


public virtual async Task<(IEnumerable<CollectionModel.CollectionProductModel> collectionProductModels, int totalCount)> PrepareCollectionProductModel(string collectionId, string storeId, int pageIndex, int pageSize)
{
    var allowedCustomerGroupIds = _contextAccessor.WorkContext.CurrentCustomer.GetCustomerGroupIds();
    var query = _productCollectionService.GetByCollectionId(collectionId, storeId, allowedCustomerGroupIds, true);
    var pagedResult = query.ToPagedList(pageIndex, pageSize);

    var items = new List<CollectionModel.CollectionProductModel>();

    foreach (var x in pagedResult)
        items.Add(new CollectionModel.CollectionProductModel {
            Id = x.Id,
            CollectionId = x.CollectionId,
            ProductId = x.ProductId,
            ProductName = (await _productService.GetProductById(x.ProductId)).Name,
            IsFeaturedProduct = x.IsFeaturedProduct,
            DisplayOrder = x.DisplayOrder
        });

    return (items, pagedResult.TotalCount);
}