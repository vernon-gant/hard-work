using MotorPool.Services.VehicleBrand.Models;

namespace MotorPool.Services.VehicleBrand.Services;

public interface VehicleBrandService
{
    ValueTask<List<VehicleBrandSignatureWithId>> GetVehicleBrandsWithIdAsync();

    ValueTask<List<VehicleBrandViewModel>> GetAllAsync();

    ValueTask<VehicleBrandViewModel?> GetByIdAsync(int vehicleBrandId);

    ValueTask CreateAsync(VehicleBrandDTO vehicleBrandDTO);
}