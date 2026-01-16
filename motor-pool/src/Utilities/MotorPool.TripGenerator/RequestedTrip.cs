namespace MotorPool.TripGenerator;

public class RequestedTrip
{
    public required double AverageSpeed_kmh { get; set; }
    public required Point StartPoint { get; set; }
    public List<Point> Hops { get; set; } = new();

    public required Point EndPoint { get; set; }

    public required DateTime StartTime { get; set; }

    public required int VehicleId { get; set; }

    public static RequestedTrip FromGraphHopperResponseWithOptions(GraphHopperResponse response, TripGenerationOptions generationOptions)
    {
        return new RequestedTrip
        {
            VehicleId = generationOptions.VehicleId,
            StartTime = generationOptions.StartTime,
            StartPoint = generationOptions.StartPoint,
            EndPoint = generationOptions.EndPoint,
            AverageSpeed_kmh = generationOptions.AverageSpeed_kmh,
            Hops = response.Paths[0]
                          .Points
                          .Coordinates
                          .Select(point => new Point(point[1], point[0]))
                          .ToList()
        };
    }

}