using Microsoft.EntityFrameworkCore;
using MotorPool.Domain;
using MotorPool.Domain.Reports;
using MotorPool.Persistence;
using MotorPool.Services.Geo.GraphHopper;

namespace MotorPool.Services.Reporting.Core;

public class VehicleMileageReportGenerator(AppDbContext dbContext, GraphHopperClient graphHopperClient) : ReportGenerator<VehicleMileageReport>
{
    public async ValueTask GenerateByDay(VehicleMileageReport report)
    {
        for (DateOnly currentDate = report.StartTime; currentDate <= report.EndTime; currentDate = currentDate.AddDays(1))
        {
            GeoPoint? start = await dbContext.GeoPoints
                                             .OrderBy(geoPoint => geoPoint.RecordedAt)
                                             .FirstOrDefaultAsync(geoPoint => geoPoint.RecordedAt.Day == currentDate.Day && geoPoint.RecordedAt.Month == currentDate.Month &&
                                                                              geoPoint.RecordedAt.Year == currentDate.Year && geoPoint.VehicleId == report.VehicleId);

            if (start is null) continue;

            GeoPoint end = await dbContext.GeoPoints
                                          .OrderBy(geoPoint => geoPoint.RecordedAt)
                                          .LastAsync(geoPoint => geoPoint.RecordedAt.Day == currentDate.Day && geoPoint.RecordedAt.Month == currentDate.Month &&
                                                                 geoPoint.RecordedAt.Year == currentDate.Year);

            decimal distance_km = await graphHopperClient.GetDistanceAsync_m(start, end) / 1000;

            report.Result[currentDate.ToShortDateString()] = $"{distance_km:F2} km";
        }
    }

    public async ValueTask GenerateByMonth(VehicleMileageReport report)
    {
        const string MONTH_FORMAT = "MM/yyyy";

        for (DateTime currentDate = report.StartTime.ToDateTime(TimeOnly.MinValue);
             currentDate <= report.EndTime.ToDateTime(TimeOnly.MinValue);
             currentDate = currentDate.AddMonths(1))
        {
            List<Trip> monthTrips = await dbContext.Trips
                                                   .Where(trip => ((trip.StartTime.Month == currentDate.Month && trip.StartTime.Year == currentDate.Year) ||
                                                                   (trip.EndTime.Month == currentDate.Month && trip.EndTime.Year == currentDate.Year)) &&
                                                                  trip.VehicleId == report.VehicleId)
                                                   .ToListAsync();

            if (monthTrips.Count == 0) continue;

            List<(Trip, List<GeoPoint>)> monthTripsWithGeoPoints = monthTrips.Select(trip => (trip, dbContext.GeoPoints
                                                                                                        .Where(geoPoint => geoPoint.RecordedAt >= trip.StartTime &&
                                                                                                                   geoPoint.RecordedAt <= trip.EndTime &&
                                                                                                                   geoPoint.RecordedAt.Month == currentDate.Month &&
                                                                                                                   geoPoint.RecordedAt.Year == currentDate.Year)
                                                                                                        .ToList()))
                                                                        .ToList();

            decimal distance_km = await GetMileage_km(monthTripsWithGeoPoints);

            report.Result[currentDate.ToString(MONTH_FORMAT)] = $"{distance_km:F2} km";
        }
    }

    public async ValueTask GenerateByYear(VehicleMileageReport report)
    {
        for (int currentYear = report.StartTime.Year; currentYear <= report.EndTime.Year; currentYear++)
        {
            List<Trip> yearTrips = await dbContext.Trips.Where(trip => (trip.StartTime.Year == currentYear || trip.EndTime.Month == currentYear) && trip.VehicleId == report.VehicleId).ToListAsync();

            if (yearTrips.Count == 0) continue;

            List<(Trip, List<GeoPoint>)> yearTripsWithGeoPoints = yearTrips.Select(trip => (trip, dbContext.GeoPoints
                                                                                                       .Where(geoPoint => geoPoint.RecordedAt >= trip.StartTime &&
                                                                                                                  geoPoint.RecordedAt <= trip.EndTime &&
                                                                                                                  geoPoint.RecordedAt.Year == currentYear)
                                                                                                       .ToList()))
                                                                       .ToList();

            decimal distance_km = await GetMileage_km(yearTripsWithGeoPoints);

            report.Result[currentYear.ToString()] = $"{distance_km:F2} km";
        }
    }

    private async ValueTask<decimal> GetMileage_km(List<(Trip, List<GeoPoint>)> tripsWithGeoPoints)
    {
        IEnumerable<Task<decimal>> distanceTasks = tripsWithGeoPoints.Select(tripWithGeoPoint => (tripWithGeoPoint.Item2.First(), tripWithGeoPoint.Item2.Last()))
                                                                     .Select(async pair => await graphHopperClient.GetDistanceAsync_m(pair.Item1, pair.Item2) / 1000);

        IEnumerable<decimal> distance_results = await Task.WhenAll(distanceTasks);

        return distance_results.Sum();
    }
}