namespace MotorPool.Services.Manager;

public interface ManagerPermissionService
{

    ValueTask<bool> IsManagerAccessibleEnterprise(int managerId, int enterpriseId);

    ValueTask<bool> IsManagerAccessibleVehicle(int managerId, int vehicleId);

    ValueTask<bool> IsManagerAccessibleDriver(int managerId, int driverId);

}