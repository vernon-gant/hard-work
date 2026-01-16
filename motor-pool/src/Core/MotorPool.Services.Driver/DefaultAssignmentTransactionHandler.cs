using System.Transactions;
using MotorPool.Domain;
using MotorPool.Repository.Driver;

namespace MotorPool.Services.Drivers;

public class DefaultAssignmentTransactionHandler(AssignmentChangeLogger changeLogger, DriverQueryRepository driverVehicleRepository) : AssignmentTransactionHandler
{
    public async Task<TransactionResult> AssignVehicleAsync(DriverVehicle assignment)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var assignmentAdded = await driverVehicleRepository.AddVehicleAssignmentAsync(assignment);

        if (!assignmentAdded) return new TransactionError("Failed to add vehicle assignment");

        changeLogger.AppendLog(assignment);

        if (changeLogger.LastAppendResult is Failure failure) return new TransactionError(failure.ErrorMessage);

        scope.Complete();

        return new TransactionSuccess();
    }
}