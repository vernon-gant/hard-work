namespace TrainBuilder;

public abstract class BaseCarriage
{
    protected BaseCarriage(int capacity)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(capacity);

        Capacity = capacity;
    }

    protected int Capacity { get; init; }

    public abstract char Marker();
}