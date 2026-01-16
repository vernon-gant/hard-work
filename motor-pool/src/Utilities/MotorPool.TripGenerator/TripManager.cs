using System.Collections.Concurrent;

using MotorPool.Domain;
using MotorPool.Persistence;

using OneOf;
using OneOf.Types;

namespace MotorPool.TripGenerator;

public class TripManager(DistanceCalculator distanceCalculator)
{
    private readonly ConcurrentBag<(Trip, List<GeoPoint>)> _generatedTrips = new();

    public bool Add(RequestedTrip requestedTrip, double speed_kmh)
    {
        if (requestedTrip.Hops.Count == 0) return false;

        List<GeoPoint> generatedRoute = [
            new()
            {
                Latitude = requestedTrip.StartPoint.Latitude,
                Longitude = requestedTrip.StartPoint.Longitude,
                VehicleId = requestedTrip.VehicleId,
                RecordedAt = requestedTrip.StartTime
            }
        ];

        var routeProducer = new RouteProducer(requestedTrip);

        while (!routeProducer.IsFinished().AsT0)
        {
            routeProducer.FindNext(speed_kmh, distanceCalculator);
            var currentGeoPoint = routeProducer.GetCurrentGeoPoint(generatedRoute.Last(), speed_kmh, distanceCalculator).AsT0;
            generatedRoute.Add(currentGeoPoint);
        }

        var readyTrip = new Trip
                        {
                            VehicleId = requestedTrip.VehicleId,
                            StartTime = requestedTrip.StartTime,
                            EndTime = generatedRoute.Last().RecordedAt
                        };

        _generatedTrips.Add((readyTrip, generatedRoute));

        return true;
    }

    public OneOf<Success, EmptyTrips, DatabaseError> Save(AppDbContext dbContext)
    {
        if (_generatedTrips.Count == 0) return new EmptyTrips();

        using var transaction = dbContext.Database.BeginTransaction();

        try
        {
            foreach (var (trip, geoPoints) in _generatedTrips)
            {
                if (SaveTrip(dbContext, trip, geoPoints)) continue;

                transaction.Rollback();
                return new DatabaseError();
            }

            transaction.Commit();

            _generatedTrips.Clear();

            return new Success();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Console.WriteLine($"An error occurred while generating the trip: {ex.Message}");

            return new DatabaseError();
        }
    }

    private bool SaveTrip(AppDbContext dbContext, Trip trip, List<GeoPoint> geoPoints)
    {
        try
        {
            dbContext.GeoPoints.AddRange(geoPoints);
            dbContext.SaveChanges();

            trip.StartGeoPointId = geoPoints.First().GeoPointId;
            trip.EndGeoPointId = geoPoints.Last().GeoPointId;
            dbContext.Trips.Add(trip);
            dbContext.SaveChanges();

            geoPoints.ForEach(point => point.TripId = trip.TripId);
            dbContext.SaveChanges();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while saving the trip: {ex}");

            return false;
        }
    }
}

public struct EmptyTrips;

public struct DatabaseError;