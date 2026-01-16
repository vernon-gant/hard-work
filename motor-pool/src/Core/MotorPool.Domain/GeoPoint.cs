using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace MotorPool.Domain;

public class GeoPoint
{
    [Key]
    public int GeoPointId { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public int VehicleId { get; set; }

    public Vehicle? Vehicle { get; set; }

    public int? TripId { get; set; }

    public Trip? Trip { get; set; }

    public DateTime RecordedAt { get; set; }

    public string Coordinates => $"{Latitude.ToString("F6", CultureInfo.InvariantCulture)},{Longitude.ToString("F6", CultureInfo.InvariantCulture)}";

    public override string ToString() => $"GeoPointId: {GeoPointId}, Latitude: {Latitude}, Longitude: {Longitude}, VehicleId: {VehicleId}, RecordedAt: {RecordedAt}";
}