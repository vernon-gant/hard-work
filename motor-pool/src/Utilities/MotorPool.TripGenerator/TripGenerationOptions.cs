using System.Globalization;
using CommandLine;

namespace MotorPool.TripGenerator;

public class TripGenerationOptions
{
    // Single trip options
    private const double DefaultAverageSpeed_kmh = 60.0;
    private const int DefaultVehicleSampleSize = 100;
    private const int DefaultTripsPerVehicle = 20;

    private string _startPointString = string.Empty;

    [Option("isSingleTrip", SetName = "single-trip")]
    public string IsSingleTrip { get; set; } = string.Empty;

    [Option("start", SetName = "single-trip")]
    public string StartPointString
    {
        get => throw new InvalidOperationException();
        set => _startPointString = value;
    }

    public Point StartPoint => Point.FromString(_startPointString);

    private string _endPointString = string.Empty;

    [Option("end", SetName = "single-trip")]
    public string EndPointString
    {
        get => throw new InvalidOperationException();
        set => _endPointString = value;
    }

    public Point EndPoint => Point.FromString(_endPointString);

    [Option("averageSpeed", Default = DefaultAverageSpeed_kmh, SetName = "single-trip")]
    public double AverageSpeed_kmh { get; set; }

    [Option("vehicleId", SetName = "single-trip")]
    public int VehicleId { get; set; }

    public string _startTimeString = string.Empty;

    [Option("startTime", SetName = "single-trip")]
    public string StartTimeString
    {
        get => throw new InvalidOperationException("The use of this property is restricted");
        set => _startTimeString = value;
    }

    public DateTime StartTime => string.IsNullOrEmpty(_startTimeString) ? DateTime.UtcNow : DateTime.ParseExact(_startTimeString, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);

    public RequestedTrip RequestedTrip => new()
                                          {
                                              VehicleId = VehicleId,
                                              StartTime = StartTime,
                                              StartPoint = StartPoint,
                                              EndPoint = EndPoint,
                                              AverageSpeed_kmh = AverageSpeed_kmh
                                          };


    // Random trips options

    [Option("vehicleSampleSize", Required = false, Default = DefaultVehicleSampleSize, SetName = "random-trips")]
    public int VehicleSampleSize { get; set; }

    [Option("routesPerVehicle", Required = false, Default = DefaultTripsPerVehicle, SetName = "random-trips")]
    public int TripsPerVehicle { get; set; }

    [Option('c', "connectionString", Required = true)]
    public string ConnectionString { get; set; } = string.Empty;
}