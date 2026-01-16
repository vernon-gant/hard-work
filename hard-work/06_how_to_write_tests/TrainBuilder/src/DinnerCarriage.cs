namespace TrainBuilder;

public class DinnerCarriage(int capacity) : BaseCarriage(capacity)
{
    private const char ConstMarker = 'D';

    public DinnerCarriage() : this(DefaultCapacity) { }

    public override char Marker => ConstMarker;
}