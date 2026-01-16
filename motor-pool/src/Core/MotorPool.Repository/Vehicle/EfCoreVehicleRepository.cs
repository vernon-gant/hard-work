using Microsoft.EntityFrameworkCore;
using MotorPool.Persistence;
using MotorPool.Persistence.QueryObjects;

namespace MotorPool.Repository.Vehicle;

using Vehicle = Domain.Vehicle;

public class EfCoreVehicleQueryRepository(AppDbContext dbContext) : VehicleQueryRepository
{
    public async ValueTask<List<Vehicle>> GetAllAsync(VehicleQueryOptions? queryOptions) => await VehicleQueryBase().WithQueryOptions(queryOptions).ToListAsync();

    public async ValueTask<PagedResult<Vehicle>> GetAllAsync(PageOptions pageOptions, VehicleQueryOptions? queryOptions)
    {
        IQueryable<Vehicle> allVehiclesQuery = VehicleQueryBase().WithQueryOptions(queryOptions).OrderBy(vehicle => vehicle.VehicleId);

        int totalVehicles = await allVehiclesQuery.CountAsync();

        return PagedResult<Vehicle>.FromOptionsAndElements(pageOptions, await allVehiclesQuery.Page(pageOptions).ToListAsync(),totalVehicles);
    }

    public async ValueTask<Vehicle?> GetByIdAsync(int vehicleId) => await VehicleQueryBase().FirstOrDefaultAsync(vehicle => vehicle.VehicleId == vehicleId);

    public async ValueTask<bool> VINExists(string vin) => await dbContext.Vehicles.AnyAsync(vehicle => vehicle.MotorVIN == vin);

    private IQueryable<Vehicle> VehicleQueryBase() => dbContext.Vehicles
        .AsNoTracking()
        .Include(vehicle => vehicle.VehicleBrand)
        .Include(vehicle => vehicle.DriverVehicles)
        .Include(vehicle => vehicle.Enterprise)
        .Include(vehicle => vehicle.Enterprise!.ManagerLinks)
        .Include(vehicle => vehicle.Trips);
}