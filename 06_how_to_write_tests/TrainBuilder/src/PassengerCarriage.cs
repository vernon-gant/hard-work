namespace TrainBuilder;

public class PassengerCarriage(int capacity) : BaseCarriage(capacity)
{
    private const char ConstMarker = 'P';

    public override char Marker => ConstMarker;
}