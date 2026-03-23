# 1. Dependency of Framework

Found an interesting example today. The initial dependency problem could be like "Does the Automapper(object mapping library in C#) engine depend
on the types that are used for mapping?". It is interesting, because today I found an issue in automapper that there is "vulnerability" which can
cause StackOverflow for types with a deep level of how nested the properties are. Person owns Person which owns a Person. And if this level reaches something like 25000 then the app will crash. Automapper went commercial and of course they published the update in the commercial versions, leaving the open source with "critical vulnerability". Nice move :)

Now we are equipped with the definition of the dependency. The first interpretation:

1. Stability of the AutoMapper engine at runtime (dynamic semantics). AutoMapper does depend on the mapped types here. A type with 25000 levels of nesting is the factual cause of a StackOverflow crash. The engine doesn't just produce a wrong mapping — it destroys the entire process. This is similar to the round-robin scheduler that doesn't depend on tasks... unless a task hangs and never yields.

If we restrict the space of allowable types to "types with nesting depth less than X which does not cause this problem(I know this sounds like I am silly but I do not remember the actual semantics of the parameter and concrete value which must be set to avoid this problem)" then even runtime stability no longer depends on the specific types. The StackOverflow becomes impossible within this space. Nice :)

2. Correctness of the mapping result(functional requirements). It turns out that we depend here on the configuration for the mapping and not the type itself. The engine will produce correct result regardless of the type as long as the configuration is valid. So in that setup the auto mapper does not depend on the type.


# 2. Dependency of the Shared Format

Last week we talked about this with the team. We use OData for frontend backend communication and there is no versioning. Classical problems - someone changed the property name on the backend and frontend was broken. Who depends on what :) Common decision was to move to OData API versioning. Of course there is already an existing library for that and we just need to add it. Now we will have versioned EMD model and both client/server will only depend on the shared format aka the versioned contract. Nothing much what I could add here at thi point...

# 3. Dependency of Dependencies

We used Radz
en's DialogService to show a modal with a participant CV preview inside. The DialogService uses JSInterop to set overflow: hidden on <body> to prevent background scrolling - standard behavior. The preview component was originally used outside of the dialog and did not need any overflow manipulation. But for some mysterious reason it was setting `overflow: auto` on <body> after closing the preview. When the preview was moved into the dialog, boom... Closing the preview killed the dialog overlay, because it overwrote the overflow: hidden that Radzen set. Does the Radzen dialog depend on the preview component?

Initially it did not! The dialog worked perfectly fine on its own. But a change in the preview component introduced a new dependency by touching shared global state - THE DOM. A change in B created a dependency that was not there before. Resolution: we restrict the space of allowable changes - no custom component may directly manipulate global DOM elements like <body> via JSInterop. Who does that will die. Only external library components are allowed to do so, because their side effects are known and documented. Use scoped CSS, layout wrappers, whatever - but do not touch global DOM. This was a pain in the ass for me to debug, because I am not a CSS expert and had no idea what was going on. So finally:

Visual correctness of the Radzen dialog overlay, defined in dynamic semantics, does not depend on the preview component - relative to the space of allowable changes where custom components are prohibited from manipulating global DOM state.


# 4. Dependency of Crash

Almost all modern UI frameworks offer a component development approach. We develop components and compose pages of them. But here comes the problem -
what if one of the components inside of one page crashes? Everything dies. Generalizing it we could say that all components within single page depend on each other, because if one component fails, other components will not be rendered. As proposed in the lesson we resolve this dependency problem by introducing a super specification - "unhandled exceptions do not propagate beyond the component boundaries". Blazor offers for this the `<ErrorBoundary>` component, which is a wrapper component with single responsibility of swallowing the exception and conditionally rendering the markup we want when exception occurs. With the full dependency definition we get:

Succefful rendering of the component on the page in runtime does not depend on the rendering of other componets within single page relative to the space of allowable changes where all exceptions are handled withint the component or the component is wrapped into `<ErrorBoundary>`.


# 5. Dependency of Fallback

 Netwrok related dependencies in this example shine :) As I was writing in the first report, for PDF generation we use Gotenber which runs as a docker instance. Currently we do not have any fallback to other PDF engine, but if we would have, say to some other in process PDF libraries, then quesiton could be : does the PDF generation depend on Gotenberg?

 As mentioned in the lesson, the Gotenberg is part of the dependency set {Gotenber, in process PDF engine}, so the PDF generatin does not depend on Gotenber alone. But is the produced result the same? Here we should refer to two importants points : quality requirements and PDF rendering. Different engines use different PDF rendering strategies, what will result in subtle font, spacing differences. So the output will not be literally the "same". But what about the requirements? On the functional side we do not care about these subtle differences. Minor font/spacing changes are acceptable. So we get:

1. Correct PDF generation defined on the functional level depends on {Gotenberg, in process PDF engine} as a set, Gotenber is just part of the dependency.
2. Visual compliance of the generated PDF on the functional level does not depend on the Gotenberg specifically, because the exact UI details are not stricly specified in requirements.


# 6. Dependency of Inversion

Good example with FluentValidation or how C# compiler can not guarantee you everything out of the box. To create custom validators we must inherit from the `AbstractValidator<T>` and then build our rules inside of the constructor. Then we use this validator using the `IValidator<T>` on the calling side. If we inject, say, `IValidator<CreateParticipantCommand>` our code will perfecrly compile(if the `CreateParticipantCommand`) exists. But if there is no concrete validator which inherits from `AbstractValidator<CreateParticipantCommand>`(will be then auto registered by the DI extension), then runtime resolution of the dependency will fail like in the case with interpreter. The inversion actually removes the dependency at compile time, but leaves a runtime dependency that the compiler can't see. So we get two dependency resolutions:

1. Contract conformance defined in static semantics does not depend on the concrete validator and is enforced by the C# compiler.
2. Validator execution defined in dynamic semantics does depend on concrete validator being registered in the DI container.

Or we can apply the super specification to the second one "relative to the space of allowable changes where the validator is mandatory registered in the DI container".


# 7. Cyclic Dependency

Was hard to find in the codebase, but I found a dependency chain `ParticipantService` -> `ProjectValidator` -> `ParticipantService`. At first glance, when we follow the transitive rule, then the `ParticipantService` depends on itself, but if we look precisely, then we have a following chain : when transfer participant to a new project, participant service goes to the project validator, which has its own logic but also call to the `ParticipantService` where it gets last participant case and if the case is completed then it allows transfer.

Here the transitive chain breaks, because we are not stuck in the same method loop, we call a different method which is another concern. We do not call the `TransferParticipant`. We could visualize that as A -> B -> A'.

Ccorrectness of transferring participant to a new project, defined in functional requirements, does not depend on itself - even though there is a circular dependency at the type level, the causal chain at runtime operates on different methods with different concerns.


# 8. Higher Order Dependency

Just today I decided to stop scattering the module existance check with subsequene module settings retrieval and created:

```cs
public interface IModule<TSettings> where TSettings : class, IProjectSpecific
{
    static abstract Guid ModuleId { get; }
}

public class TimeSlotBookingModule : IModule<TimeSlotBookingSettings>
{
    public static Guid ModuleId => ...;
}

public interface IModuleRegistry<TModule, TModuleSettings> where TModule : IModule<TModuleSettings>, TModuleSettings : class, IProjectSpecific
{
    ValueTask<bool> ProjectHasModule(Guid projectId);

    ValueTask<TModuleSettings?> GetModuleSettings(Guid projectId);
}

public class DbContextRegistry<TModule, TSetting>(DbContext dbContext) : IModuleRegistry<TModule, TSetting> where T : IModule<TSetting> where TSetting : class, IProjectSpecific
{
    public async ValueTask<book> ProjectHasModule(Guid projectId)
    {
        var moduleId = TModule.ProjectId;
        return await dbContext.ProjectModules.AnyAsync(pm => pm.ProjectId == projectId && pm.ModuleId == moduleId);
    }

    public ValueTask<TModuleSettings?> GetModuleSettings(Guid projectId) => await dbContext.Set<TSetting>().FindAsync(projectId);
}
```

But nobody stops the consuming side creating the module with a ModuleId set to random guid or mispelled and using some other module id different from the destination. Or even better, the TModuleSetting in the registry is not present in the DbContext. It will compile perfecly but we will get a runtime exception that the type is not found. So, does the DbContextRegistry depend on the TModule an TSetting instances? Let's clarify that:

1. Correctness of DbContextRegistry as a generic component, defined in functional requirements, does not depend on the concrete TModule or TSetting. The registry correctly calls Set<TSetting>().FindAsync() and ProjectModules.AnyAsync() regardless of what types are passed.
2. Runtime behavior of DbContextRegistry when called with a specific TModule and TSetting, defined in dynamic semantics, does depend on the consuming code. With all problems described above :)


# 9. Dependency of Majority

In the previous company two persons had to give approve on the submitted PR. Because we had PR review, the approve from Team Lead and one colleague or from two colleagues are equal. The problem comes when reviewers share the same knowledge about some domain or programming concept, then if I have the bug which falls under the assumption, which they both share, then the bug will go through. The "independence" breaks exactly here. I had no other ideas about this type of dependency, honsetly :)