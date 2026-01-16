using MotorPool.Domain.Reports;

namespace MotorPool.Services.Reporting.Core;

public interface ReportGenerator<in T> where T : AbstractReport
{
    ValueTask GenerateByDay(T report);

    ValueTask GenerateByMonth(T report);

    ValueTask GenerateByYear(T report);
}