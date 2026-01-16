using AutoMapper;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using MotorPool.Domain;
using MotorPool.Persistence;
using MotorPool.Persistence.QueryObjects;
using MotorPool.Repository.Geo;
using MotorPool.Services.Geo.GraphHopper;
using MotorPool.Services.Geo.Models;

namespace MotorPool.Services.Geo.Services.Concrete;

public class DefaultTripQueryService(AppDbContext dbContext, IMapper mapper, GraphHopperClient graphHopperClient, ILogger<DefaultTripQueryService> logger, GeoQueryRepository queryRepository) : TripQueryService
{
    public async ValueTask<IEnumerable<GeoPointViewModel>> GetVehicleGeoPoints(int vehicleId, DateTime startTime, DateTime endTime)
    {
        if (startTime > endTime) throw new ArgumentException("Start date cannot be greater than end date");

        TimeZoneInfo enterpriseTimeZone = dbContext.Vehicles.GetVehicleTimeZoneInfo(vehicleId);

        var test = await dbContext.Vehicles
                                  .Include(vehicle => vehicle.GeoPoints)
                                  .Take(10)
                                  .Select(vehicle => new
                                                     {
                                                         Vehicle = vehicle,
                                                         GeoPoints = vehicle.GeoPoints.Select(geoPoint => geoPoint.GeoPointId).Take(10)
                                                     })
                                  .ToListAsync();

        List<GeoPoint> rawGeoPoints = await queryRepository.GetVehicleGeoPoints(vehicleId, TimeZoneInfo.ConvertTimeToUtc(startTime, enterpriseTimeZone), TimeZoneInfo.ConvertTimeToUtc(endTime, enterpriseTimeZone));

        logger.LogInformation("Returned geopoints for vehicle {VehicleId}", vehicleId);

        return rawGeoPoints.Select(geoPoint => mapper.Map<GeoPointViewModel>(geoPoint, options => { options.Items["EnterpriseTimeZone"] = enterpriseTimeZone; }));
    }

    public async ValueTask<IEnumerable<GeoPointViewModel>> GetVehicleTripsInGeoPoints(int vehicleId, DateTime startTime, DateTime endTime)
    {
        if (startTime > endTime) throw new ArgumentException("Start date cannot be greater than end date");

        TimeZoneInfo enterpriseTimeZone = dbContext.Vehicles.GetVehicleTimeZoneInfo(vehicleId);

        List<GeoPoint> rawGeoPoints = await queryRepository.GetVehicleTripsInGeoPoints(vehicleId, TimeZoneInfo.ConvertTimeToUtc(startTime, enterpriseTimeZone), TimeZoneInfo.ConvertTimeToUtc(endTime, enterpriseTimeZone));

        logger.LogInformation("Returned trips in geopoints for vehicle {VehicleId}", vehicleId);

        return rawGeoPoints.Select(geoPoint => mapper.Map<GeoPointViewModel>(geoPoint, options => { options.Items["EnterpriseTimeZone"] = enterpriseTimeZone; }));
    }

    // N + 1 problem
    // public async ValueTask<IEnumerable<TripViewModel>> GetVehicleTrips(int vehicleId, DateTime startTime, DateTime endTime)
    // {
    //     TimeZoneInfo enterpriseTimeZone = dbContext.Vehicles.GetVehicleTimeZoneInfo(vehicleId);
    //
    //     List<Trip> vehicleTrips = await dbContext.Trips.Where(trip => trip.StartTime >= TimeZoneInfo.ConvertTimeToUtc(startTime, enterpriseTimeZone) &&
    //                                                                   trip.EndTime <= TimeZoneInfo.ConvertTimeToUtc(endTime, enterpriseTimeZone) && trip.VehicleId == vehicleId)
    //                                              .OrderBy(trip => trip.StartTime)
    //                                              .ToListAsync();
    //
    //     List<GeoPoint> allTripsGeoPoints = vehicleTrips.SelectMany(trip => dbContext
    //                                                                       .GeoPoints.Where(geoPoint => geoPoint.VehicleId == trip.VehicleId && geoPoint.RecordedAt >= trip.StartTime && geoPoint.RecordedAt <= trip.EndTime)
    //                                                                       .OrderBy(geoPoint => geoPoint.RecordedAt))
    //                                                    .ToList();
    //
    //     List<Task<TripViewModel>> tripToViewModelTasks = vehicleTrips.Select(async trip =>
    //                                                                   {
    //                                                                       List<GeoPoint> tripGeoPoints = allTripsGeoPoints.Where(geoPoint => geoPoint.RecordedAt >= trip.StartTime && geoPoint.RecordedAt <= trip.EndTime)
    //                                                                                                                       .ToList();
    //                                                                       GeoPoint startPoint = tripGeoPoints.First();
    //                                                                       GeoPoint endPoint = tripGeoPoints.Last();
    //
    //                                                                       return await TripToViewModel(trip, enterpriseTimeZone, startPoint, endPoint);
    //                                                                   })
    //                                                                  .ToList();
    //
    //     logger.LogInformation("Returned trips for vehicle {VehicleId}", vehicleId);
    //     return await Task.WhenAll(tripToViewModelTasks);
    // }

    public async ValueTask<IEnumerable<TripViewModel>> GetVehicleTrips(int vehicleId, DateTime startTime, DateTime endTime)
    {
        TimeZoneInfo enterpriseTimeZone = dbContext.Vehicles.GetVehicleTimeZoneInfo(vehicleId);

        List<Trip> vehicleTrips = await queryRepository.GetVehicleTrips(vehicleId, TimeZoneInfo.ConvertTimeToUtc(startTime, enterpriseTimeZone), TimeZoneInfo.ConvertTimeToUtc(endTime, enterpriseTimeZone));

        List<Task<TripViewModel>> tripToViewModelTasks = vehicleTrips.Select(async trip => await TripToViewModel(trip, enterpriseTimeZone, trip.StartGeoPoint!, trip.EndGeoPoint!)).ToList();

        logger.LogInformation("Returned trips for vehicle {VehicleId}", vehicleId);

        return await Task.WhenAll(tripToViewModelTasks);
    }

    public async ValueTask<IEnumerable<(TripViewModel, List<GeoPointViewModel>)>> GetTripsWithRoutes(int vehicleId, IEnumerable<int> tripIds)
    {
        TimeZoneInfo enterpriseTimeZone = dbContext.Vehicles.GetVehicleTimeZoneInfo(vehicleId);

        List<Trip> vehicleTrips = await dbContext.Trips.Where(trip => tripIds.Contains(trip.TripId)).ToListAsync();

        List<(Trip trip, List<GeoPoint>)> tripsWithGeoPoints = vehicleTrips
                                                              .Select(trip => (
                                                                          trip,
                                                                          dbContext.GeoPoints
                                                                                   .Include(geoPoint => geoPoint.Vehicle)
                                                                                   .Include(geoPoint => geoPoint.Vehicle!.Enterprise)
                                                                                   .Where(geoPoint => geoPoint.RecordedAt >= trip.StartTime && geoPoint.RecordedAt <= trip.EndTime && geoPoint.VehicleId == trip.VehicleId)
                                                                                   .OrderBy(geoPoint => geoPoint.RecordedAt)
                                                                                   .ToList()))
                                                              .ToList();

        var tripToViewModelTasks = tripsWithGeoPoints.Select(async tripWithGeoPoints =>
                                                      {
                                                          GeoPoint startPoint = tripWithGeoPoints.Item2.First();
                                                          GeoPoint endPoint = tripWithGeoPoints.Item2.Last();

                                                          List<GeoPointViewModel> geoPointViewModels = mapper.Map<List<GeoPointViewModel>>(tripWithGeoPoints.Item2,
                                                                                                                                           options => { options.Items["EnterpriseTimeZone"] = enterpriseTimeZone; });

                                                          TripViewModel tripViewModel = await TripToViewModel(tripWithGeoPoints.trip, enterpriseTimeZone, startPoint, endPoint);

                                                          return (tripViewModel, geoPointViewModels);
                                                      })
                                                     .ToList();

        return await Task.WhenAll(tripToViewModelTasks);
    }
    public async ValueTask<Dictionary<int, List<TripViewModel>>> GetEnterpriseTrips(List<Enterprise> enterprises, DateTime startTime, DateTime endTime)
    {
        var enterpriseTimeZones = enterprises.ToDictionary(e => e.EnterpriseId, e => TimeZoneInfo.FindSystemTimeZoneById(e.TimeZoneId));

        var enterpriseIds = enterprises.Select(e => e.EnterpriseId).ToList();

        var trips = await dbContext.Trips
                                   .Include(trip => trip.Vehicle)
                                   .Where(trip => enterpriseIds.Contains(trip.Vehicle!.EnterpriseId) && trip.StartTime >= TimeZoneInfo.ConvertTimeToUtc(startTime, TimeZoneInfo.Utc) &&
                                                  trip.EndTime <= TimeZoneInfo.ConvertTimeToUtc(endTime, TimeZoneInfo.Utc))
                                   .OrderBy(trip => trip.StartTime)
                                   .ToListAsync();

        var tripsByEnterprise = trips.GroupBy(trip => trip.Vehicle!.EnterpriseId);

        var result = new Dictionary<int, List<TripViewModel>>();

        foreach (var group in tripsByEnterprise)
        {
            var enterpriseId = group.Key;

            if (!enterpriseTimeZones.TryGetValue(enterpriseId, out var timeZone)) continue;

            var tripViewModels = new List<TripViewModel>();

            foreach (var trip in group)
            {
                GeoPoint startPoint = await dbContext.GeoPoints.Where(geoPoint => geoPoint.VehicleId == trip.VehicleId && geoPoint.RecordedAt == trip.StartTime).FirstAsync();

                GeoPoint endPoint = await dbContext.GeoPoints.Where(geoPoint => geoPoint.VehicleId == trip.VehicleId && geoPoint.RecordedAt == trip.EndTime).FirstAsync();

                var tripViewModel = await TripToViewModel(trip, timeZone, startPoint, endPoint);
                tripViewModels.Add(tripViewModel);
            }

            result[enterpriseId] = tripViewModels;
        }

        logger.LogInformation("Returned trips for enterprises: {EnterpriseIds}", string.Join(", ", enterpriseIds));

        return result;
    }


    private async ValueTask<TripViewModel> TripToViewModel(Trip trip, TimeZoneInfo enterpriseTimeZone, GeoPoint startPoint, GeoPoint endPoint)
    {
        TripViewModel tripViewModel = mapper.Map<TripViewModel>(trip, options => { options.Items["EnterpriseTimeZone"] = enterpriseTimeZone; });

        tripViewModel.StartPoint = new () { LatitudeDouble = startPoint.Latitude, LongitudeDouble = startPoint.Longitude };
        tripViewModel.StartPointDescription = await graphHopperClient.GetReverseGeocodingAsync(startPoint);
        tripViewModel.EndPoint = new () { LatitudeDouble = endPoint.Latitude, LongitudeDouble = endPoint.Longitude };
        tripViewModel.EndPointDescription = await graphHopperClient.GetReverseGeocodingAsync(endPoint);

        logger.LogDebug("Mapped trip {@Trip} to full view model {@TripViewModel}", trip, tripViewModel);

        return tripViewModel;
    }
}