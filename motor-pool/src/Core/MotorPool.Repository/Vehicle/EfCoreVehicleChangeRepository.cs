using Microsoft.Extensions.Logging;
using MotorPool.Persistence;

namespace MotorPool.Repository.Vehicle;

using Vehicle = Domain.Vehicle;

public class EfCoreVehicleChangeRepository(AppDbContext dbContext, ILogger<EfCoreVehicleChangeRepository> logger) : VehicleChangeRepository
{
    public async ValueTask<Vehicle> CreateAsync(Vehicle newVehicle)
    {
        try
        {
            await dbContext.Vehicles.AddAsync(newVehicle);

            await dbContext.SaveChangesAsync();

            logger.LogInformation("Created new vehicle {@Vehicle}", newVehicle);

            await dbContext.Entry(newVehicle)
                     .Reference<Domain.Enterprise>(vehicle => vehicle.Enterprise)
                     .LoadAsync();

            return newVehicle;
        }
        catch (Exception e)
        {
            logger.LogError(e,"Failed to create new vehicle");
            throw;
        }
    }

    public async ValueTask DeleteAsync(int vehicleId)
    {
        Vehicle? vehicle = await dbContext.Vehicles.FindAsync(vehicleId);

        if (vehicle is null) return;

        dbContext.Vehicles.Remove(vehicle);

        await dbContext.SaveChangesAsync();

        logger.LogInformation("Deleted vehicle {@Vehicle}", vehicle);
    }

    public async ValueTask UpdateAsync(Vehicle updatedVehicle)
    {
        dbContext.Vehicles.Update(updatedVehicle);

        await dbContext.SaveChangesAsync();

        logger.LogInformation("Updated vehicle {@Vehicle}", updatedVehicle);
    }
}