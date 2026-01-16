using MotorPool.Domain.Reports;

namespace MotorPool.Services.Reporting.Core;

public interface ReportPeriodResolver<in T> where T : AbstractReport
{
    bool CanResolve(Period period);
    Func<T,ValueTask> Resolve(Period period);
}

public class DayResolver<T>(ReportGenerator<T> reportGenerator) : ReportPeriodResolver<T> where T : AbstractReport
{
    public bool CanResolve(Period period) => period == Period.Day;

    public Func<T, ValueTask> Resolve(Period period) => reportGenerator.GenerateByDay;
}

public class MonthResolver<T>(ReportGenerator<T> reportGenerator) : ReportPeriodResolver<T> where T : AbstractReport
{
    public bool CanResolve(Period period) => period == Period.Month;

    public Func<T, ValueTask> Resolve(Period period) => reportGenerator.GenerateByMonth;
}

public class YearResolver<T>(ReportGenerator<T> reportGenerator) : ReportPeriodResolver<T> where T : AbstractReport
{
    public bool CanResolve(Period period) => period == Period.Year;

    public Func<T, ValueTask> Resolve(Period period) => reportGenerator.GenerateByYear;
}