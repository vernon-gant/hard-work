using MotorPool.Persistence;
using MotorPool.Persistence.QueryObjects;

namespace MotorPool.Repository.Vehicle;

using Vehicle = Domain.Vehicle;

public interface VehicleQueryRepository
{
    ValueTask<List<Vehicle>> GetAllAsync(VehicleQueryOptions? queryOptions = null);

    ValueTask<PagedResult<Vehicle>> GetAllAsync(PageOptions pageOptions, VehicleQueryOptions? queryOptions = null);

    ValueTask<Vehicle?> GetByIdAsync(int vehicleId);

    ValueTask<bool> VINExists(string vin);
}