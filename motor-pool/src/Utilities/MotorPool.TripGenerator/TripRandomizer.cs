using MotorPool.Persistence;

using OneOf;

namespace MotorPool.TripGenerator;

public class TripRandomizer
{
    private readonly Random _random = new ();

    private static readonly List<string> _startPoints =
    [
        "49.781416, 9.936012", "50.086221, 8.642882", "48.801742, 9.167541", "51.322897, 12.463660", "52.478276, 13.437707",
        "52.405567, 9.720241", "51.002344, 13.765620", "50.077207, 14.356108", "48.958492, 14.462011", "49.179757, 16.583639"
    ];
    private static readonly List<string> _endPoints =
    [
        "47.282894, 11.462460", "46.826717, 12.785644", "46.621385, 14.314340", "47.054181, 15.451398", "47.751344, 16.042540",
        "48.141479, 16.410762", "48.185339, 17.138477", "48.264925, 14.257803"
    ];

    private const int MinimumAverageSpeed_kmh = 60;
    private const int MaximumAverageSpeed_kmh = 90;

    private const int MaxSubtractDays = 730;
    private const int MinSubtractHours = 24;
    private const int MaxSubtractMinutes = 60;

    public OneOf<List<int>, DatabaseError> VehicleIdsSample(int sampleSize, AppDbContext dbContext)
    {
        try
        {
            var existingVehicleIds = dbContext.Vehicles.Select(v => v.VehicleId).ToList();

            return Enumerable.Repeat(0, sampleSize).Select(_ => existingVehicleIds[_random.Next(existingVehicleIds.Count)]).ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error while getting sample of vehicle ids: {e.Message}");

            return new DatabaseError();
        }
    }

    public DateTime RandomStartTime
    {
        get
        {
            var subtractSpan = TimeSpan.FromDays(_random.Next(MaxSubtractDays));
            subtractSpan = subtractSpan.Subtract(TimeSpan.FromHours(_random.Next(MinSubtractHours)));
            subtractSpan = subtractSpan.Subtract(TimeSpan.FromMinutes(_random.Next(MaxSubtractMinutes)));

            return DateTime.UtcNow.Subtract(subtractSpan);
        }
    }

    public Point RandomStartPoint => Point.FromString(_startPoints[_random.Next(_startPoints.Count)]);

    public Point RandomEndPoint => Point.FromString(_endPoints[_random.Next(_endPoints.Count)]);

    public int RandomAverageSpeed_kmh => _random.Next(MinimumAverageSpeed_kmh, MaximumAverageSpeed_kmh);
}