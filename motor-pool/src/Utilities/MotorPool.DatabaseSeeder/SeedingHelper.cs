using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MotorPool.Auth.User;
using MotorPool.Domain;
using MotorPool.Persistence;

namespace MotorPool.DatabaseSeeder;

public static class SeedingHelper
{
    public static List<VehicleBrand> VehicleBrands => new()
                                                      {
                                                          new VehicleBrand
                                                          {
                                                              CompanyName = "Toyota", ModelName = "Corolla", Type = VehicleType.PassengerCar, FuelTankCapacityLiters = 50,
                                                              PayloadCapacityKg = 450, NumberOfSeats = 5, ReleaseYear = 2020
                                                          },
                                                          new VehicleBrand
                                                          {
                                                              CompanyName = "Volvo", ModelName = "B7R", Type = VehicleType.Bus, FuelTankCapacityLiters = 300,
                                                              PayloadCapacityKg = 7500, NumberOfSeats = 50, ReleaseYear = 2019
                                                          },
                                                          new VehicleBrand
                                                          {
                                                              CompanyName = "Scania", ModelName = "P Series", Type = VehicleType.Truck, FuelTankCapacityLiters = 400,
                                                              PayloadCapacityKg = 15000, NumberOfSeats = 3, ReleaseYear = 2018
                                                          },
                                                          new VehicleBrand
                                                          {
                                                              CompanyName = "Mercedes-Benz", ModelName = "Citaro", Type = VehicleType.Bus, FuelTankCapacityLiters = 275,
                                                              PayloadCapacityKg = 18000, NumberOfSeats = 45, ReleaseYear = 2021
                                                          },
                                                          new VehicleBrand
                                                          {
                                                              CompanyName = "Ford", ModelName = "Transit", Type = VehicleType.Truck, FuelTankCapacityLiters = 80,
                                                              PayloadCapacityKg = 1200, NumberOfSeats = 3, ReleaseYear = 2022
                                                          }
                                                      };

    public static List<Enterprise> Enterprises => new()
                                                  {
                                                      new Enterprise
                                                      {
                                                          Name = "Garosh industries", City = "New York", Street = "5th Avenue", VAT = "US123456789", TimeZoneId = "America/New_York",
                                                          FoundedOn = DateOnly.FromDateTime(new DateTime(2012, 1, 1))
                                                      },
                                                      new Enterprise
                                                      {
                                                          Name = "Apple", City = "Los Angeles", Street = "Hollywood Boulevard", VAT = "US987654321", TimeZoneId = "America/Los_Angeles",
                                                          FoundedOn = DateOnly.FromDateTime(new DateTime(2004, 4, 1))
                                                      },
                                                      new Enterprise
                                                      {
                                                          Name = "Microsoft", City = "Chicago", Street = "Michigan Avenue", VAT = "US123789456", TimeZoneId = "America/Chicago",
                                                          FoundedOn = DateOnly.FromDateTime(new DateTime(2000, 4, 4))
                                                      },
                                                      new Enterprise
                                                      {
                                                          Name = "Amazon", City = "Houston", Street = "Texas Avenue", VAT = "US456123789", TimeZoneId = "America/Chicago",
                                                          FoundedOn = DateOnly.FromDateTime(new DateTime(1994, 7, 5))
                                                      },
                                                      new Enterprise
                                                      {
                                                          Name = "Tochmash", City = "Vladimir", Street = "Severnaya Street", VAT = "RU789456123", TimeZoneId = "Europe/Moscow",
                                                          FoundedOn = DateOnly.FromDateTime(new DateTime(1950, 1, 1))
                                                      },
                                                      new Enterprise
                                                      {
                                                          Name = "SAP", City = "Berlin", Street = "Wehlistrasse", VAT = "DE3242354325", TimeZoneId = "Etc/UTC",
                                                          FoundedOn = DateOnly.FromDateTime(new DateTime(1990, 4, 1))
                                                      }
                                                  };

    public static List<Manager> Managers => new()
                                            {
                                                new Manager(),
                                                new Manager(),
                                                new Manager()
                                            };

    public static void SeedEntities(AppDbContext context)
    {
        context.VehicleBrands.AddRange(VehicleBrands);

        var managers = Managers;
        var enterprises = Enterprises;

        context.Managers.AddRange(managers);
        context.Enterprises.AddRange(enterprises);

        context.SaveChanges();

        var enterpriseManagers = new List<EnterpriseManager>
                                 {
                                     new() { Enterprise = enterprises[0], Manager = managers[0] },
                                     new() { Enterprise = enterprises[0], Manager = managers[1] },
                                     new() { Enterprise = enterprises[1], Manager = managers[0] },
                                     new() { Enterprise = enterprises[1], Manager = managers[1] },
                                     new() { Enterprise = enterprises[2], Manager = managers[0] },
                                     new() { Enterprise = enterprises[2], Manager = managers[2] },
                                     new() { Enterprise = enterprises[3], Manager = managers[0] },
                                     new() { Enterprise = enterprises[4], Manager = managers[0] },
                                     new() { Enterprise = enterprises[5], Manager = managers[2] }
                                 };

        context.EnterpriseManagers.AddRange(enterpriseManagers);

        context.SaveChanges();

        Console.WriteLine("Entities seeded");
    }

    public static void SeedUsers(AuthDbContext authDbContext)
    {
        var userManager = CreateUserManager(authDbContext);

        var admin = new ApplicationUser { UserName = "admin", Email = "admin@gmail.com" };

        userManager.CreateAsync(admin, "Qwerty11!").Wait();
        userManager.AddClaimAsync(admin, new Claim("ManagerId", "1")).Wait();

        Console.WriteLine("Admin user created");
    }

    private static UserManager<ApplicationUser> CreateUserManager(AuthDbContext authDbContext)
    {
        var store = new UserStore<ApplicationUser>(authDbContext);
        var options = new IdentityOptions();
        var passwordHasher = new PasswordHasher<ApplicationUser>();
        var userValidators = new List<IUserValidator<ApplicationUser>>();
        var passwordValidators = new List<IPasswordValidator<ApplicationUser>>();
        var keyNormalizer = new UpperInvariantLookupNormalizer();
        var errorDescriber = new IdentityErrorDescriber();
        var services = new ServiceCollection();

        return new UserManager<ApplicationUser>(store, Options.Create(options), passwordHasher, userValidators, passwordValidators, keyNormalizer, errorDescriber, services.BuildServiceProvider(),
            new LoggerFactory().CreateLogger<UserManager<ApplicationUser>>());
    }
}