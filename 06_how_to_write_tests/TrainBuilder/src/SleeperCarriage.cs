namespace TrainBuilder;

public class SleeperCarriage(int capacity) : BaseCarriage(capacity)
{
    private const char ConstMarker = 'S';

    public override char Marker => ConstMarker;
}