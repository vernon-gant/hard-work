using System.Diagnostics;

namespace EntityBenchmark;

public abstract class BenchmarkBase
{
    protected const int WarmupRuns = 3;
    protected const int MeasuredRuns = 20;

    protected async Task RunBenchmark(
        Func<Task<(long Ms, int Rows)>> efCoreRun,
        Func<Task<(long Ms, int Rows)>> rawSqlRun)
    {
        for (int i = 0; i < WarmupRuns; i++)
        {
            await efCoreRun();
            await rawSqlRun();
        }

        var efTimes = new List<long>(MeasuredRuns);
        var rawTimes = new List<long>(MeasuredRuns);
        int efCount = 0, rawCount = 0;

        for (int i = 0; i < MeasuredRuns; i++)
        {
            (var efMs, efCount) = await efCoreRun();
            (var rawMs, rawCount) = await rawSqlRun();
            efTimes.Add(efMs);
            rawTimes.Add(rawMs);
        }

        PrintReport(efTimes, rawTimes, efCount, rawCount);
        Assert.That(efCount, Is.EqualTo(rawCount), "Both queries must return the same number of rows");
    }

    protected static async Task<(long Ms, T Result)> Timed<T>(Func<Task<T>> action)
    {
        var sw = Stopwatch.StartNew();
        var result = await action();
        sw.Stop();
        return (sw.ElapsedMilliseconds, result);
    }

    protected static BenchmarkDbContext Db() => BenchmarkDatabaseSetup.CreateDbContext();

    private static void PrintReport(List<long> efTimes, List<long> rawTimes, int efCount, int rawCount)
    {
        var metric = "Metric";
        var efLabel = "EF Core";
        var rawLabel = "Raw SQL";
        var ratioLabel = "Ratio";

        TestContext.Out.WriteLine();
        TestContext.Out.WriteLine($"{metric,-30} {efLabel,12} {rawLabel,12} {ratioLabel,10}");
        TestContext.Out.WriteLine(new string('-', 66));
        TestContext.Out.WriteLine($"{"Rows returned",-30} {efCount,12} {rawCount,12}");
        PrintTiming("Min (ms)", efTimes.Min(), rawTimes.Min());
        PrintTiming("Max (ms)", efTimes.Max(), rawTimes.Max());
        PrintTiming("Median (ms)", Median(efTimes), Median(rawTimes));
        PrintTiming("Avg (ms)", efTimes.Average(), rawTimes.Average());
        PrintTiming("P95 (ms)", Percentile(efTimes, 95), Percentile(rawTimes, 95));
    }

    private static void PrintTiming(string label, double ef, double raw)
    {
        var ratio = raw > 0 ? ef / raw : 0;
        TestContext.Out.WriteLine($"{label,-30} {ef,12:F1} {raw,12:F1} {ratio,9:F2}x");
    }

    private static double Median(List<long> values)
    {
        var sorted = values.OrderBy(x => x).ToList();
        int mid = sorted.Count / 2;
        return sorted.Count % 2 == 0
            ? (sorted[mid - 1] + sorted[mid]) / 2.0
            : sorted[mid];
    }

    private static double Percentile(List<long> values, int percentile)
    {
        var sorted = values.OrderBy(x => x).ToList();
        int index = (int)Math.Ceiling(percentile / 100.0 * sorted.Count) - 1;
        return sorted[Math.Max(0, index)];
    }
}