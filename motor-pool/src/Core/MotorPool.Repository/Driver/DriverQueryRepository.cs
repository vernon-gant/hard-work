using MotorPool.Domain;
using MotorPool.Persistence;
using MotorPool.Persistence.QueryObjects;

namespace MotorPool.Repository.Driver;

using Driver = Domain.Driver;

public interface DriverQueryRepository
{
    ValueTask<List<Driver>> GetAllAsync(int managerId);

    ValueTask<PagedResult<Driver>> GetAllAsync(int managerId, PageOptions pageOptions);

    ValueTask<Driver?> GetByIdAsync(int driverId);

    ValueTask<bool> AddVehicleAssignmentAsync(DriverVehicle driverVehicle);
}