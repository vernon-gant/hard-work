using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MotorPool.Domain;
using MotorPool.Persistence;
using MotorPool.Persistence.QueryObjects;
using MotorPool.Repository.Manager;

namespace MotorPool.Repository.Driver;

using Driver = Domain.Driver;

public class EfCoreDriverQueryRepository(AppDbContext dbContext, ILogger<EfCoreDriverQueryRepository> logger) : DriverQueryRepository
{
    public async ValueTask<List<Driver>> GetAllAsync(int managerId) => await DriversWithIncludesQuery().ForManager(managerId).ToListAsync();

    public async ValueTask<PagedResult<Driver>> GetAllAsync(int managerId, PageOptions pageOptions)
    {
        IQueryable<Driver> managerDriversQuery = DriversWithIncludesQuery().ForManager(managerId);

        int totalManagerDrivesCount = await managerDriversQuery.CountAsync();

        List<Driver> managerDrivers = await managerDriversQuery
            .Page(pageOptions)
            .OrderBy(driver => driver.DriverId)
            .ToListAsync();

        return PagedResult<Driver>.FromOptionsAndElements(pageOptions, managerDrivers, totalManagerDrivesCount);
    }

    public async ValueTask<Driver?> GetByIdAsync(int driverId) => await DriversWithIncludesQuery().FirstOrDefaultAsync(driver => driver.DriverId == driverId);
    public async ValueTask<bool> AddVehicleAssignmentAsync(DriverVehicle driverVehicle)
    {
        try
        {
            dbContext.Set<DriverVehicle>().Add(driverVehicle);
            await dbContext.SaveChangesAsync();
            return true;
        } catch (Exception e)
        {
            logger.LogError(e, "Failed to add vehicle assignment {@DriverVehicle} for driver {DriverId}", driverVehicle, driverVehicle.DriverId);
            return false;
        }
    }

    private IQueryable<Driver> DriversWithIncludesQuery() => dbContext.Drivers
                                                                      .AsNoTracking()
                                                                      .Include(driver => driver.DriverVehicles)
                                                                      .Include(driver => driver.Enterprise)
                                                                      .Include(driver => driver.Enterprise!.ManagerLinks);
}