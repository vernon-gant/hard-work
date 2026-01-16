using CommandLine;

namespace MotorPool.DatabaseSeeder;

public class SeedingOptions
{
    [Option('e', "enterprise-ids", Required = false, HelpText = "The ids of the enterprises where vehicles and drivers will be seeded.")]
    public IEnumerable<int> EnterpriseIds { get; set; } = Enumerable.Empty<int>();

    [Option('v', "vehicles-per-enterprise", Required = false, HelpText = "The number of vehicles to seed per enterprise.")]
    public int VehiclesPerEnterprise { get; set; }

    [Option('d', "drivers-per-enterprise", Required = false, HelpText = "The number of drivers to seed per enterprise.")]
    public int DriversPerEnterprise { get; set; }

    [Option('i', "initial-seed", Required = false, HelpText = "Seed the database with initial data.")]
    public bool InitialSeed { get; set; }

    [Option('c', "connection-string", Required = true, HelpText = "The connection string to the database.")]
    public string ConnectionString { get; set; } = string.Empty;
}