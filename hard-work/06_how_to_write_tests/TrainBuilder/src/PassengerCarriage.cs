namespace TrainBuilder;

public class PassengerCarriage(int capacity) : BaseCarriage(capacity)
{
    public PassengerCarriage() : this(DefaultCapacity) { }

    private const char ConstMarker = 'P';

    public override char Marker => ConstMarker;
}