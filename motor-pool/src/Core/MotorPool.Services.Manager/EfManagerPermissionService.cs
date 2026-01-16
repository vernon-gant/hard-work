using Microsoft.EntityFrameworkCore;

using MotorPool.Persistence;

namespace MotorPool.Services.Manager;

public class EfManagerPermissionService(AppDbContext dbContext) : ManagerPermissionService
{

    public async ValueTask<bool> IsManagerAccessibleEnterprise(int managerId, int enterpriseId) =>
        await dbContext.EnterpriseManagers.AnyAsync(link => link.ManagerId == managerId && link.EnterpriseId == enterpriseId);

    public async ValueTask<bool> IsManagerAccessibleVehicle(int managerId, int vehicleId) => await dbContext.Vehicles.AnyAsync(
        vehicle => vehicle.VehicleId == vehicleId && vehicle.Enterprise != null &&
                   vehicle.Enterprise.ManagerLinks.Any(link => link.ManagerId == managerId));

    public async ValueTask<bool> IsManagerAccessibleDriver(int managerId, int driverId) =>
        await dbContext.Drivers.AnyAsync(driver => driver.DriverId == driverId && driver.Enterprise != null &&
                                                   driver.Enterprise.ManagerLinks.Any(link => link.ManagerId == managerId));

}