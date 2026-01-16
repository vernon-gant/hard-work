namespace MotorPool.TripGenerator;

public class Haversine : DistanceCalculator
{
    public double GetPointsDistance(Point p1, Point p2) => Calculate(p1.Latitude, p1.Longitude, p2.Latitude, p2.Longitude);

    public static double Calculate(double lat1, double lon1, double lat2, double lon2)
    {
        var R = 6376500; // radius of Earth in meters
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        lat1 = ToRadians(lat1);
        lat2 = ToRadians(lat2);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
        var c = 2 * Math.Asin(Math.Sqrt(a));

        return R * c;
    }

    private static double ToRadians(double angle) => Math.PI * angle / 180.0;
}