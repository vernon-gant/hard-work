using Ampol.Domain.Enums.GroupEventEnums;
using Microsoft.EntityFrameworkCore;

namespace EntityBenchmark;

public record AppointmentQueryResult(
    Guid AppointmentParticipantId,
    DateTime Date,
    DateTime TimeFrom,
    DateTime TimeTo,
    int AppointmentType,
    int? GroupEventType,
    string? LocationName,
    string? AdvisorName,
    Guid? AdvisorId);

[TestFixture]
public class AppointmentQueryBenchmarkTests : BenchmarkBase
{
    private const int ParticipantCount = 5000;
    private const int GroupEventCount = 2000;
    private const int AppointmentsPerGroupEvent = 20;

    private static readonly List<Guid> TargetParticipantIds = [];
    private static readonly bool OnlyFutureDates = false;

    [OneTimeSetUp]
    public async Task Seed()
    {
        var users = DataSeeder.CreateUsers(10);
        var participants = AppointmentDataSeeder.CreateParticipants(ParticipantCount);
        var locations = AppointmentDataSeeder.CreateLocations();
        var groupEvents = AppointmentDataSeeder.CreateGroupEvents(GroupEventCount);
        var appointments = AppointmentDataSeeder.CreateAppointments(groupEvents, locations, AppointmentsPerGroupEvent, new DateTime(2025, 3, 1));
        var appointmentUsers = AppointmentDataSeeder.CreateAppointmentUsers(appointments, users);
        var appointmentParticipants = AppointmentDataSeeder.CreateAppointmentParticipants(participants, appointments);

        // Query for 10 of 50 participants
        TargetParticipantIds.AddRange(participants.Take(10).Select(p => p.Id));

        await using var db = Db();
        db.Users.AddRange(users);
        db.Participants.AddRange(participants);
        db.AppointmentLocations.AddRange(locations);
        db.GroupEvents.AddRange(groupEvents);
        await db.SaveChangesAsync();

        foreach (var batch in appointments.Chunk(500))
        {
            await using var batchDb = Db();
            batchDb.Appointments.AddRange(batch);
            await batchDb.SaveChangesAsync();
        }

        foreach (var batch in appointmentUsers.Chunk(500))
        {
            await using var batchDb = Db();
            batchDb.AppointmentUsers.AddRange(batch);
            await batchDb.SaveChangesAsync();
        }

        foreach (var batch in appointmentParticipants.Chunk(500))
        {
            await using var batchDb = Db();
            batchDb.AppointmentParticipants.AddRange(batch);
            await batchDb.SaveChangesAsync();
        }

        TestContext.Out.WriteLine(
            $"Seeded: {users.Count} users, {participants.Count} participants, " +
            $"{groupEvents.Count} group events, {appointments.Count} appointments, " +
            $"{appointmentUsers.Count} appointment-users, {appointmentParticipants.Count} appointment-participants");
        TestContext.Out.WriteLine($"Querying {TargetParticipantIds.Count} participants, OnlyFutureDates={OnlyFutureDates}");
    }

    [OneTimeTearDown]
    public async Task Cleanup()
    {
        await using var db = Db();
        await db.Database.ExecuteSqlRawAsync("DELETE FROM [AppointmentParticipants]");
        await db.Database.ExecuteSqlRawAsync("DELETE FROM [AppointmentUsers]");
        await db.Database.ExecuteSqlRawAsync("DELETE FROM [Appointments]");
        await db.Database.ExecuteSqlRawAsync("DELETE FROM [GroupEvents]");
        await db.Database.ExecuteSqlRawAsync("DELETE FROM [AppointmentLocations]");
        await db.Database.ExecuteSqlRawAsync("DELETE FROM [Participants]");
    }

    [Test, Order(1)]
    public async Task Benchmark_EfCore_vs_RawSql() =>
        await RunBenchmark(EfCoreQuery, RawSqlQuery);

    [Test, Order(2)]
    public async Task Verify_ResultsAreIdentical()
    {
        var (_, efResults) = await Timed(EfCoreQueryWithResults);
        var (_, rawResults) = await Timed(RawSqlQueryWithResults);

        var efIds = efResults.Select(r => r.AppointmentParticipantId).Order().ToList();
        var rawIds = rawResults.Select(r => r.AppointmentParticipantId).Order().ToList();
        Assert.That(rawIds, Is.EqualTo(efIds));

        var ef1 = efResults.OrderBy(r => r.AppointmentParticipantId).First();
        var raw1 = rawResults.OrderBy(r => r.AppointmentParticipantId).First();

        Assert.Multiple(() =>
        {
            Assert.That(raw1.Date, Is.EqualTo(ef1.Date));
            Assert.That(raw1.TimeFrom, Is.EqualTo(ef1.TimeFrom));
            Assert.That(raw1.AdvisorName, Is.EqualTo(ef1.AdvisorName));
            Assert.That(raw1.GroupEventType, Is.EqualTo(ef1.GroupEventType));
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
        foreach (var s in sql.TakeLast(3))
            TestContext.Out.WriteLine(s);
    }

    // ── EF Core — mirrors AppointmentServiceEndpoint.GetAppointments ──

    private static IQueryable<BenchmarkAppointmentParticipant> BuildEfQuery(BenchmarkDbContext db) =>
        db.AppointmentParticipants
            .Where(x => TargetParticipantIds.Any(y => y == x.ParticipantId))
            .Where(x => !OnlyFutureDates || x.Appointment!.Date >= DateTime.Today.Date)
            .Include(x => x.Appointment)
                .ThenInclude(x => x!.AppointmentUsers)
                .ThenInclude(x => x.User)
            .Include(x => x.Appointment)
                .ThenInclude(x => x!.GroupEvent)
            .Include(x => x.Appointment)
                .ThenInclude(x => x!.Location);

    private static async Task<(long Ms, int Rows)> EfCoreQuery()
    {
        await using var db = Db();
        var (ms, results) = await Timed(() => BuildEfQuery(db).ToListAsync());
        return (ms, results.Count);
    }

    private static async Task<List<AppointmentQueryResult>> EfCoreQueryWithResults()
    {
        await using var db = Db();
        var results = await BuildEfQuery(db).ToListAsync();

        return results.Select(ap => new AppointmentQueryResult(
            ap.Id,
            ap.Appointment!.Date,
            ap.Appointment.TimeFrom,
            ap.Appointment.TimeTo,
            (int)ap.Appointment.Type,
            ap.Appointment.GroupEvent?.GroupEventType is { } get ? (int)get : null,
            ap.Appointment.Location?.Name,
            ap.Appointment.AppointmentUsers.Count != 0
                ? ap.Appointment.AppointmentUsers.First().User?.FullName
                : null,
            ap.Appointment.AppointmentUsers.Count != 0
                ? ap.Appointment.AppointmentUsers.First().UserId
                : null
        )).ToList();
    }

    // ── Raw SQL with OUTER APPLY TOP 1 ──

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

    private static async Task<List<AppointmentQueryResult>> RawSqlQueryWithResults()
    {
        await using var db = Db();
        var conn = db.Database.GetDbConnection();
        await conn.OpenAsync();
        try { return await ExecuteRaw(conn); }
        finally { await conn.CloseAsync(); }
    }

    private static async Task<List<AppointmentQueryResult>> ExecuteRaw(System.Data.Common.DbConnection conn)
    {
        using var cmd = conn.CreateCommand();

        var inParams = TargetParticipantIds.Select((id, i) =>
        {
            var name = $"@pid{i}";
            AddParam(cmd, name, id);
            return name;
        }).ToList();

        var futureFilter = OnlyFutureDates
            ? "AND a.[Date] >= CAST(GETDATE() AS DATE)"
            : "";

        cmd.CommandText = $@"
            SELECT
                ap.[Id]              AS AppointmentParticipantId,
                a.[Date],
                a.[TimeFrom],
                a.[TimeTo],
                a.[Type]             AS AppointmentType,
                ge.[GroupEventType],
                loc.[Name]           AS LocationName,
                firstUser.[FullName] AS AdvisorName,
                firstUser.[UserId]   AS AdvisorId
            FROM [AppointmentParticipants] ap
            INNER JOIN [Appointments] a          ON a.[Id] = ap.[AppointmentId]
            LEFT  JOIN [GroupEvents] ge          ON ge.[Id] = a.[GroupEventId]
            LEFT  JOIN [AppointmentLocations] loc ON loc.[Id] = a.[LocationId]
            OUTER APPLY (
                SELECT TOP 1 au.[UserId], u.[FullName]
                FROM [AppointmentUsers] au
                INNER JOIN [AspNetUsers] u ON u.[Id] = au.[UserId]
                WHERE au.[AppointmentId] = a.[Id]
                ORDER BY au.[Id]
            ) firstUser
            WHERE ap.[ParticipantId] IN ({string.Join(", ", inParams)}) {futureFilter}";

        using var reader = await cmd.ExecuteReaderAsync();
        var results = new List<AppointmentQueryResult>();

        while (await reader.ReadAsync())
        {
            results.Add(new AppointmentQueryResult(
                AppointmentParticipantId: reader.GetGuid(O("AppointmentParticipantId")),
                Date: reader.GetDateTime(O("Date")),
                TimeFrom: reader.GetDateTime(O("TimeFrom")),
                TimeTo: reader.GetDateTime(O("TimeTo")),
                AppointmentType: reader.GetInt32(O("AppointmentType")),
                GroupEventType: reader.IsDBNull(O("GroupEventType")) ? null : reader.GetInt32(O("GroupEventType")),
                LocationName: reader.IsDBNull(O("LocationName")) ? null : reader.GetString(O("LocationName")),
                AdvisorName: reader.IsDBNull(O("AdvisorName")) ? null : reader.GetString(O("AdvisorName")),
                AdvisorId: reader.IsDBNull(O("AdvisorId")) ? null : reader.GetGuid(O("AdvisorId"))
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