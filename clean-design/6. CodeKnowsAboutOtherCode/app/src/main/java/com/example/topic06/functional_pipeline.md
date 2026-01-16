## Functional pipeline

After some rethinking of the import process I decided to make a simple step based pipeline of mapping input to output in each step. I actually reinvented the wheel in terms of functional composition in the functional programming but I got a pretty nice
way to functionally model some sort of "Chain of responsibility" but in functional style where out steps in general are functions, but not pure of course. Then we can combine these steps into a "Pipeline" and execute it. Pipeline can also get a request so we mark it as contravariant.

```c#
public interface IStep<in TRequest, in TInput, TOutput, TCodes>
{
    OneOf<TOutput, TCodes> Execute(TRequest request, TInput input);
}
```

And then we also define the pipeline

```c#
public interface IPipeline<in TRequest, TOutput, TCodes, in TRequest>
{
    OneOf<TOutput, TCodes> Execute(TRequest request);
}
```

The idea is that our pipeline in general is just a "function which takes a request" and then produces an output or the TCodes - i could not come up with a better name :) here I means just the return types which are normally structs with OneOf which just tell with their name the statuc code like

```c#
public struct Unauthorized;
```

and of course we can combine them into OneOf<> and get OneOf<TOutput, OneOf<...>>. We start working with a request and from a pipeline perspective we do not need any `input` type. Then we can have a normal builder with method like "Start", "Then", "End" where say we start with a step which takes an input type "None" and end with a step which returns "TOutput". Then we can implement the steps and in a strongly typed manner build the pipeline. Reinventing the wheel but allows us to build a functional pipeline in OOP :)