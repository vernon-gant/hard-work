using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using MotorPool.Domain.Reports;
using MotorPool.LoadTesting.Configuration;
using MotorPool.Services.Reporting.DTO;
using MotorPool.Services.Vehicles.Models;
using NBomber.Contracts;
using NBomber.CSharp;

namespace MotorPool.LoadTesting;

public class VehicleScenarios
{
    public static async Task<Response<List<VehicleViewModel>>> GetVehiclesStep(HttpClient httpClient)
    {
        HttpResponseMessage response = await httpClient.GetAsync("/vehicles?WithTrips=true");
        response.EnsureSuccessStatusCode();

        string vehiclesResponse = await response.Content.ReadAsStringAsync();
        List<VehicleViewModel> vehicles = JsonSerializer.Deserialize<List<VehicleViewModel>>(vehiclesResponse, JsonSettings.SerializerOptions) ?? throw new InvalidOperationException("Failed to deserialize vehicles");

        return vehicles.Count == 0 ? Response.Fail<List<VehicleViewModel>>("No vehicles found") : Response.Ok(vehicles);
    }

    public static ScenarioProps GetVehiclesWithTripsAndQueryFirstTripScenario(HttpClient httpClient) =>
        Scenario.Create("GetVehiclesWithTripsAndQueryFirstTrip", async context =>
                 {
                     var step1 = await GetVehiclesStep(httpClient);

                     context.Data["vehicles"] = step1.Payload.Value;

                     await Step.Run("get_first_vehicle_trip", context, async () =>
                     {
                         List<VehicleViewModel> vehicles = (context.Data["vehicles"] as List<VehicleViewModel>)!;
                         VehicleViewModel firstVehicle = vehicles.First();
                         string now = DateTime.Now.ToString("s");
                         string yearAgo = DateTime.Now.AddYears(-1).ToString("s");

                         HttpResponseMessage tripResponse = await httpClient.GetAsync($"/vehicles/{firstVehicle.VehicleId}/trips/{yearAgo}/{now}");
                         tripResponse.EnsureSuccessStatusCode();

                         return Response.Ok();
                     });

                     return Response.Ok();
                 })
                .WithLoadSimulations(Simulation.Inject(rate: 5, interval: TimeSpan.FromSeconds(5), during: TimeSpan.FromMinutes(1)));

    public static ScenarioProps GetVehiclesWithTripsAndGenerateReportScenario(HttpClient httpClient)
    {
        return Scenario.Create("GetVehiclesWithTripsAndGenerateReport", async context =>
                        {
                            var step1 = await GetVehiclesStep(httpClient);

                            context.Data["vehicles"] = step1.Payload.Value;

                            await Step.Run("generate_report", context, async () =>
                            {
                                List<VehicleViewModel> vehicles = (context.Data["vehicles"] as List<VehicleViewModel>)!;
                                Random random = new Random();
                                VehicleViewModel randomVehicle = vehicles[random.Next(vehicles.Count)];
                                context.Data["vehicleId"] = randomVehicle.VehicleId;
                                VehicleMileageReportDTO reportDTO = new()
                                                                    {
                                                                        VehicleId = randomVehicle.VehicleId,
                                                                        StartTime = DateOnly.FromDateTime(DateTime.Now.AddMonths(-6).ToLocalTime()),
                                                                        EndTime = DateOnly.FromDateTime(DateTime.Now.ToLocalTime()),
                                                                        Period = Period.Year
                                                                    };
                                StringContent reportContent = new(JsonSerializer.Serialize(reportDTO), Encoding.UTF8, "application/json");
                                HttpResponseMessage reportResponse = await httpClient.PostAsync("/reports/vehicle-mileage", reportContent);
                                reportResponse.EnsureSuccessStatusCode();

                                return Response.Ok();
                            });

                            await Step.Run("generate_report_cache", context, async () =>
                            {
                                int vehicleId = (int)context.Data["vehicleId"];
                                VehicleMileageReportDTO reportDTO = new()
                                                                    {
                                                                        VehicleId = vehicleId,
                                                                        StartTime = DateOnly.FromDateTime(DateTime.Now.AddMonths(-6).ToLocalTime()),
                                                                        EndTime = DateOnly.FromDateTime(DateTime.Now.ToLocalTime()),
                                                                        Period = Period.Year
                                                                    };
                                StringContent reportContent = new(JsonSerializer.Serialize(reportDTO), Encoding.UTF8, "application/json");
                                HttpResponseMessage reportResponse = await httpClient.PostAsync("/reports/vehicle-mileage", reportContent);
                                reportResponse.EnsureSuccessStatusCode();

                                return Response.Ok();
                            });

                            return Response.Ok();
                        })
                       .WithLoadSimulations(Simulation.Inject(rate: 1, interval: TimeSpan.FromSeconds(20), during: TimeSpan.FromMinutes(1)))
                       .WithWarmUpDuration(TimeSpan.FromSeconds(5));
    }
}