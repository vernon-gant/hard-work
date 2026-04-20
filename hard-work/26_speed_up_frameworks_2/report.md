# Default external code behavior

## 1. Blazor `OnInitializedAsync`

When I started working with Blazor I started seeing `NullReferenceException`s in during rendering and could not connect it with my missing knowledge about rendering. My expectation about this method which is called when a component is created was, that it should complete until the component renders and then when we get our data, component renders. As it turned out all modern UI frameworks use asynchronous rendering and in case of component initiliazation this is called something like `two step rendering`. Component get initialized, the `OnInitialized` and `OnInitliazedAsync` are called, and first rendering starts immediately without really awaiting the `OnInitiliazedAsync`. Now it seems logically to me, because we do not want to block everything until data arrives and we need to show something to the user. The correct pattern here is to use the spinner so on the pages that require data, so that we say do not access the properties of person until we load it.

```cs
private Person person;

// component

<RadzenLabel Text="@person.Name"/> // produces NullReferenceException if person is null
```

In my case these we some complex objects which we manually initialized with `= new()` but internally their properties we fully initialized only after some child component's `OnInitializedAsync` completed. There was no spinner, of course, on this page, and only the person who wrote the code knew that we should not access properties of this object or call methods which touch this internal field. So here only my misunderstanding and a careless coding from this colleague.

## 2. Custom OData select

I just wanted to quickly do an OData `$select` on some request, and saw that there is some custom endpoint in the `GenericDataController` which does the select:

```cs
[HttpGet("select")]
public async Task<IActionResult> GetSelect(ODataQueryOptions<TEntity> queryOptions)
{
    try
    {
        Type targetType = typeof(TEntity);

        if (queryOptions.Request.Query.ContainsKey("targetType"))
            targetType = Type.GetType(queryOptions.Request.Query["targetType"]!) ?? targetType;
        ...
}
```

these type manipulations looked already very suspicious to me because there is already an embedded select defined by the protocol and the library. So I made a call to this endpoint, passed the type and got some weird exception from the reflection lib. This was again my mistake, because there was actually a documented method which instructed how to use the endpoint from a `GenericDataService`, which I also did not use...

```cs
/// <summary>
/// Returns a subset of the given Entity as determined by the select clause of the oDataParams
/// </summary>
/// <param name="oDataParams">The oDataParams to apply</param>
/// <typeparam name="TEntity">The Entity to apply the oDataParams to, used to determine the endpoint to call</typeparam>
/// <typeparam name="TResult">An object which contains all properties that are used in the select clause. It's important that the naming of the individual properties stays the same in TResult and TEntity.</typeparam>
public async Task<IEnumerable<TResult>> GetODataSelect<TEntity,TResult>(ODataParams? oDataParams) where TResult : new()
{
    ...
}
```

I still told the team lead that they just reinvented the wheel and this endpoint does not make any sense at all. But I admit that I overlooked the documentation.


## 3. Entity Framework Core behavior before deep research

Before the deep research on how EfCore works, I came across the thing which I thought was a bug. But it was not. I made the AI interpretation dialog where we interpret the CV and then save the data using the server side models. The user just chooses which fields he wants to persist - existing or interpreted. There were also collection of elements like work experience and user can choose whether parsed work experience from CV should be in the final collection. After all selections and populations of collections with selected items, I just sent the model to the `PUT` method of the `GenericDataController` and was wondering why everything except collections is set and saved. After digging in I saw the `DbContext.Entry(x).SetValues(incoming)` instruction and still though that I was doing something wrong on the client. But the data was there in the incoming model in the controller. And after looking up the documentation for `.SetValues()` it turned out that it sets only scalar properties. What also makes sense, because how should `EfCore` resolve case, when we did not load the collection properties aka `JOIN` and we just override the the values from new collection, how should it handle the existing. Especially if there are intersections. So collections had to be reconciled separately. 30 points form Griffindor...


# Well typed results

## 1. ReadOnly collections

Follow up to the example with `Set` where we weaken the specification and stay concrete on the implementaiton level. Sometimes we still need to restrict the specification because the logic requires so. We should not allow the user to do everything, at the end of the day the well typed system does not allow to do things which violate the rules, follow up to the capabilities from clean architecture :) Why returning mutable collections from methods where mutating these collection would corrupt the state? My favourite FluentValidation...

```cs
public class ValidationResult {
	private List<ValidationFailure> _errors;

	/// <summary>
	/// Whether validation succeeded
	/// </summary>
	public virtual bool IsValid => Errors.Count == 0;

	/// <summary>
	/// A collection of errors
	/// </summary>
	public List<ValidationFailure> Errors {
		get => _errors;
		set {
			if (value == null) {
				throw new ArgumentNullException(nameof(value));
			}

			// Ensure any nulls are removed and the list is copied
			// to be consistent with the constructor below.
			_errors = value.Where(failure => failure != null).ToList();
		}
	}
}
```

Maybe the author did not suspect that there will be people who will add validation failures manually to the resulting `Errors`. This is really mad. The validators were designed composable, one validator can use other through DI and their validation failures will be merged. This manual manipulation of the `Errors` allows us to continue adding failures which are not under the validator responsibilities. The validation logic spreads, here we added some failure, there, some deep in the method call chain and then we have no idea who is responsible for validation. Restrict! `IReadOnlyList` or even `IReadOnlyCollection`.

## 2. Primitive Obsession

What is the difference between passing a `Guid` which is supposed to be a `UserId` and a `Guid` which is supposed to be a `ProjectId` to a method which expects a `Guid userId`. There is no difference! Same applies to returns:

```cs
public static List<Guid> GetProjectsWhereUserHasClaim(this UserInfo user, ...)
```

Second example where we should make our specification stricted and not softer, because we want to differentiate between a `ProjectId` and a `UserId` at least so that compiler tells us where we accidently pass raw guids in a swapped manner. [There is even a C# library for that!](https://github.com/andrewlock/StronglyTypedId). Even with source generators and EfCore integration!


## 3. Even more types

```cs
public async Task<(double, double)> GetCoordinates(string address, string? country = null)
```

Google treats the order in the `latitude comes first` manner, `GeoJSON` in the reversed order. So what this tuple should be? The calling code saves the coordinates in the database, so using a `Point` defined by `NetTopologySuite` gives us a concrete type with operations and semantics at this point. But the most important part, that it fixes this ambiguity **what comes first**.