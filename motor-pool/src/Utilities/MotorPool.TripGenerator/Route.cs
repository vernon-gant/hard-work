namespace MotorPool.TripGenerator;

using OneOf;
using OneOf.Types;

public class Route
{
    private readonly IReadOnlyList<Point> _routePoints;
    private int _currentHopIndex;

    public Route(IReadOnlyList<Point> routePoints)
    {
        if (routePoints.Count == 0) throw new InvalidOperationException("Route must contain at least two points");

        _routePoints = routePoints;
        _currentHopIndex = 0;
    }

    public bool IsInitialized => _routePoints.Count > 0;

    public bool IsFinished => _currentHopIndex >= _routePoints.Count - 1;

    public OneOf<Point, RouteNotInitialized> CurrentPoint => IsInitialized ? _routePoints[_currentHopIndex] : new RouteNotInitialized();

    public OneOf<Success, RouteFinished, RouteNotInitialized> AdvanceToNextHop(DistanceCalculator distanceCalculator, double minimalDrivingTime, double speedMps)
    {
        if (_routePoints.Count == 0) return new RouteNotInitialized();

        if (IsFinished) return new RouteFinished();

        for (int i = _currentHopIndex + 1; i < _routePoints.Count; i++)
        {
            var distance = distanceCalculator.GetPointsDistance(_routePoints[_currentHopIndex], _routePoints[i]);

            if (!(distance >= minimalDrivingTime * speedMps) && i < _routePoints.Count - 1) continue;

            _currentHopIndex = i;

            return new Success();
        }

        return new RouteFinished();
    }
}

public struct RouteNotInitialized;

public struct RouteFinished;