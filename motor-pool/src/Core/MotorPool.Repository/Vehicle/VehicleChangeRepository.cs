namespace MotorPool.Repository.Vehicle;

using Vehicle = Domain.Vehicle;

public interface VehicleChangeRepository
{
    ValueTask<Vehicle> CreateAsync(Vehicle newVehicle);

    ValueTask UpdateAsync(Vehicle updatedVehicle);

    ValueTask DeleteAsync(int vehicleId);
}