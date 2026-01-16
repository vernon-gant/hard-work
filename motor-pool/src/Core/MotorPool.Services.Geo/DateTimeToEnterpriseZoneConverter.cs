using AutoMapper;

namespace MotorPool.Services.Geo;

public class DateTimeToEnterpriseZoneConverter : IValueConverter<DateTime,DateTime>
{

    public DateTime Convert(DateTime sourceMember, ResolutionContext context)
    {
        TimeZoneInfo enterpriseTimeZone = context.Items["EnterpriseTimeZone"] as TimeZoneInfo ?? throw new InvalidOperationException();
        return TimeZoneInfo.ConvertTimeFromUtc(sourceMember, enterpriseTimeZone);
    }

}