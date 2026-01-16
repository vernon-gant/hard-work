namespace MotorPool.Domain;

public class Trip
{
    public int TripId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public int VehicleId { get; set; }

    public Vehicle? Vehicle { get; set; }

    public int? StartGeoPointId { get; set; }

    public GeoPoint? StartGeoPoint { get; set; }

    public int? EndGeoPointId { get; set; }

    public GeoPoint? EndGeoPoint { get; set; }

    public IEnumerable<GeoPoint> GeoPoints { get; set; }
}