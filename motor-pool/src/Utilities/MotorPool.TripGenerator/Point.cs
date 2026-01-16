using System.Globalization;
using MotorPool.Domain;

namespace MotorPool.TripGenerator;

public struct Point(double latitude, double longitude)
{
    public double Latitude { get; set; } = latitude;

    public double Longitude { get; set; } = longitude;

    public static Point FromString(string pointString)
    {
        string[] coordinates = pointString.Split(',');
        return new Point(double.Parse(coordinates[0], NumberStyles.Any, CultureInfo.InvariantCulture), double.Parse(coordinates[1], NumberStyles.Any, CultureInfo.InvariantCulture));
    }

    public GeoPoint ToGeoPoint(DateTime recordedAt, int vehicleId)
    {
        return new GeoPoint
               {
                   Latitude = Latitude,
                   Longitude = Longitude,
                   VehicleId = vehicleId,
                   RecordedAt = recordedAt
               };
    }

    public override string ToString() => $"{Latitude.ToString(CultureInfo.InvariantCulture)},{Longitude.ToString(CultureInfo.InvariantCulture)}";
}