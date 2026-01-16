using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MotorPool.Domain;
using MotorPool.Persistence;

namespace MotorPool.Repository.Geo;

public class EfCoreGeoRepository(AppDbContext dbContext, ILogger<EfCoreGeoRepository> logger) : GeoQueryRepository
{
    // N + 1 problem
    // public async ValueTask<List<(Trip, GeoPoint, GeoPoint)>> TripsWithStartAndEndPoints(int vehicleId, int limit)
    // {
    //     try
    //     {
    //         List<Trip> trips = await dbContext.Trips.ToListAsync();
    //
    //         List<(Trip, GeoPoint, GeoPoint)> tripsWithStartAndEndPoints = new();
    //
    //         foreach (Trip trip in trips)
    //         {
    //             GeoPoint startGeoPoint = await dbContext.GeoPoints.FirstOrDefaultAsync(geoPoint => geoPoint.RecordedAt >= trip.StartTime);
    //             GeoPoint endGeoPoint = await dbContext.GeoPoints.FirstOrDefaultAsync(geoPoint => geoPoint.RecordedAt <= trip.EndTime);
    //             tripsWithStartAndEndPoints.Add((trip, startGeoPoint, endGeoPoint));
    //          }
    //
    //         return tripsWithStartAndEndPoints;
    //     catch (Exception e)
    //     {
    //         logger.LogError(e, "Error getting trips with start and end points for vehicle {VehicleId}", vehicleId);
    //         throw;
    //     }
    // }
    public async ValueTask<List<Trip>> TripsWithStartAndEndPoints(int vehicleId, int limit)
    {
        try
        {
            return await dbContext.Trips
                                  .Include(trip => trip.StartGeoPoint)
                                  .Include(trip => trip.EndGeoPoint)
                                  .Take(limit)
                                  .ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting trips with start and end points for vehicle {VehicleId}", vehicleId);
            throw;
        }
    }

    public async ValueTask<List<GeoPoint>> GetVehicleGeoPoints(int vehicleId, DateTime startTimeUtc, DateTime endTimeUtc)
    {
        try
        {
            return await dbContext.GeoPoints.Include(geoPoint => geoPoint.Vehicle)
                                  .Include(geoPoint => geoPoint.Vehicle!.Enterprise)
                                  .Where(geoPoint => geoPoint.RecordedAt > startTimeUtc && geoPoint.RecordedAt < endTimeUtc)
                                  .ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting geopoints for vehicle {VehicleId}", vehicleId);
            throw;
        }
    }

    public async ValueTask<List<GeoPoint>> GetVehicleTripsInGeoPoints(int vehicleId, DateTime startTimeUtc, DateTime endTimeUtc)
    {
        try
        {
            return await dbContext.Trips.Where(trip => trip.StartTime >= startTimeUtc && trip.EndTime <= endTimeUtc && trip.VehicleId == vehicleId)
                                  .OrderBy(trip => trip.StartTime)
                                  .SelectMany(trip => dbContext.GeoPoints.Include(geoPoint => geoPoint.Vehicle)
                                                               .Include(geoPoint => geoPoint.Vehicle!.Enterprise)
                                                               .Where(geoPoint => geoPoint.VehicleId == trip.VehicleId && geoPoint.RecordedAt >= trip.StartTime && geoPoint.RecordedAt <= trip.EndTime)
                                                               .OrderBy(geoPoint => geoPoint.RecordedAt))
                                  .ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting trips in geopoints for vehicle {VehicleId}", vehicleId);
            throw;
        }
    }

    public async ValueTask<List<Trip>> GetVehicleTrips(int vehicleId, DateTime startTimeUtc, DateTime endTimeUtc)
    {
        try
        {
            return await dbContext.Trips
                                  .Include(trip => trip.GeoPoints)
                                  .Include(trip => trip.StartGeoPoint)
                                  .Include(trip => trip.EndGeoPoint)
                                  .Where(trip => trip.StartTime >= startTimeUtc && trip.EndTime <= endTimeUtc && trip.VehicleId == vehicleId)
                                  .OrderBy(trip => trip.StartTime)
                                  .ToListAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error getting trips for vehicle {VehicleId}", vehicleId);
            throw;
        }
    }
}