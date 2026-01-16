using System.Text;
using System.Text.Json;
using Bogus;
using MotorPool.API.Messages;
using MotorPool.Services.Vehicles.Models;
using NBomber.Contracts;
using NBomber.CSharp;

namespace MotorPool.LoadTesting;

public class TelemetryScenarios
{
    public static ScenarioProps CreateGetEnterprisesScenario(HttpClient httpClient)
    {
        return Scenario.Create("PostTelemetry", async context =>
                        {
                            var step1 = await VehicleScenarios.GetVehiclesStep(httpClient);

                            context.Data["vehicles"] = step1.Payload.Value;

                            await Step.Run("post_telemetry", context, async () =>
                            {
                                List<VehicleViewModel> vehicles = (context.Data["vehicles"] as List<VehicleViewModel>)!;
                                Random random = new Random();
                                VehicleViewModel firstVehicle = vehicles[random.Next(vehicles.Count)];

                                CANTelemetry telemetry = new()
                                                         {
                                                             VehicleId = firstVehicle.VehicleId,
                                                             Latitude = new Faker().Address.Latitude(),
                                                             Longitude = new Faker().Address.Longitude(),
                                                             Speed = new Faker().Random.Double(0, 100),
                                                             Timestamp = DateTime.Now
                                                         };

                                HttpResponseMessage tripResponse = await httpClient.PostAsync("/telemetry", new StringContent(JsonSerializer.Serialize(telemetry), Encoding.UTF8, "application/json"));
                                tripResponse.EnsureSuccessStatusCode();

                                return Response.Ok();
                            });

                            return Response.Ok();
                        })
                       .WithWarmUpDuration(TimeSpan.FromSeconds(10))
                       .WithLoadSimulations(Simulation.Inject(rate: 20, interval: TimeSpan.FromSeconds(5), during: TimeSpan.FromMinutes(1)));
    }
}