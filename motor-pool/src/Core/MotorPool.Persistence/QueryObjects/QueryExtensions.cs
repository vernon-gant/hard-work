using Microsoft.EntityFrameworkCore;

using MotorPool.Domain;

namespace MotorPool.Persistence.QueryObjects;

public static class QueryExtensions
{

    public static TimeZoneInfo GetVehicleTimeZoneInfo(this DbSet<Vehicle> vehicles, int vehicleId)
    {
        Enterprise vehicleEnterprise = vehicles
                                       .Include(vehicle => vehicle.Enterprise)
                                       .Where(vehicle => vehicle.VehicleId == vehicleId)
                                       .Select(vehicle => vehicle.Enterprise!)
                                       .First();

        return TimeZoneInfo.FindSystemTimeZoneById(vehicleEnterprise.TimeZoneId);
    }

}