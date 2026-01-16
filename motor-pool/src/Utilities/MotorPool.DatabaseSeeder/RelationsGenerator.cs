using MotorPool.Domain;

namespace MotorPool.DatabaseSeeder;

public interface RelationsGenerator
{
    List<DriverVehicle> GenerateDriverVehicles();

    void GenerateActiveDrivers();
}

public class RandomRelationsGenerator(MotorPoolRandomizer randomizer, List<Vehicle> vehicles, List<Driver> drivers) : RelationsGenerator
{
    public List<DriverVehicle> GenerateDriverVehicles() => randomizer.GetSample(vehicles)
                                                                     .AsParallel()
                                                                     .SelectMany(vehicle => randomizer.GetSample(drivers)
                                                                                                      .Select(driver => new DriverVehicle
                                                                                                                        {
                                                                                                                            VehicleId = vehicle.VehicleId,
                                                                                                                            DriverId = driver.DriverId
                                                                                                                        })
                                                                                                      .ToList())
                                                                     .ToList();

    public void GenerateActiveDrivers()
    {
        const double VEHICLE_WITH_ACTIVE_DRIVER_PROBABILITY = 0.12;

        List<Vehicle> vehiclesWithDrivers = vehicles.Where(vehicle => vehicle.DriverVehicles.Count != 0).ToList();

        HashSet<Driver> activeDrivers = new();

        randomizer.GetSample(vehiclesWithDrivers, VEHICLE_WITH_ACTIVE_DRIVER_PROBABILITY).ForEach(vehicle =>
        {
            List<Driver> potentialActiveDrivers = drivers.Where(driver => driver.DriverVehicles.AsParallel().Any(driverVehicle => driverVehicle.VehicleId == vehicle.VehicleId) && !activeDrivers.Contains(driver)).ToList();
            Driver newActiveDriver = potentialActiveDrivers[randomizer.FromRange(0, potentialActiveDrivers.Count - 1)];
            activeDrivers.Add(newActiveDriver);
            newActiveDriver.ActiveVehicleId = vehicle.VehicleId;
        });
    }
}