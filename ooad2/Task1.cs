// Looks like I copied it from ChatGpt but now :) vehicles and animals are just well built for inheritance, polymorphism and composition
public class Engine
{
    private int _capacity;
    
    public void TurnOn()
    {
        
    }
    
    public void TurnOff()
    {
        
    }
}

public abstract class MotorVehicle
{
    protected int _releaseYear;
    
    protected string _vin;
    
    // Every motor vehicle has an egine so this is a good example of composition
    protected Engine _engine;
    
    public abstract void Start();
    
    public abstract void Stopp();
}

// Moreover we can have different types of motor vehicles and they all are is-a relationship - for example car
// A car is a vehicle and can also take a form of it in the program(we can save it in the MotorVehicle reference) so polymorphism is also here!
public class Car : Vehicle
{
    // We could call for example this method on a Car object which is stored in the MotorVehicle reference.
    // Because Car is a subtype we do not car which one - Car or Bike. We know they are and depending on the real type appropriate method will be called.
    public override void Start()
    {
        Console.WriteLine("Car started...");
        _engine.TurnOn();
    }
    
    public override void Stop()
    {
        Console.WriteLine("Car stopped...");
        _engine.TurnOff();
    }
}

public class Bike : Vehicle
{
    public override void Start()
    {
        Console.WriteLine("Bike started...");
        _engine.TurnOn();
    }
    
    public override void Stop()
    {
        Console.WriteLine("Bike stopped...");
        _engine.TurnOff();
    }
}