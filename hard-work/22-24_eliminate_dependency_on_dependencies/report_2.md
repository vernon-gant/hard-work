# 1. OData

## Semantics

### Static

Dependency on `Microsoft.AspNetCore.OData` packages. All controllers for most entities inherit from ODataController -> the build dependency is deep - removing the package would break compilation of practically the entire API layer.

### Dynamic

On startup OData engine builds the Entity Data Model and after that every GET request coming from the client gets translated into Expression trees, which are then taken by the EF Core. If there is something wroton with the middleware or with the engine, everything breaks.

### Functional

The system fully depends on OData on the logical level, there is no abstraction layer between OData and the consumers/model building. There is no any equivalent query mechanism which would preserve the correctness of the system when we would swap OData, it is too deep on both sides.

## Key Properties

The most important is of course Correctness of the API, if OData incorrectly translates $filter or $select, the wrong data will come. Performance and security are not that centered here, but usability from the client query building perspective and omitting building endpoints for every use case is definitely a key property.

## Space of Allowable Changes

In the case of proejct which uses OData that deeply, the space where we are independent from OData is tiny and is probably some internal changes of how say `$select` is parsed or transformed into Expression Tree. The **super specification** is that these changes on the library level do not affect the protocol correctness.

# 2. Blazor WASM

## Semantics

### Static

The entire frontend is built with Blazor WASM - all UI components inherit from `ComponentBase`, use [Parameter] attributes, Razor syntax, etc. Removing `Microsoft.AspNetCore.Components` would leave no frontend at all.

### Dynamic

Blazor WASM runs the entire .NET runtime in the browser via WebAssembly. Every user interaction goes through Blazor's rendering pipeline + it manages the component lifecycle. And of course diffing which is then mapped to a RenderBatch which is then sent to JSInterop layer which then applies changes to the DOM

### Functional

No abstraction layer, and i think in case of frontend frameworks it is almost impossible to have one, then you write you own framework :) All components use Blazor specific types like `EventCallback` or `NavigationManager`. Switching to React just means rewrite everything from scratch.

## Key Properties

The main sale point of Blazor is of course usability, we write frontend and backend Code using C#, we can share models, with Blazor Hybrid even call methods on the server over SignalR... Performance is of course also an important one, Blazor WASM has a heavy initial load, but after the runtime loaded + started everything runs pretty fast. Security as another cross cutting concept is also relevant here, such a client side framework should be securely designed and regularly patched with security fixes, so "in active maintainance" is also an important property.

## Space of Allowable Changes

If we consider internal changes that preserve Blazor's public contracts - our system does not depend on Blazor's internals. If we consider changes to the component lifecycle or rendering model - our system does depend on Blazor internals, because every component uses these directly. The super specification is that any of these changes do not affect the correctness of the UI rendering in browser.

# 3. Gotenberg (Word -> PDF Conversion)

## Semantics

### Static

The project depends on the Gotenberg NuGet client package for the strongly typed HTTP client.

### Dynamic

Gotenberg runs as a separate Docker container alongside the app. When a report needs to be generated, the backend calls Gotenberg synchronously via HTTP to convert Word to PDF. If the Gotenberg container is down or unreachable, report generation fails - there is no fallback. So at runtime the system fully depends on Gotenberg being alive.

### Functional

This is where it gets interesting compared to OData and Blazor. The abstraction with a single method hides Gotenberg completely. We can reason that swapping Gotenberg for any other converter would preserve correctness, as long as it converts Word to PDF. The system does NOT depend on Gotenberg at the functional level.

## Key Properties

Correctness of report generation, definitely - if Gotenberg produces a broken PDF or mangles the Word layout, the reports are wrong. Performance - conversion happens synchronously during the request, so Gotenberg's speed directly affects response time. Reliability - because Gotenberg runs as a separate Docker container, it introduces a network hop and an additional point of failure that a local library would not have.

## Space of Allowable Changes

If we consider changes to Gotenberg's API, NuGet client or internals - our system does not depend on it, everything is hidden behind the abstraction, only the single implementation class is rewritten. If we consider changes to the quality of PDF output - our system does depend on Gotenberg, because no interface can protect from that. The **super specification** follows from that - all changes except those touching PDF output quality do not affect the system.

# 4. FluentValidation

## Semantics

### Static

The project depends on the FluentValidation NuGet package. Validators inherit directly from `AbstractValidator<T>`. However unlike OData or Blazor, it is not used everywhere - only in some places, so removing the package would break only those specific validators, not the whole system.

### Dynamic

Validators are called manually before processing commands or inputs. If FluentValidation itself breaks at runtime, only the flows that use validation would fail - the rest of the system continues to work. The blast radius is limited compared to OData or Blazor.

### Functional

There is no abstraction layer over FluentValidation - the rule-building API is used directly in validator classes. Swapping to something else (DataAnnotations, manual validation) would require rewriting every validator from scratch. However since validators are isolated classes and not spread across the whole codebase like OData syntax, the effort is contained to rewriting the validators themselves, and a bit adjusting the client logic to call a different api.

## Key Properties

Now correctness of the system state is the property - if FluentValidation evaluates rules incorrectly, invalid data gets through or valid data gets rejected. Security is also important - validation is a defense layer, if it silently fails or a vulnerability is discovered, bad input reaches the system. I remember that the Mock library in C# was sending some data to their servers and we had to switch to NSubstitute.

## Space of Allowable Changes

If we consider changes that preserve public API - our system does not depend on FluentValidation's internals. They can rewrite the rule evaluation engine, optimize performance, whatever. If we consider changes that modify the rule building API itself - our system depends on the library, every validator must be rewritten. Same with changes touching correctness of rule evaluation - our system depends on FluentValidation, because invalid data reaches the system.