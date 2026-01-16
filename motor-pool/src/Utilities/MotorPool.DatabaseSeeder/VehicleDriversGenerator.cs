using Bogus;
using MotorPool.Domain;

namespace MotorPool.DatabaseSeeder;

public interface VehicleDriversGenerator
{
    List<Vehicle> GenerateVehicles(int enterpriseId);

    List<Driver> GenerateDrivers(int enterpriseId);
}

public class RandomVehicleDriversGenerator(List<int> vehicleBrandIds, int vehiclesPerEnterprise, int driversPerEnterprise, MotorPoolRandomizer randomizer) : VehicleDriversGenerator
{
    private static readonly Faker _faker = new();

    public List<Vehicle> GenerateVehicles(int enterpriseId) => Enumerable.Range(0, vehiclesPerEnterprise)
                                                                         .Select(_ => new Vehicle
                                                                                      {
                                                                                          MotorVIN = randomizer.MotorVIN(),
                                                                                          Cost = randomizer.FromRange(1, 10000),
                                                                                          ManufactureYear = randomizer.FromRange(1990, DateTime.Now.Year),
                                                                                          ManufactureLand = _faker.Address.Country(),
                                                                                          Mileage = randomizer.FromRange(0, 1000000),
                                                                                          EnterpriseId = enterpriseId,
                                                                                          VehicleBrandId = vehicleBrandIds[randomizer.FromRange(0, vehicleBrandIds.Count - 1)],
                                                                                          AcquiredOn = _faker.Date.Between(new DateTime(2005, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), DateTime.Now)
                                                                                      })
                                                                         .ToList();

    public List<Driver> GenerateDrivers(int enterpriseId) =>
        Enumerable.Range(0, driversPerEnterprise)
                  .Select(_ => new Driver
                               {
                                   FirstName = _faker.Name.FirstName(),
                                   LastName = _faker.Name.LastName(),
                                   Salary = randomizer.FromRange(1000, 10000),
                                   EnterpriseId = enterpriseId
                               })
                  .ToList();
}