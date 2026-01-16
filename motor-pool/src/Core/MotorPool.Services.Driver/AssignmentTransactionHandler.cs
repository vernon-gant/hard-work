using MotorPool.Domain;

namespace MotorPool.Services.Drivers;

public interface AssignmentTransactionHandler
{
    Task<TransactionResult> AssignVehicleAsync(DriverVehicle assignment);
}