using System.Collections.Concurrent;
using CommandLine;

using Microsoft.EntityFrameworkCore;
using MotorPool.Auth.User;
using MotorPool.DatabaseSeeder;
using MotorPool.Domain;
using MotorPool.Persistence;

ParserResult<SeedingOptions> parsedResult = Parser.Default.ParseArguments<SeedingOptions>(args);

if (parsedResult.Errors.Any())
{
    Console.WriteLine("Invalid arguments. Please provide the enterprise ids, the number of vehicles and drivers per enterprise.");
    foreach (Error error in parsedResult.Errors) Console.WriteLine(error);
    return;
}

SeedingOptions options = parsedResult.Value;

AppDbContext dbContext = new (new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(options.ConnectionString).Options);
AuthDbContext authDbContext = new (new DbContextOptionsBuilder<AuthDbContext>().UseSqlServer(options.ConnectionString).Options);

if (options.InitialSeed)
{
    dbContext.Database.EnsureDeleted();
    authDbContext.Database.EnsureDeleted();

    dbContext.Database.Migrate(); // Applies any pending migrations for AppDbContext
    authDbContext.Database.Migrate();

    SeedingHelper.SeedEntities(dbContext);
    SeedingHelper.SeedUsers(authDbContext);
    return;
}

if (options.DriversPerEnterprise < 1 || options.VehiclesPerEnterprise < 1)
{
    Console.WriteLine("Invalid arguments. The number of vehicles and drivers per enterprise must be greater than 0.");
    return;
}

Repository repository = new EfCoreRepository(dbContext);

if (!repository.AllEnterprisesExist(options.EnterpriseIds))
{
    Console.WriteLine("Invalid arguments. One or more enterprise ids do not exist.");
    return;
}

MotorPoolRandomizer randomizer = new PseudoMotorPoolRandomizer();
List<int> vehicleBrandIds = repository.GetVehicleBrandIds();
VehicleDriversGenerator dataGenerator = new RandomVehicleDriversGenerator(vehicleBrandIds, options.VehiclesPerEnterprise, options.DriversPerEnterprise, randomizer);

ConcurrentBag<List<Vehicle>> vehiclesBag = new ConcurrentBag<List<Vehicle>>();
ConcurrentBag<List<Driver>> driversBag = new ConcurrentBag<List<Driver>>();

Parallel.ForEach(options.EnterpriseIds,
    enterpriseId =>
    {
        Parallel.Invoke(
            () => vehiclesBag.Add(dataGenerator.GenerateVehicles(enterpriseId)),
            () => driversBag.Add(dataGenerator.GenerateDrivers(enterpriseId))
        );
    });

List<Vehicle> vehicles = vehiclesBag.SelectMany(vehicleList => vehicleList).ToList();
List<Driver> drivers = driversBag.SelectMany(driverList => driverList).ToList();

dbContext.Vehicles.AddRange(vehicles);
dbContext.Drivers.AddRange(drivers);
dbContext.SaveChanges();

RelationsGenerator relationsGenerator = new RandomRelationsGenerator(randomizer, vehicles, drivers);

Parallel.Invoke(
    () => relationsGenerator.GenerateDriverVehicles(),
    () => relationsGenerator.GenerateActiveDrivers()
);

dbContext.SaveChanges();