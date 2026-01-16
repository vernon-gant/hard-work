# Intro

Working with legacy code is always a challenge we all know that. However this challenge
also helps us to grow as a developer. For practicing on cleaning up legacy code which smells I decided to take the grandnode2 project which I inspected during my bachelor thesis. The codebase is big enough, however it took much more time that I expected to find specific flawed parts of code which I wanted to fix. On the other hand it reminded
me again how important it is to think in terms of abstractions on the third level and put these abstractions into code and not think immediately on the second level of reasoning aka on the code level. When we omit logical thinking many of following bad design choices may occur.

## Basic implementation level refactoring

Just fo the warmup I decided to take the validation of domain constraints, which of course are written with if and elses. This thing really drives me mad because we have such a powerful DSL for expressing domain constraints declaratively in C# using the FluentValidation library. Before approaching my practical thesis part I was really shocked how many FluenValidation validators exist in this project which just put all logic in the Custom() or CustomAsync() methods with ifs and elses instead of utilizing the DSL capabilities. We can easily standardize the way we think about domain constraints by brining them into specific format and then just literally writing this form using the DSL. Like a normal english sentence and the written form will be easily read from the code by any domain expert or product owner. With nested ifs and elses nested in a big switch block this is not possible.

We can even go one step further and add here return values, say in the case of [api authentication](01_implementation_level/ApiAuthentication.cs) we want to return a specific user based on the defined rules. FluentValidation allows us to do it. Say if the auth header is not set at all we return null. Altough I would change this as well, the primary focus was not on method return types design here. After all manipulations we ended up with this instead of ifs and elses

```c#
        RuleFor(WholeContext).Cascade(CascadeMode.Stop)
            .Must(HaveNonNullHttpContext).WithState(NullCustomer)
            .Must(HaveSetAuthorizationHeader).WithState(NullCustomer)
            .OverridePropertyName(WholeContextPropertyName)
            .DependentRules(() =>
            {
                RuleFor(WholeContext).Must(ReturnCustomer).WhenAsync(IsApiFrontAuthenticated).WithState(ApiCustomer).OverridePropertyName(WholeContextPropertyName);

                RuleFor(WholeContext).Cascade(CascadeMode.Stop)
                    .MustAsync(AuthenticateUsingHttpContextSuccessfully).WithState(NullCustomer)
                    .MustAsync(HaveActiveNotDeletedAndRegisteredCustomer).WithState(NullCustomer)
                    .Must(ReturnCustomer).WhenAsync(CustomerEmailClaimIsSet, ApplyConditionTo.CurrentValidator).WithState(EmailCustomer)
                    .Must(ReturnCustomer).WithState(NullCustomer)
                    .OverridePropertyName(WholeContextPropertyName);
            });
```

Next example [shopping cart common warnings validator](01_implementation_level/ShoppingCartCommonWarningsValidator.cs) as I already mentioned used the single CustomAsync() method and validated inside everything with if, switch and custom setting of failures. After proper refactoring we end up with user and developer
friendly DSL version

```
        RuleFor(ItemCount)
            .LessThan(MaxCartItems).When(ShoppingCartTypeIsShoppingCart, ApplyConditionTo.CurrentValidator).WithMessage(MaxCartItemsMessage)
            .LessThan(MaxWishlistItems).When(ShoppingCartTypeIsWishlist, ApplyConditionTo.CurrentValidator).WithMessage(MaxWishlistItemsMessage);

        RuleFor(WholeContext).Cascade(CascadeMode.Stop)
            .MustAsync(HaveCustomerAuthorizedForCartEnabling).When(ShoppingCartTypeIsShoppingCart, ApplyConditionTo.CurrentValidator).WithMessage(CartDisabledMessage)
            .MustAsync(HaveCustomerAuthorizedForWishlistEnabling).When(ShoppingCartTypeIsWishlist, ApplyConditionTo.CurrentValidator).WithMessage(WishlistDisabledMessage);

        RuleFor(Quantity).GreaterThan(0).WithMessage(QuantityPositiveMessage);
```

## Method only exists in tests

This one was taken from my personal experience during my internship. I was writing an [excel reader](02_method_used_only_in_tests/XlsxReader.cs) for theimport and because I had no idea how to test my class I copied the implementation of the team lead who used some library for working with excel and started writing tests **for the implementation** which is an inherently wrong approach. For that I created an internal setter which allowed me to set the mocked the internal implementation `IExclelReader` during test. And this internal method was used just in tests and broke the whole encapsulation. In the tests I was literally saying what and when it returns and just adjusted tests to the implementation. I will never do it again. As I figured out later the best way for such cases are simple integration tests. Yes, we do have to access and load files. But what is bad here? This given us however a clear expectation what the class should return on which file input. Just create several sheets and then load them using MemoryStream and assert maybe even using snapshot testing to avoid any asserts at all. Although this is an integration test because we interact with I/O in essence this is just a unit test which uses a file for input. Plain and simple.

## Method calls chain

How hard it is to follow a deep chain of methods calling over private methods which in turn call other private methods. And the whole flaw comes here from the lack of logical thinking. For example the [search of products](03_methods_call_chain/GetSearchProductsQueryHandler.cs). Initially the whole handler had 485 lines of code. That's crazy. DDD with its commands and queries probably mean something different and not this madness. Main method was calling other private methods which were calling other private methods with tons of ifs and elses which were calling other private methods...

The whole process can be represented as a pipeline where we on each step apply some change to the initial request and return a new one. Pipeline consists of multiple steps where each step is a separate ADT. Step can be applied or ignored, the steps order should be configured manually, however it gives us a full control of how the whole search is composed. I also abstracted away the basic filtering and ordering using not rocket science ADTs and after all refactorings ended up with this version

```c#
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
```

## Too many parameters

Although it is not a fix rule how many parameters a method should have, after some research the number 4-5 seemed to be the red flag and a sign that we need to refactor the method. But do we need to refactor the method? We need to refactor the way we think about the method and whether it really does only one thing at a time. Because very ofter if we have 10 parameters then our method does to many things as in the example of [product collection service](04_too_may_parameters/ProductCollectionService.cs). At first glance there is nothing bad, however if we take a closer look we are responsible here for : caching, pagination and querying the product collections. Too many things right? After spending some time with it following steps were done

1. Separate the caching from the actual logic of the methods aka Proxy pattern. For that we implement the actual logic for the CRUD operations in the normal default implementation and then use it in the cached implementation.
2. Cached implementation however needs to cache data, the problem is that very often caching is implemented using lazy evaluation and the actual data retrieval is delayed. So what if we have some abstract query ADT which would allow us to be executed later by the client and in case of the cached implementation we would executed the query exactly in the lazy form? Yeah that's possible, for that we just introduce the `IQuery` with 2 implementations `NormalQuery` and `CachedQuery` where the cached query uses inside the cache. And the cached implementation of the service return this implementation to the client. The client however does not care which implementation is returned and just uses the interface

```c#
public Query<ProductsCollection> GetByCollectionId(string collectionId, string storeId, string[] allowedCustomerGroupsIds, bool showHidden)
{
    var normalQuery = _productCollectionService.GetByCollectionId(collectionId, storeId, allowedCustomerGroupsIds, showHidden);
    var key = string.Format(CacheKey.PRODUCTCOLLECTIONS_ALLBYCOLLECTIONID_KEY, false, collectionId, _contextAccessor.WorkContext.CurrentCustomer.Id, storeId);
    return new CachedQuery<ProductsCollection>(key, normalQuery.Queryable, _cacheBase);
}

...

var allowedCustomerGroupIds = _contextAccessor.WorkContext.CurrentCustomer.GetCustomerGroupIds();
var query = _productCollectionService.GetByCollectionId(collectionId, storeId, allowedCustomerGroupIds, true);
var pagedResult = query.ToPagedList(pageIndex, pageSize);
```

Now we have 4 parameters instead of 5. It had to be 3 because I removed pagination configuration and lifted it to the client, however I removed a dependency to the context accessor inside of the service which was used only to retrieve the customer group ids, that's why 4. In this case, however, I do not see anything bad having all 4 paramters related to the query itself.

## Weird decisions

The first weird decision was observed in [attribute extensions](05_weird_decisions/CheckoutAttributeExtensions.cs) where I literally saw two 99% same methods with different names what was very confusing. This is a typical case when code duplication leads to confusion, after extracting the same part to an extra method and giving it a proper name it started making sense that in one case we want to have attribute extensions which should have values or in other words which are selectable and in the second case we want that they are selectable and also not a checkbox. With proper naming it all makes sense...

The [second weird place](05_weird_decisions/ProductExtensions.cs) was with 2 wrapper methods which were just returning data from an `out` variable. They were the same except that different variables were returned. Both methods should be deleted as we can directly use the out parameter and then return it. No benefit from this fragmentation.

## Too much data returned to the caller

Very often we just need to return an ID and for some reason load the whole entity from the database - happens everywhere. Like in [this case](06_too_much_data_returned/BrandImport.cs) where we load a picure and return it, however only its id is needed. No need to give the whole picture to the caller. Or like in the [example wiht a brand](06_too_much_data_returned/BrandsByDiscount.cs) where we return the whole Brand which inherits from the base entity model and has many other properties, however the caller only needs the brand id and name which can be returned in a tuple form. Plain and simple.