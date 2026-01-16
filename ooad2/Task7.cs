public abstract class Animal
{
    public abstract void Voice();
}

public class Cat : Animal
{
    public override void Voice()
    {
        Console.WriteLine("Cat")
    }
}

public class Dog : Animal
{
    public override void Voice()
    {
        Console.WriteLine("Dog")
    }
}

public class Rat : Animal
{
    public override void Voice()
    {
        Console.WriteLine("Rat")
    }
}

public class Zoo
{
    public void Process(Animal animal)
    {
        // This is an example of dyncamic binding where we do not know
        // at compile time which method will be called - one of Cat, Dog or Rat.
        // In runtime it will be decided based on the real object type which this reference points to.
        // And we are probably not interested which exactly as we are just interested
        // in the interface of an Animal.
        animal.Voice();
    }
}