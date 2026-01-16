// Returning back to our Motor Pool project now I see my weak places. Reporting actually is a good example because here we have 2 separate classifications
// Type which could be say VehicleMileage or DriverMileage or even FuelConsumption and TimePeriod which could be say DailyTimePeriod, ThreeDaysTimePeriod, MonthTimePeriod
// and others. Putting them into one hierarchy or encoding something as enumerable fields(I used both :) ) are not optimal solutions. However now I see that we could create two separate hierarchies
// for that.

public abstract class ReportType
{
    public abstract string ReportName { get; }

    public abstract void GenerateReportData(DateTime startDate, DateTime endDate);
}

public class VehicleMileageReportType : ReportType
{
    public override string ReportName => "Vehicle Mileage Report";

    public override void GenerateReportData(DateTime startDate, DateTime endDate)
    {
        ...
    }
}

public abstract class TimePeriod
{
    public abstract string PeriodName { get; }

    public abstract List<(DateTime StartDate, DateTime EndDate)> GetPeriodDates();
}

public class DailyTimePeriod : TimePeriod
{
    public override string PeriodName => "Daily";

    public override List<(DateTime StartDate, DateTime EndDate)> GetPeriodDates()
    {
        ...
    }
}

public class Report
{
    private ReportType _reportType;
    private TimePeriod _timePeriod;

    public Report(ReportType reportType, TimePeriod timePeriod)
    {
        _reportType = reportType;
        _timePeriod = timePeriod;
    }

    public void Generate()
    {
        ...
    }
}