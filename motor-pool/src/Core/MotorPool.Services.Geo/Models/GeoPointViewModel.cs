namespace MotorPool.Services.Geo.Models;

public class GeoPointViewModel
{

    public PointViewModel Point { get; init; } = default!;

    public DateTime RecordedAt { get; init; }

    public int VehicleId { get; init; }

}