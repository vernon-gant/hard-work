using Ampol.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EntityBenchmark;

public record UserEventQueryResult(
    Guid Id,
    string Title,
    DateTime DateFrom,
    DateTime? DateTo,
    DateTime TimeFrom,
    DateTime TimeTo,
    DaysOfWeekGermanFlagEnum DaysOfWeek,
    Guid UserId,
    string? UserFullName,
    UserEventType Type,
    int? ZipCode,
    Guid? CityId);

[TestFixture]
public class UserEventQueryBenchmarkTests : BenchmarkBase
{
    private const int UserCount = 50;
    private const int EventsPerUser = 1000;

    private static readonly List<Guid> AdvisorIds = [];
    private static readonly DateTime StartDate = new(2025, 4, 1);
    private static readonly DateTime EndDate = new(2025, 4, 14);
    private static readonly TimeSpan StartTime = new(8, 0, 0);
    private static readonly TimeSpan EndTime = new(17, 0, 0);

    [OneTimeSetUp]
    public async Task Seed()
    {
        var users = DataSeeder.CreateUsers(UserCount);
        var events = DataSeeder.CreateUserEvents(users, EventsPerUser, new DateTime(2025, 3, 1));
        AdvisorIds.AddRange(users.Take(UserCount / 2).Select(u => u.Id));

        await using var db = Db();
        db.Users.AddRange(users);
        await db.SaveChangesAsync();

        foreach (var batch in events.Chunk(500))
        {
            await using var batchDb = Db();
            batchDb.UserEvents.AddRange(batch);
            await batchDb.SaveChangesAsync();
        }

        TestContext.Out.WriteLine($"Seeded {users.Count} users, {events.Count} events");
        TestContext.Out.WriteLine($"Querying {AdvisorIds.Count} advisors, {StartDate:d} - {EndDate:d}, {StartTime} - {EndTime}");
    }

    [OneTimeTearDown]
    public async Task Cleanup()
    {
        await using var db = Db();
        await db.Database.ExecuteSqlRawAsync("DELETE FROM [UserEvents]");
        await db.Database.ExecuteSqlRawAsync("DELETE FROM [AspNetUsers]");
    }

    [Test, Order(1)]
    public async Task Benchmark_EfCore_vs_RawSql() => await RunBenchmark(EfCoreQuery, RawSqlQuery);

    [Test, Order(2)]
    public async Task Verify_ResultsAreIdentical()
    {
        var (_, efResults) = await Timed(EfCoreQueryWithResults);
        var (_, rawResults) = await Timed(RawSqlQueryWithResults);

        var efIds = efResults.Select(r => r.Id).Order().ToList();
        var rawIds = rawResults.Select(r => r.Id).Order().ToList();
        Assert.That(rawIds, Is.EqualTo(efIds));

        var ef1 = efResults.OrderBy(r => r.Id).First();
        var raw1 = rawResults.OrderBy(r => r.Id).First();

        Assert.Multiple(() =>
        {
            Assert.That(raw1.Title, Is.EqualTo(ef1.Title));
            Assert.That(raw1.UserFullName, Is.EqualTo(ef1.UserFullName));
            Assert.That(raw1.Type, Is.EqualTo(ef1.Type));
        });

        TestContext.Out.WriteLine($"Verified: {efIds.Count} rows identical");
    }

    [Test, Order(3)]
    public async Task ShowGeneratedSql()
    {
        var sql = new List<string>();
        await using var db = BenchmarkDatabaseSetup.CreateDbContextWithLogging(msg =>
        {
            if (msg.Contains("SELECT")) sql.Add(msg);
        });

        _ = await BuildEfQuery(db).ToListAsync();

        TestContext.Out.WriteLine("=== EF Core Generated SQL ===");
        foreach (var s in sql.TakeLast(2))
            TestContext.Out.WriteLine(s);
    }

    // ── EF Core ──

    private static IQueryable<BenchmarkUserEvent> BuildEfQuery(BenchmarkDbContext db)
    {
        DateTime startDate = StartDate, endDate = EndDate;

        return db.UserEvents.AsNoTracking()
            .Where(x => AdvisorIds.Contains(x.UserId))
            .Where(x => x.Type == UserEventType.Presence || x.Type == UserEventType.Absence)
            .Where(x => startDate <= x.DateTo && endDate >= x.DateFrom)
            .Where(x => StartTime <= x.TimeTo.TimeOfDay && EndTime >= x.TimeFrom.TimeOfDay)
            .Include(x => x.User);
    }

    private static async Task<(long Ms, int Rows)> EfCoreQuery()
    {
        await using var db = Db();
        var (ms, results) = await Timed(() => BuildEfQuery(db).ToListAsync());
        return (ms, results.Count);
    }

    private static async Task<List<UserEventQueryResult>> EfCoreQueryWithResults()
    {
        await using var db = Db();
        var results = await BuildEfQuery(db).ToListAsync();
        return results.Select(e => new UserEventQueryResult(
            e.Id, e.Title, e.DateFrom, e.DateTo, e.TimeFrom, e.TimeTo,
            e.DaysOfWeekGermanFlagEnum, e.UserId, e.User?.FullName, e.Type,
            e.ZipCode, e.CityId)).ToList();
    }

    // ── Raw SQL ──

    private static async Task<(long Ms, int Rows)> RawSqlQuery()
    {
        await using var db = Db();
        var conn = db.Database.GetDbConnection();
        await conn.OpenAsync();
        try
        {
            var (ms, results) = await Timed(() => ExecuteRaw(conn));
            return (ms, results.Count);
        }
        finally { await conn.CloseAsync(); }
    }

    private static async Task<List<UserEventQueryResult>> RawSqlQueryWithResults()
    {
        await using var db = Db();
        var conn = db.Database.GetDbConnection();
        await conn.OpenAsync();
        try { return await ExecuteRaw(conn); }
        finally { await conn.CloseAsync(); }
    }

    private static async Task<List<UserEventQueryResult>> ExecuteRaw(System.Data.Common.DbConnection conn)
    {
        using var cmd = conn.CreateCommand();

        var inParams = AdvisorIds.Select((id, i) =>
        {
            var name = $"@uid{i}";
            AddParam(cmd, name, id);
            return name;
        }).ToList();

        cmd.CommandText = $@"
            SELECT
                ue.[Id], ue.[Title], ue.[DateFrom], ue.[DateTo],
                ue.[TimeFrom], ue.[TimeTo], ue.[DaysOfWeekGermanFlagEnum],
                ue.[UserId], u.[FullName] AS UserFullName,
                ue.[Type], ue.[ZipCode], ue.[CityId]
            FROM [UserEvents] ue
            INNER JOIN [AspNetUsers] u ON u.[Id] = ue.[UserId]
            WHERE ue.[UserId] IN ({string.Join(", ", inParams)})
              AND ue.[Type] IN (@typePresence, @typeAbsence)
              AND @startDate <= ue.[DateTo]
              AND @endDate   >= ue.[DateFrom]
              AND @startTime <= CAST(ue.[TimeTo] AS TIME)
              AND @endTime   >= CAST(ue.[TimeFrom] AS TIME)";

        AddParam(cmd, "@typePresence", (int)UserEventType.Presence);
        AddParam(cmd, "@typeAbsence", (int)UserEventType.Absence);
        AddParam(cmd, "@startDate", StartDate);
        AddParam(cmd, "@endDate", EndDate);
        AddParam(cmd, "@startTime", StartTime);
        AddParam(cmd, "@endTime", EndTime);

        using var reader = await cmd.ExecuteReaderAsync();
        var results = new List<UserEventQueryResult>();

        while (await reader.ReadAsync())
        {
            results.Add(new UserEventQueryResult(
                Id: reader.GetGuid(O("Id")),
                Title: reader.GetString(O("Title")),
                DateFrom: reader.GetDateTime(O("DateFrom")),
                DateTo: reader.IsDBNull(O("DateTo")) ? null : reader.GetDateTime(O("DateTo")),
                TimeFrom: reader.GetDateTime(O("TimeFrom")),
                TimeTo: reader.GetDateTime(O("TimeTo")),
                DaysOfWeek: (DaysOfWeekGermanFlagEnum)reader.GetInt32(O("DaysOfWeekGermanFlagEnum")),
                UserId: reader.GetGuid(O("UserId")),
                UserFullName: reader.IsDBNull(O("UserFullName")) ? null : reader.GetString(O("UserFullName")),
                Type: (UserEventType)reader.GetInt32(O("Type")),
                ZipCode: reader.IsDBNull(O("ZipCode")) ? null : reader.GetInt32(O("ZipCode")),
                CityId: reader.IsDBNull(O("CityId")) ? null : reader.GetGuid(O("CityId"))
            ));

            int O(string col) => reader.GetOrdinal(col);
        }

        return results;
    }

    private static void AddParam(System.Data.Common.DbCommand cmd, string name, object value)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = name;
        p.Value = value;
        cmd.Parameters.Add(p);
    }
}