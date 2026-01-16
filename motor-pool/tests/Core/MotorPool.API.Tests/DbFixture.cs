using Microsoft.EntityFrameworkCore;
using MotorPool.Auth.User;
using MotorPool.Domain;
using MotorPool.Persistence;
using Testcontainers.MsSql;

namespace MotorPool.API.Tests;

[SetUpFixture]
public class DbFixture
{
    public const int ManagerId = 1;

    private AppDbContext _dbContext = null!;
    private MsSqlContainer _container = null!;

    public static string ConnectionString { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task SetUp()
    {
        _container = new MsSqlBuilder().Build();

        await _container.StartAsync();

        ConnectionString = _container.GetConnectionString() + ";TrustServerCertificate=true";

        var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(ConnectionString).EnableSensitiveDataLogging().EnableDetailedErrors().Options;
        _dbContext = new AppDbContext(dbContextOptions);

        await _dbContext.Database.MigrateAsync();
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _dbContext.DisposeAsync();
        await _container.DisposeAsync();
    }
}