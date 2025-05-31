# Visitor Pattern in OOP and FP

This pattern becomes relevant when there is an existing Abstract Data Type (ADT) that we typically don't want to change by adding new methods. This aligns with the principles of Object-Oriented Programming (OOP). However, sometimes the project context requires adding these behaviors. This scenario applies specifically to OOP languages due to their architecture and specifics.

Nevertheless, the statement "not to add new methods" is somewhat controversial because we often end up adding methods anyway. More precisely, we aim to extend classes as interfaces and decouple method definitions from their implementations. For example, adding a new method to in case of the visitor pattern looks like:

```csharp
public void Perform(IVisitor visitor)
{
    visitor.Perform(this);
}
```

Here, we extend a class with a new method from the interface's perspective, but delegate the actual implementation to another class. Thus, we can freely swap implementations without touching the original class's source code.

## OOP Specifics

In OOP, we can easily introduce new types using inheritance by adding child classes. However, adding new methods directly to classes is not recommended and is considered bad practice because it can violate the Single Responsibility Principle (SRP). Continuously adding new methods to a class complicates it and blurs responsibilities. For example, consider a `Document` class with subclasses `PdfDocument` and `WordDocument`. If we add a method like `exportToJson()` to the `Document` class, it then becomes responsible for JSON exporting in addition to document storage and logic.

Moreover, adding new methods may break class invariants. An ADT defines invariants, for example, that a document must always have at least one page. A new method accessing internals could inadvertently violate this invariant - such as deleting a page without proper checks.

Additionally, adding methods breaks the Open/Closed Principle, which states that classes must be closed for modification but open for extension. Adding new methods modifies existing, working, and tested class code. The proper approach is either to use composition, as shown below, where a service gains new functionality without altering the original service:

```csharp
public class Logger
{
    void Log(string msg) { /*...*/ }
}

public class ServiceWithLogging
{
    private readonly Service _svc;
    private readonly Logger _log;

    void Operation()
    {
        _log.Log("start");
        _svc.Operation();
        _log.Log("end");
    }
}
```

or to use inheritance with subclasses introducing new methods:

```csharp
public class Base
{
    void Operation() { /*...*/ }
}

// extension without modifying Base:
public class Extended : Base
{
    void NewFeature() { /*...*/ }
}
```

Another option is the Visitor pattern, which allows adding methods without modifying existing code or breaking invariants. However, it is not a perfect solution but rather a workaround that compensates for this drawback in OOP. We decouple the operation description from its implementation by defining method signatures in a new interface. These methods accept the class we wish to extend:

```csharp
interface IBicycleVisitor
{
    void TimeTrial(RoadBike s);

    void BunnyHop(MountainBike m);
}
```

We then bind this visitor to the base class of these subclasses and implement a dispatch method to determine the type and call the appropriate method:

```csharp
public abstract class Bicycle
{
    public abstract void Accept(IBicycleVisitor visitor);
}

public class RoadBike : Bicycle
{
    public override void Accept(IBicycleVisitor v)
    {
        v.TimeTrial(this);
    }
}
```

This implementation avoids modifying original classes or their invariants, interacting only through their interfaces.

## Multiple Dispatch

Languages like Julia support functional programming techniques, including a powerful mechanism called multiple dispatch, which selects the correct function at runtime based on argument types. We can have multiple functions with the same name and parameter counts, differing only by parameter types:

```julia
function collide(a::Asteroid, b::Spaceship)
  # …
end

function collide(a::Spaceship, b::Asteroid)
  # …
end

# At runtime, calling collide(x, y) selects the version matching the types of both x and y.
```

In OOP, there is no built-in mechanism for dispatching based on two or more runtime argument types simultaneously. Instead, we rely on overloads or virtual method dispatch (virtual single dispatch):

### 1. Static Overload Resolution

We can declare multiple methods with the same name but different parameter types or counts. Resolution is entirely static. If the compiler does not find a method matching the provided argument types at compile time, it results in a compilation error:

```csharp
public class A { }
public class B { }

public class DerivedA : A { }
public class DerivedB : B { }

public class Processor
{
    public void Process(A a, B b)
    {
        Console.WriteLine("Process(A, B)");
    }

    public void Process(DerivedA a, DerivedB b)
    {
        Console.WriteLine("Process(DerivedA, DerivedB)");
    }
}

public class Program
{
    public static void Main()
    {
        A aBase = new DerivedA();
        B bBase = new DerivedB();
        Processor p = new Processor();

        // Compiler sees aBase as A and bBase as B → picks overload #1
        p.Process(aBase, bBase);  // Outputs: Process(A, B)
    }
}
```

### 2. Virtual Single Dispatch

If a method is virtual or abstract and overridden in subclasses, at runtime, the method call resolves based solely on the actual type of the object (`this`), not on argument types:

```csharp
public abstract class Shape
{
    public abstract void Draw();
}

public class Circle : Shape
{
    public override void Draw()
    {
        Console.WriteLine("Draw Circle");
    }
}

public class Program
{
    public static void Main()
    {
        Shape s = new Circle();
        s.Draw();  // Outputs: Draw Circle
    }
}
```

The Visitor pattern is called double dispatch because it dispatches based on the type of the element first and then on the visitor type:

```csharp
public interface IBicycleVisitor
{
    void VisitRoadBike(RoadBike bike);
}

public class RoadBike : Bicycle
{
    public override void Accept(IBicycleVisitor visitor)
    {
        visitor.VisitRoadBike(this);
    }
}
```

## Visitor Problem

The main problem is that the declared universal method .Visit() or .Operate() on the base element - which subclasses implement - is not truly universal, because in each override we insert a specific call to the visitor. A second problem is that, by default, one visitor supports only a single concrete logic. When we extend the class interface with additional operations, we end up needing new universal methods named Operate1, Operate2, and so on.

The root cause lies in the OOP paradigm itself—methods/behaviors and data are stored together by definition. Inheritance must be applied cautiously so as not to break the Liskov Substitution Principle (LSP). As mentioned, by using subclasses we can extend existing classes with new functionality. However, the key is that when we work with polymorphic entities, we typically store them in containers holding base‐class references. To access child‐specific methods, we must downcast, which is always a code smell. Yes, we have pattern matching in C# now, but it does not make our code semantically safe.

## Mixins

In general, the Visitor pattern is useful when we want to extract behavior into an external class, with individual visitors implementing that behavior. However, sometimes when adding behavior to multiple types, we want to separate *which types* have this behavior from *how* the behavior is implemented. In other words, we want to avoid copying and pasting the same method implementation into multiple classes, especially if those implementations are very similar. Instead, we want to inject the implemented behavior directly into the desired classes. In OOP, this technique is called a **mixin**. The mixin class itself is not meant to be instantiated or to inherit from other classes. Mixins clearly separate state from functions.

In C# implementation of mixins involves combining interfaces with extension methods. Usually, we create interfaces that may have virtual methods or simply act as dummy interfaces. In my bachelor thesis, I am exploring the GrandNode2 project and I found a good example of a mixin which injects the functinality for getting the SEO friendly name for entities implementing 3 interfaces :

```csharp
public interface ISlugEntity
{
    string SeName { get; set; }
}

public interface ITranslationEntity
{
    IList<TranslationEntity> Locales { get; set; }
}

public abstract class BaseEntity : ParentEntity, IAuditableEntity
{
    public IList<UserField> UserFields { get; set; } = new List<UserField>();

    public DateTime CreatedOnUtc { get; set; }

    public string CreatedBy { get; set; }

    public DateTime? UpdatedOnUtc { get; set; }

    public string UpdatedBy { get; set; }
}
```

An important remark is that we intentionally avoid using default interface methods introduced in newer versions of C#, as changing default methods later would require altering the interface itself. Such changes could break existing code or cause unexpected behaviors. Instead, we move all functionality into static classes containing extension methods what makes them stateless and not allowing classes to inherit smth(because they are static as well). These extension methods use a this parameter corresponding to the interface type, and we may even constrain it to multiple types as in our case:

```csharp
public static string GetSeName<T>(this T entity, string languageId, bool returnDefaultValue = true)
    where T : BaseEntity, ISlugEntity, ITranslationEntity
{
    ArgumentNullException.ThrowIfNull(entity);

    var value = entity.Locales.FirstOrDefault(x => x.LanguageId == languageId && x.LocaleKey == "SeName")?.LocaleValue;
    var seName = !string.IsNullOrEmpty(value) ? value : (returnDefaultValue ? entity.SeName : string.Empty);

    return seName;
}
```

Thus, if the target type satisfies the type constraints—or simply implements the single interface when multiple constraints aren't used—it will immediately acquire the desired functionality without touching its original source code.