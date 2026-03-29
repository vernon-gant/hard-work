# 1. Dependency of Framework

I did not think about "dependency" from this perspective. Obviously, frontend components implemented using Blazor **DO** depend on Blazor, its rendering cycle and contracts. But does Blazor depend on it? Obviously not, like a task scheduler. It just runs them generically. What might happen here is that **WE** start depending on the Blazor in our BL using some assumptions about its internals. This is very bad and should never be done. Components only call the BL which knows nothing about Blazor or whatever external mechanism.

# 2. Dependency of the Shared Format

Authorities post every `n` months an XML containing
entities such as `Skills`, `SkillToWorkEnvironments` or `SkillToSubskills`. At the beginning they were posting just XML files and we had a bunch of DTOs with XMLElement attributes, which had to be adjusted. After 3 times it was proposed that they post together with XML files also XSD definitions. The idea was that every time if something changed in the XSD, we just generate new models using the `dotnet-xscgen` and then reimplement the handlers which map the aggregate DTO of single files to our owned types. Because there could be files, containing multiple entities like `ImportantSkills`, `ImportantWorkEnvironments`, etc. And because next time these entities could traverl to another file, we just defined handlers which returned the unified `ImportEntity` empty interface which concrete objects holding the domain objects aka adapter:

```cs
public interface ImportEntity;

public record struct AmsSkillImportEntity(AmsSkill Skill) : ImportEntity;
```

And then using the Mediator and its polymorphism the `UpdateDatabase<T>` request was dispatched to the correct handler. Maybe too detailed, but the idea is that now we do not care what is the format of their XML, we just know that it will contain the entities we need, the question is where :) Yes, we generate a lot of DTO, and maybe write a bit more code, but do not search the XSD files and compare with previous version looking for any new changes.

# 3. Dependency of Dependency

Recently had one :) I was working on a query for sending skills of a participant to authorities, wrote unit tests for my query with test containers(50/50 integration test) and all worked fine. After merge tests were failing, but I changed literally nothing. It turned out that my colleague added a global query filter to the `DbContext` for the participant and booom :) Both depend on `DbContext` and refactoring of `B` changed this `DbContext`. The solution was to clearly communicated and document such things and really justify with a big fight in daily why we need global filters, because everyone then should keep in head their existance. Especially if they are non trivial like soft deletion :)

# 4. Dependency of Crash

This is an interesting counter argument to the broad definition that "depends on = change can affect". Everything depends on everything over crash :) In ASP.NET this is actually true, an unhandled exception in the HostedService is not swalled anymore and stops the program. So not having a default exception catch somewhere in the loop would make everything depend on everything accoding to the broad definition... But of course we can either use catch or use OneOf if the battle with the senior regarding the advantage of explicitness instead of implicit exception generation is won.

# 5. Dependency of Fallback

The only case of this dependency type I had till now **implicitly** is probably the cache -> database query fallback, any other API call fallbacks were not used. We just used for the email sending the `SendGrid` and no other provider and if you get 500, then it sucks :) In the case of loading data there is nothing much one can do here, because if cache is not available we go to the database. Does the app depend on the cache in this case? It depends :) If we treat it that app depends on cache if cache's failure causes app's failure, then no. But cache'e failure also makes the app load data longer, so yes. We definitely need a rigorous definition of what a dependency is!

# 6. Dependency of Inversion

The project attempted to use the DDD with CQRS with the extensive use of MediatR for commands and queries. And in order to decouple the caller from the "Service", all service calls were wrapped into a MediatR request handler, which did nothing but just routed the call to the service. At the end the controller still depends on the service B which is injected over service interface into the MediatR handler. At the end it just produced 2 levels of abstractions which are arguably needed. I proposed then just to write the query/command in the request handler directly using the domain types and eliminate these pretty "services" completely.

# 7. Cyclic Dependency

Had a case last week. There was a `Domain` project and I wanted to extract the `Access Rights` into a separate project. To bind the entities which had a claim in the system or in other words access to which must be explicitly grante in the system for read/delete/update I introduced the

```cs
public interface IClaimEntity
{
    static abstract ClaimValue { get; }
}

public class Project : BaseEntity, IClaimEntity { ... }
```

Firstly, I allocated the `IClaimEntity` into the separate `Identity` project. All entities are in the `Domain` project => cyclic dependency, because in order to implement the `IClaimEntity` the `Domain` project had to reference the `Identity` project and `Identity` project had to reference the `Domain` project for the `Claim` ids which are defined in a static class. To resolve it I just moved the `Claims` into `Identity` as well and removed the dependency of the `Identity` project on the `Domain`. Maybe still not the best design, but it was the first iteration...

# 8. Higher Order Dependency

Dependency on the correctness of the properties of the client code, sounds cool! I did not have the problem with this type of dependency, but if we continue the previous example, then implementing the `IClaimEntity` for the `Project` and setting in the `Project` claim for some other entity would break the behavior in the access right module, because it will use the wrong claim value. How easy it is to break things actually :)

# 9. Dependency of Majority

I think this is the typical case of writing different type of tests so that they "independently" test the program. We can write a unit test, integration test and even and e2e test for the feature we do, but if we miss some assumption or the spec is ambiguous about an edge case, all 3 different types of tests will miss it. The hidden coupling in that case would be the assumptions about the program behavior. PR reviews by different colleagues also count :)