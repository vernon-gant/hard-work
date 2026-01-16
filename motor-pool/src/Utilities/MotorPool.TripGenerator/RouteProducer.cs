using OneOf;
using OneOf.Types;

using GeoPoint = MotorPool.Domain.GeoPoint;

namespace MotorPool.TripGenerator;

public class RouteProducer
{
    private readonly Route _route;

    private const double MINIMAL_DRIVING_TIME_s = 8;

    public RouteProducer(RequestedTrip requestedTrip)
    {
        var points = new List<Point> { requestedTrip.StartPoint }
                     .Concat(requestedTrip.Hops)
                     .Concat(new List<Point> { requestedTrip.EndPoint })
                     .ToList();

        if (points.Count < 2) throw new InvalidOperationException("Route must contain at least two points");

        _route = new Route(points);
    }

    public OneOf<bool, RouteNotInitialized> IsFinished() => _route.IsInitialized ? _route.IsFinished : new RouteNotInitialized();

    public OneOf<Success, RouteFinished, RouteNotInitialized> FindNext(double speedKmh, DistanceCalculator distanceCalculator) => _route.AdvanceToNextHop(distanceCalculator, SpeedMps(speedKmh), MINIMAL_DRIVING_TIME_s);

    public OneOf<GeoPoint, RouteNotInitialized> GetCurrentGeoPoint(GeoPoint previousGeoPoint, double speedKmh, DistanceCalculator distanceCalculator)
    {
        if (!_route.IsInitialized) return new RouteNotInitialized();

        var currentPoint = _route.CurrentPoint.AsT0;
        var distance = distanceCalculator.GetPointsDistance(new Point(previousGeoPoint.Latitude, previousGeoPoint.Longitude), currentPoint);
        var time = distance / SpeedMps(speedKmh);

        return new GeoPoint
        {
            Latitude = currentPoint.Latitude,
            Longitude = currentPoint.Longitude,
            VehicleId = previousGeoPoint.VehicleId,
            RecordedAt = previousGeoPoint.RecordedAt.AddSeconds(time)
        };
    }

    private double SpeedMps(double speedKmh) => speedKmh / 3.6;
}