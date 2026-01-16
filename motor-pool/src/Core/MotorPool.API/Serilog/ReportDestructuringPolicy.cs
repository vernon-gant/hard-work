using MotorPool.Domain.Reports;
using Serilog.Core;
using Serilog.Events;

namespace MotorPool.API.Serilog;

public class ReportDestructuringPolicy : IDestructuringPolicy
{
    public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue? result)
    {
        if (value is not AbstractReport report)
        {
            result = null;
            return false;
        }

        result = new StructureValue(new[]
                                    {
                                        new LogEventProperty("Type", new ScalarValue(report.Type)),
                                        new LogEventProperty("Period", new ScalarValue(report.Period)),
                                        new LogEventProperty("StartTime", new ScalarValue(report.StartTime)),
                                        new LogEventProperty("EndTime", new ScalarValue(report.EndTime))
                                    });

        return true;
    }
}