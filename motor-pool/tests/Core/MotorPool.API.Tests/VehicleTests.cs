using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MotorPool.Domain;
using MotorPool.Persistence;
using MotorPool.Services.Vehicles.Models;

namespace MotorPool.API.Tests;

public class VehicleTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task GetVehicles_ReturnsVehicles_WhenAuthenticated()
    {
        var factory = new MotorPoolWebApplicationFactory(DbFixture.ConnectionString);
        var jwtToken = AuthHelper.GenerateJwtToken(DbFixture.ManagerId);
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var dbContext = factory.Services.GetRequiredService<AppDbContext>();
        dbContext.Vehicles.Add(new Vehicle
                               {
                                   MotorVIN = Guid.NewGuid().ToString("N").Substring(0, 17),
                                   Cost = 20000,
                                   ManufactureYear = 2020,
                                   ManufactureLand = "Japan",
                                   Mileage = 5000,
                                   AcquiredOn = DateTime.UtcNow.AddYears(-1),
                                   EnterpriseId = 1,
                                   VehicleBrandId = 1
                               });
        dbContext.Vehicles.Add(new Vehicle
                               {
                                   MotorVIN = Guid.NewGuid().ToString("N").Substring(0, 17),
                                   Cost = 30000,
                                   ManufactureYear = 2021,
                                   ManufactureLand = "Germany",
                                   Mileage = 10000,
                                   AcquiredOn = DateTime.UtcNow.AddYears(-1),
                                   EnterpriseId = 1,
                                   VehicleBrandId = 2
                               });
        dbContext.Vehicles.Add(new Vehicle
                               {
                                   MotorVIN = Guid.NewGuid().ToString("N").Substring(0, 17),
                                   Cost = 40000,
                                   ManufactureYear = 2022,
                                   ManufactureLand = "USA",
                                   Mileage = 15000,
                                   AcquiredOn = DateTime.UtcNow.AddYears(-1),
                                   EnterpriseId = 1,
                                   VehicleBrandId = 3
                               });

        await dbContext.SaveChangesAsync();

        var response = await client.GetAsync("/vehicles?withTrips=false");

        var vehicles = await response.Content.ReadFromJsonAsync<IEnumerable<Vehicle>>();

        Assert.IsTrue(response.IsSuccessStatusCode);
        vehicles.Should().HaveCount(3);
    }

    [Test]
    public async Task GetVehicles_ReturnsForbidden_WhenManagerIsNotAccessible()
    {
        var factory = new MotorPoolWebApplicationFactory(DbFixture.ConnectionString);
        var client = factory.CreateClient();
        var managerId = 999;
        var jwtToken = AuthHelper.GenerateJwtToken(managerId);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var response = await client.GetAsync("/vehicles?withTrips=false");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task GetVehicle_ReturnsVehicle_WhenAuthenticated()
    {
        var factory = new MotorPoolWebApplicationFactory(DbFixture.ConnectionString);
        var jwtToken = AuthHelper.GenerateJwtToken(DbFixture.ManagerId);
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var dbContext = factory.Services.GetRequiredService<AppDbContext>();
        var dbVehicle = new Vehicle
                      {
                          MotorVIN = Guid.NewGuid().ToString("N").Substring(0, 17),
                          Cost = 20000,
                          ManufactureYear = 2020,
                          ManufactureLand = "Japan",
                          Mileage = 5000,
                          AcquiredOn = DateTime.UtcNow.AddYears(-1),
                          EnterpriseId = 1,
                          VehicleBrandId = 1
                      };
        dbContext.Vehicles.Add(dbVehicle);

        await dbContext.SaveChangesAsync();

        var response = await client.GetAsync($"/vehicles/{dbVehicle.VehicleId}");

        response.EnsureSuccessStatusCode();

        var vehicle = await response.Content.ReadFromJsonAsync<Vehicle>();

        vehicle.Should().NotBeNull();
    }

    [Test]
    public async Task GetVehicle_ReturnsForbidden_WhenManagerIsNotAccessible()
    {
        var factory = new MotorPoolWebApplicationFactory(DbFixture.ConnectionString);
        var client = factory.CreateClient();
        var managerId = 3;
        var jwtToken = AuthHelper.GenerateJwtToken(managerId);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var dbContext = factory.Services.GetRequiredService<AppDbContext>();
        var dbVehicle = new Vehicle
                      {
                          MotorVIN = Guid.NewGuid().ToString("N").Substring(0, 17),
                          Cost = 20000,
                          ManufactureYear = 2020,
                          ManufactureLand = "Japan",
                          Mileage = 5000,
                          AcquiredOn = DateTime.UtcNow.AddYears(-1),
                          EnterpriseId = 1,
                          VehicleBrandId = 1
                      };
        dbContext.Vehicles.Add(dbVehicle);

        await dbContext.SaveChangesAsync();

        var response = await client.GetAsync($"/vehicles/{dbVehicle.VehicleId}");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task CreateVehicle_ReturnsVehicle_WhenAuthenticated()
    {
        var factory = new MotorPoolWebApplicationFactory(DbFixture.ConnectionString);
        var jwtToken = AuthHelper.GenerateJwtToken(DbFixture.ManagerId);
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var vehicle = new VehicleDTO
                      {
                          MotorVIN = Guid.NewGuid().ToString("N").Substring(0, 17),
                          Cost = 20000,
                          ManufactureYear = 2020,
                          ManufactureLand = "Japan",
                          Mileage = 5000,
                          AcquiredOn = DateTime.UtcNow.AddYears(-1),
                          EnterpriseId = 1,
                          VehicleBrandId = 1
                      };

        var response = await client.PostAsJsonAsync("/vehicles", vehicle);

        response.EnsureSuccessStatusCode();

        var createdVehicle = await response.Content.ReadFromJsonAsync<Vehicle>();

        createdVehicle.Should().NotBeNull();

        var dbContext = factory.Services.GetRequiredService<AppDbContext>();
        var dbVehicle = await dbContext.Vehicles.FindAsync(createdVehicle!.VehicleId);

        dbVehicle.Should().NotBeNull();
    }

    [TearDown]
    public void TearDown()
    {
    }
}