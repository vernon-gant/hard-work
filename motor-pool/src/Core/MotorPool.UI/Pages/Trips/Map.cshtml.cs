using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

using MotorPool.Persistence;
using MotorPool.Services.Geo.Models;
using MotorPool.Services.Geo.Services;

namespace MotorPool.UI.Pages.Trips;

public class MapModel(TripQueryService tripQueryService, AppDbContext dbContext) : PageModel
{
    [BindProperty]
    public int[] SelectedTrips { get; set; } = [];

    public List<(TripViewModel, List<GeoPointViewModel>)> TripsWithRoutes { get; set; } = [];

    public async Task OnPostAsync()
    {
        var vehicleIdToTrips = dbContext.Trips.Include(trip => trip.Vehicle).Where(trip => SelectedTrips.Contains(trip.TripId)).GroupBy(trip => trip.VehicleId).ToDictionary(group => group.Key, group => group.ToList());

        foreach (var IdToTrips in vehicleIdToTrips)
        {
            var tripIds = IdToTrips.Value.Select(trip => trip.TripId).ToList();
            var tripWithRoutes = await tripQueryService.GetTripsWithRoutes(IdToTrips.Key, tripIds);
            TripsWithRoutes.AddRange(tripWithRoutes);
        }
    }
}