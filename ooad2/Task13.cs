// Variant 1 - method is public in parent class and in derived class.
// Is a default case where we just inherit public method and it also becomes public in the derived class.
public class Parent
{
    public void PublicMethod()
    {
        Console.WriteLine("Parent Public Method");
    }
}

public class Child : Parent
{
    // Inherits PublicMethod from Parent
}

// Variant 2 when in parent type we have a public method and in derived a hidden one is not really supported. The only things we can achieve is to use the "new" keyword and hide the
// method override from parent class preventing dynamic binding from calling the overriden method. We will actually create a new method accessible only from derived
// class reference. For example

public class Parent
{
    public void PublicMethod()
    {
        Console.WriteLine("Parent Public Method");
    }
}

public class Child : Parent
{
    // The new keyword hides the Parent's PublicMethod in the scope of Child
    private new void PublicMethod()
    {
        Console.WriteLine("Child Public Method");
    }
}

Child child = new Child();
child.PublicMethod(); // Outputs: Parent Public Method

Parent parentAsChild = new Child();
parentAsChild.PublicMethod(); // Outputs: Parent Public Method

// So not really what we wanted but we can create a new method with the same name and hide it from parent class. Just wanted to mention that. But in the strictest sense variant 2 is not supported.


// Variant 3 is not supported at all. I can even not imagine that...

// Variant 4 is also a default variant and is supported. Here we can create private methods with same names in both base and derived classes and it't totally okay because they are private and do
// not interfere with each other

public class Parent
{
    private void PrivateMethod()
    {
        Console.WriteLine("Parent Private Method");
    }

    public void CallPrivate()
    {
        PrivateMethod();
    }
}

public class Child : Parent
{
    // Not overriding
    private void PrivateMethod()
    {
        Console.WriteLine("Child Private Method");
    }

    public void CallChildPrivate()
    {
        PrivateMethod();
    }
}