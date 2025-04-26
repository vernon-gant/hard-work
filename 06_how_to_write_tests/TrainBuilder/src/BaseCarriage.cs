namespace TrainBuilder;

public abstract class BaseCarriage : ICarriage
{
    protected BaseCarriage(int capacity)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(capacity);

        Capacity = capacity;
    }

    public int Capacity { get; }

    public abstract char Marker { get; }
}