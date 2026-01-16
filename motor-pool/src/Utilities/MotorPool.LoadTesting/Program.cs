using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using MotorPool.LoadTesting;
using MotorPool.LoadTesting.Configuration;
using NBomber.CSharp;

ConfigurationManager configuration = new ();
configuration.AddEnvironmentVariables();
AppSettings appSettings = configuration.GetSection("AppSettings").Get<AppSettings>() ?? throw new InvalidOperationException("AppSettings not found.");

LoginHandler loginHandler = new (appSettings);
string token = await loginHandler.GetTokenAsync();

var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri(appSettings.BaseUrl);
httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

var vehicleScenario1 = VehicleScenarios.GetVehiclesWithTripsAndQueryFirstTripScenario(httpClient);
var vehicleScenario2 = VehicleScenarios.GetVehiclesWithTripsAndGenerateReportScenario(httpClient);
var telemetryScenario = TelemetryScenarios.CreateGetEnterprisesScenario(httpClient);

NBomberRunner
   .RegisterScenarios(vehicleScenario1, vehicleScenario2, telemetryScenario)
   .Run();