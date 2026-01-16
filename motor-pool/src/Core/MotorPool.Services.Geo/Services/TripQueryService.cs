using MotorPool.Domain;
using MotorPool.Services.Geo.Models;

namespace MotorPool.Services.Geo.Services;

public interface TripQueryService
{
    ValueTask<IEnumerable<GeoPointViewModel>> GetVehicleGeoPoints(int vehicleId, DateTime startTime, DateTime endTime);

    ValueTask<IEnumerable<GeoPointViewModel>> GetVehicleTripsInGeoPoints(int vehicleId, DateTime startTime, DateTime endTime);

    ValueTask<IEnumerable<TripViewModel>> GetVehicleTrips(int vehicleId, DateTime startTime, DateTime endTime);

    ValueTask<IEnumerable<(TripViewModel, List<GeoPointViewModel>)>> GetTripsWithRoutes(int vehicleId, IEnumerable<int> tripIds);

    ValueTask<Dictionary<int, List<TripViewModel>>> GetEnterpriseTrips(List<Enterprise> enterprises, DateTime startTime, DateTime endTime);
}