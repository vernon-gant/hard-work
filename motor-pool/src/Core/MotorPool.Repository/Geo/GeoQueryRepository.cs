using MotorPool.Domain;

namespace MotorPool.Repository.Geo;

public interface GeoQueryRepository
{
    ValueTask<List<Trip>> TripsWithStartAndEndPoints(int vehicleId, int limit);
    ValueTask<List<GeoPoint>> GetVehicleGeoPoints(int vehicleId, DateTime startTimeUtc, DateTime endTimeUtc);

    ValueTask<List<GeoPoint>> GetVehicleTripsInGeoPoints(int vehicleId, DateTime startTimeUtc, DateTime endTimeUtc);

    ValueTask<List<Trip>> GetVehicleTrips(int vehicleId, DateTime startTimeUtc, DateTime endTimeUtc);
}