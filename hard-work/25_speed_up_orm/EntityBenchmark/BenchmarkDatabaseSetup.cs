using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace EntityBenchmark;

[SetUpFixture]
public class BenchmarkDatabaseSetup
{
    private static MsSqlContainer _container = null!;
    private static string _connectionString = null!;

    [OneTimeSetUp]
    public async Task GlobalSetup()
    {
        _container = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithCleanUp(true)
            .Build();

        await _container.StartAsync();
        _connectionString = _container.GetConnectionString();

        await using var ctx = CreateDbContext();
        await ctx.Database.EnsureCreatedAsync();
    }

    [OneTimeTearDown]
    public async Task GlobalTeardown() => await _container.DisposeAsync();

    public static BenchmarkDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<BenchmarkDbContext>()
            .UseSqlServer(_connectionString)
            .Options);

    public static BenchmarkDbContext CreateDbContextWithLogging(Action<string> log) =>
        new(new DbContextOptionsBuilder<BenchmarkDbContext>()
            .UseSqlServer(_connectionString)
            .LogTo(log, Microsoft.Extensions.Logging.LogLevel.Information)
            .EnableSensitiveDataLogging()
            .Options);
}