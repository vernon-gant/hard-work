using CommandLine;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using MotorPool.Persistence;
using MotorPool.TripGenerator;

using OneOf;
using OneOf.Types;

using Error = CommandLine.Error;

ConfigurationBuilder configurationBuilder = new ();
configurationBuilder.AddUserSecrets<Program>();
IConfigurationRoot configuration = configurationBuilder.Build();

ParserResult<TripGenerationOptions> parsedResult = Parser.Default.ParseArguments<TripGenerationOptions>(args);

if (parsedResult.Errors.Any())
{
    foreach (Error error in parsedResult.Errors) Console.WriteLine(error);

    return;
}

TripGenerationOptions generationOptions = parsedResult.Value;

var isSingleTrip = generationOptions.IsSingleTrip.Length > 0;

AppDbContext dbContext = new (new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(generationOptions.ConnectionString).Options);

if (isSingleTrip && dbContext.Vehicles.Find(generationOptions.VehicleId) is null)
{
    Console.WriteLine($"Vehicle with ID {generationOptions.VehicleId} does not exist.");

    return;
}

string graphHopperApiKey = configuration["GraphHopper:APIKey"] ?? throw new InvalidOperationException("GraphHopper API key is missing.");

GraphHopperClient graphHopperClient = new (graphHopperApiKey);
DistanceCalculator distanceCalculator = new Haversine();
TripManager tripManager = new (distanceCalculator);
TripGenerator tripGenerator = new (dbContext, tripManager, graphHopperClient);
TripRandomizer randomizer = new ();

OneOf<Success, GraphHopperError, DatabaseError> result = isSingleTrip
    ? tripGenerator.GenerateSingleTrip(generationOptions.RequestedTrip)
    : await tripGenerator.GenerateRandomTripsAsync(randomizer, generationOptions);

result.Switch(
    _ => Console.WriteLine("Success"),
    _ => Console.WriteLine("GraphHopper error"),
    _ => Console.WriteLine("Database error")
);


internal partial class Program { }