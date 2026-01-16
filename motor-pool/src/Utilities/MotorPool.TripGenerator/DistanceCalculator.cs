namespace MotorPool.TripGenerator;

public interface DistanceCalculator
{
    double GetPointsDistance(Point p1, Point p2);
}