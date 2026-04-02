using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EntityBenchmark;

[TestFixture]
public class ProjectQueryBenchmarkTests : BenchmarkBase
{
    private const int ProjectCount = 500_000;

    [OneTimeSetUp]
    public async Task Seed()
    {
        var table = BuildDataTable();
        var rng = new Random(999);

        for (int i = 0; i < ProjectCount; i++)
        {
            var id = Guid.NewGuid();
            var hasEnd = rng.NextDouble() < 0.6;
            var start = new DateTime(2020, 1, 1).AddDays(rng.Next(0, 1500));

            table.Rows.Add(
                id,                                                     // Id
                (long)i,                                                // Key
                $"Projekt {i}",                                         // Designation
                $"MN-{rng.Next(1000000, 9999999)}",                     // MeasureNumber
                rng.NextDouble() < 0.8 ? Guid.NewGuid() : DBNull.Value,// ContactPersonId
                rng.NextDouble() < 0.3 ? Guid.NewGuid() : DBNull.Value,// SecondaryContactPersonId
                start,                                                  // ProjectStart
                hasEnd ? start.AddMonths(rng.Next(6, 36)) : DBNull.Value,// ProjectEnd
                hasEnd ? start.AddMonths(rng.Next(36, 48)) : DBNull.Value,// EndOfContinuingCare
                hasEnd ? start.AddMonths(rng.Next(48, 120)) : DBNull.Value,// RetentionObligation
                rng.NextDouble() < 0.2,                                 // AutomaticAnonymization
                rng.NextDouble() < 0.5 ? rng.Next(10, 200) : DBNull.Value,// SustainabilityTargetNumber
                rng.NextDouble() < 0.4 ? $"CC-{rng.Next(1000, 9999)}" : DBNull.Value,// ContractorCostObjectId
                false,                                                  // Anonymized
                DBNull.Value,                                           // AnonymizedOn
                $"Strasse {rng.Next(1, 200)}",                          // Street
                $"{rng.Next(1000, 9999)}",                              // ZipCode
                $"Stadt {i % 50}",                                      // Location
                rng.NextDouble() < 0.9 ? Guid.NewGuid() : DBNull.Value,// CountryId
                $"+43{rng.Next(100000, 999999)}",                       // Phone
                rng.NextDouble() < 0.3 ? $"+43{rng.Next(100000, 999999)}" : DBNull.Value,// Fax
                $"projekt{i}@oesb.at",                                  // Email
                rng.NextDouble() < 0.5 ? $"ATU{rng.Next(10000000, 99999999)}" : DBNull.Value,// VatNumber
                rng.NextDouble() < 0.7 ? Guid.NewGuid() : DBNull.Value,// ProjectMeasureId
                rng.Next(5, 30),                                        // DaysBtwConsultationsLimit
                rng.Next(7, 21),                                        // DaysBtwConsultationsAfterCareLimit
                rng.Next(14, 30),                                       // DaysSinceF2FOrangeLimit
                rng.Next(21, 42),                                       // DaysSinceF2FRedLimit
                rng.Next(21, 45),                                       // DaysSinceF2FOrangeAfterCareLimit
                rng.Next(30, 90),                                       // DaysSinceF2FRedAfterCareLimit
                rng.NextDouble() < 0.8,                                 // AllowTransfersFromExternalProjects
                Math.Round((decimal)(rng.NextDouble() * 20), 2),        // AverageJobPlacementsPerMonth
                rng.NextDouble() < 0.1 ? Guid.NewGuid() : DBNull.Value,// FollowUpProjectId
                rng.NextDouble() < 0.5 ? Guid.NewGuid() : DBNull.Value,// AmsContingentId
                rng.NextDouble() < 0.5 ? Guid.NewGuid() : DBNull.Value,// ProjectCategoryContingentId
                rng.NextDouble() < 0.6 ? Guid.NewGuid() : DBNull.Value,// MunicipalId
                rng.NextDouble() < 0.6 ? Guid.NewGuid() : DBNull.Value,// DistrictId
                rng.NextDouble() < 0.8 ? Guid.NewGuid() : DBNull.Value,// FederalStateId
                $"Stadt {i % 50}",                                      // City
                rng.Next(0, 5)                                          // ProjectCategory
            );
        }

        await using var db = Db();
        var conn = (SqlConnection)db.Database.GetDbConnection();
        await conn.OpenAsync();

        using var bulk = new SqlBulkCopy(conn) { DestinationTableName = "Projects" };
        foreach (DataColumn col in table.Columns)
            bulk.ColumnMappings.Add(col.ColumnName, col.ColumnName);
        await bulk.WriteToServerAsync(table);

        await conn.CloseAsync();
        TestContext.Out.WriteLine($"Seeded {ProjectCount} projects via SqlBulkCopy");
    }

    [OneTimeTearDown]
    public async Task Cleanup()
    {
        await using var db = Db();
        await db.Database.ExecuteSqlRawAsync("DELETE FROM [Projects]");
    }

    [Test, Order(1)]
    public async Task Benchmark_EfCore_vs_RawSql() =>
        await RunBenchmark(EfCoreQuery, RawSqlQuery);

    [Test, Order(2)]
    public async Task Verify_RowCounts()
    {
        await using var db = Db();
        var efCount = await db.Projects.AsNoTracking().CountAsync();

        var conn = db.Database.GetDbConnection();
        await conn.OpenAsync();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM [Projects]";
        var rawCount = (int)(await cmd.ExecuteScalarAsync())!;
        await conn.CloseAsync();

        Assert.That(efCount, Is.EqualTo(rawCount));
        Assert.That(efCount, Is.EqualTo(ProjectCount));
        TestContext.Out.WriteLine($"Verified: {efCount} rows in both paths");
    }

    [Test, Order(3)]
    public async Task ShowGeneratedSql()
    {
        var logs = new List<string>();
        await using var db = BenchmarkDatabaseSetup.CreateDbContextWithLogging(msg => logs.Add(msg));

        _ = await db.Projects.AsNoTracking().ToListAsync();

        TestContext.Out.WriteLine("=== EF Core Generated SQL + Execution Time ===");
        foreach (var line in logs.Where(l => l.Contains("Executed DbCommand") || l.Contains("SELECT")))
            TestContext.Out.WriteLine(line);
    }

    // ── EF Core: dbContext.Projects.ToListAsync() ──

    private static async Task<(long Ms, int Rows)> EfCoreQuery()
    {
        await using var db = Db();
        var (ms, results) = await Timed(() => db.Projects.AsNoTracking().ToListAsync());
        return (ms, results.Count);
    }

    // ── Raw SQL: SELECT * with manual reader ──

    private static async Task<(long Ms, int Rows)> RawSqlQuery()
    {
        await using var db = Db();
        var conn = db.Database.GetDbConnection();
        await conn.OpenAsync();
        try
        {
            var (ms, count) = await Timed(async () =>
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM [Projects]";
                using var reader = await cmd.ExecuteReaderAsync();

                int rows = 0;
                while (await reader.ReadAsync())
                {
                    // Read every column to make the comparison fair —
                    // the reader must deserialize bytes into CLR types,
                    // just like EF Core's materializer does.
                    _ = reader.GetGuid(0);            // Id
                    _ = reader.GetInt64(1);            // Key
                    _ = reader.GetString(2);           // Designation
                    _ = reader.GetString(3);           // MeasureNumber
                    if (!reader.IsDBNull(4)) _ = reader.GetGuid(4);
                    if (!reader.IsDBNull(5)) _ = reader.GetGuid(5);
                    _ = reader.GetDateTime(6);         // ProjectStart
                    if (!reader.IsDBNull(7)) _ = reader.GetDateTime(7);
                    if (!reader.IsDBNull(8)) _ = reader.GetDateTime(8);
                    if (!reader.IsDBNull(9)) _ = reader.GetDateTime(9);
                    _ = reader.GetBoolean(10);
                    if (!reader.IsDBNull(11)) _ = reader.GetInt32(11);
                    if (!reader.IsDBNull(12)) _ = reader.GetString(12);
                    _ = reader.GetBoolean(13);
                    if (!reader.IsDBNull(14)) _ = reader.GetDateTime(14);
                    if (!reader.IsDBNull(15)) _ = reader.GetString(15);
                    if (!reader.IsDBNull(16)) _ = reader.GetString(16);
                    if (!reader.IsDBNull(17)) _ = reader.GetString(17);
                    if (!reader.IsDBNull(18)) _ = reader.GetGuid(18);
                    if (!reader.IsDBNull(19)) _ = reader.GetString(19);
                    if (!reader.IsDBNull(20)) _ = reader.GetString(20);
                    if (!reader.IsDBNull(21)) _ = reader.GetString(21);
                    if (!reader.IsDBNull(22)) _ = reader.GetString(22);
                    if (!reader.IsDBNull(23)) _ = reader.GetGuid(23);
                    _ = reader.GetInt32(24);
                    _ = reader.GetInt32(25);
                    _ = reader.GetInt32(26);
                    _ = reader.GetInt32(27);
                    _ = reader.GetInt32(28);
                    _ = reader.GetInt32(29);
                    _ = reader.GetBoolean(30);
                    _ = reader.GetDecimal(31);
                    if (!reader.IsDBNull(32)) _ = reader.GetGuid(32);
                    if (!reader.IsDBNull(33)) _ = reader.GetGuid(33);
                    if (!reader.IsDBNull(34)) _ = reader.GetGuid(34);
                    if (!reader.IsDBNull(35)) _ = reader.GetGuid(35);
                    if (!reader.IsDBNull(36)) _ = reader.GetGuid(36);
                    if (!reader.IsDBNull(37)) _ = reader.GetGuid(37);
                    if (!reader.IsDBNull(38)) _ = reader.GetString(38);
                    _ = reader.GetInt32(39);
                    rows++;
                }

                return rows;
            });
            return (ms, count);
        }
        finally { await conn.CloseAsync(); }
    }

    // ── Schema for SqlBulkCopy ──

    private static DataTable BuildDataTable()
    {
        var t = new DataTable();
        t.Columns.Add("Id", typeof(Guid));
        t.Columns.Add("Key", typeof(long));
        t.Columns.Add("Designation", typeof(string));
        t.Columns.Add("MeasureNumber", typeof(string));
        t.Columns.Add("ContactPersonId", typeof(Guid));
        t.Columns.Add("SecondaryContactPersonId", typeof(Guid));
        t.Columns.Add("ProjectStart", typeof(DateTime));
        t.Columns.Add("ProjectEnd", typeof(DateTime));
        t.Columns.Add("EndOfContinuingCare", typeof(DateTime));
        t.Columns.Add("RetentionObligation", typeof(DateTime));
        t.Columns.Add("AutomaticAnonymization", typeof(bool));
        t.Columns.Add("SustainabilityTargetNumber", typeof(int));
        t.Columns.Add("ContractorCostObjectId", typeof(string));
        t.Columns.Add("Anonymized", typeof(bool));
        t.Columns.Add("AnonymizedOn", typeof(DateTime));
        t.Columns.Add("Street", typeof(string));
        t.Columns.Add("ZipCode", typeof(string));
        t.Columns.Add("Location", typeof(string));
        t.Columns.Add("CountryId", typeof(Guid));
        t.Columns.Add("Phone", typeof(string));
        t.Columns.Add("Fax", typeof(string));
        t.Columns.Add("Email", typeof(string));
        t.Columns.Add("VatNumber", typeof(string));
        t.Columns.Add("ProjectMeasureId", typeof(Guid));
        t.Columns.Add("DaysBtwConsultationsLimit", typeof(int));
        t.Columns.Add("DaysBtwConsultationsAfterCareLimit", typeof(int));
        t.Columns.Add("DaysSinceF2FOrangeLimit", typeof(int));
        t.Columns.Add("DaysSinceF2FRedLimit", typeof(int));
        t.Columns.Add("DaysSinceF2FOrangeAfterCareLimit", typeof(int));
        t.Columns.Add("DaysSinceF2FRedAfterCareLimit", typeof(int));
        t.Columns.Add("AllowTransfersFromExternalProjects", typeof(bool));
        t.Columns.Add("AverageJobPlacementsPerMonth", typeof(decimal));
        t.Columns.Add("FollowUpProjectId", typeof(Guid));
        t.Columns.Add("AmsContingentId", typeof(Guid));
        t.Columns.Add("ProjectCategoryContingentId", typeof(Guid));
        t.Columns.Add("MunicipalId", typeof(Guid));
        t.Columns.Add("DistrictId", typeof(Guid));
        t.Columns.Add("FederalStateId", typeof(Guid));
        t.Columns.Add("City", typeof(string));
        t.Columns.Add("ProjectCategory", typeof(int));

        // Allow DBNull for nullable columns
        foreach (DataColumn col in t.Columns)
            col.AllowDBNull = true;

        return t;
    }
}