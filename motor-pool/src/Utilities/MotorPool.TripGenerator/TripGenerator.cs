using MotorPool.Persistence;

using OneOf;
using OneOf.Types;

namespace MotorPool.TripGenerator;

public class TripGenerator(AppDbContext appDbContext, TripManager manager, GraphHopperClient graphHopperClient)
{
    public OneOf<Success, GraphHopperError, DatabaseError> GenerateSingleTrip(RequestedTrip requestedTrip)
    {
        var routeResult = graphHopperClient.GetRouteAsync(requestedTrip.StartPoint, requestedTrip.EndPoint).Result;

        if (routeResult.IsT1) return new GraphHopperError();

        requestedTrip.Hops = routeResult.AsT0;

        manager.Add(requestedTrip, requestedTrip.AverageSpeed_kmh);

        var saveResult = manager.Save(appDbContext);

        return saveResult.IsT0 ? new Success() : saveResult.AsT2;
    }
    public async Task<OneOf<Success, GraphHopperError, DatabaseError>> GenerateRandomTripsAsync(TripRandomizer tripRandomizer, TripGenerationOptions options)
    {
        var loadingStatus = tripRandomizer.VehicleIdsSample(options.VehicleSampleSize, appDbContext);

        if (loadingStatus.IsT1) return loadingStatus.AsT1;

        var vehicleIds = loadingStatus.AsT0;

        var generatedTrips = vehicleIds.SelectMany(vehicleId => Enumerable.Range(0, options.TripsPerVehicle)
                                                                          .Select(_ => new RequestedTrip
                                                                                       {
                                                                                           VehicleId = vehicleId,
                                                                                           StartTime = tripRandomizer.RandomStartTime,
                                                                                           StartPoint = tripRandomizer.RandomStartPoint,
                                                                                           EndPoint = tripRandomizer.RandomEndPoint,
                                                                                           AverageSpeed_kmh = tripRandomizer.RandomAverageSpeed_kmh
                                                                                       }))
                                       .ToList();

        var routeTasks = generatedTrips.Select(async generatedTrip =>
        {
            var routeResult = await graphHopperClient.GetRouteAsync(generatedTrip.StartPoint, generatedTrip.EndPoint);

            if (routeResult.IsT1) return new GraphHopperError();

            var route = routeResult.AsT0;

            if (route.Count == 0) return new GraphHopperError();

            generatedTrip.Hops = route;

            manager.Add(generatedTrip, generatedTrip.AverageSpeed_kmh);

            return (object?)null;
        });

        await Task.WhenAll(routeTasks);

        var saveResult = manager.Save(appDbContext);

        return saveResult.IsT0 ? new Success() : saveResult.AsT2;
    }
}

public struct GraphHopperError;