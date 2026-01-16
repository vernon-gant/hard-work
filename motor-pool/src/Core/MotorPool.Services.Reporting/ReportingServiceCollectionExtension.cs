using Microsoft.Extensions.DependencyInjection;

using MotorPool.Domain.Reports;
using MotorPool.Services.Reporting.Core;
using MotorPool.Services.Reporting.DTO;

namespace MotorPool.Services.Reporting;

public static class ReportingServiceCollectionExtension
{
    public static void AddReporting(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddAutoMapper(typeof(ReportingServiceCollectionExtension));
        serviceCollection.AddScoped<ReportGenerator<VehicleMileageReport>, VehicleMileageReportGenerator>();
        serviceCollection.AddScoped<ReportPeriodResolver<VehicleMileageReport>,DayResolver<VehicleMileageReport>>();
        serviceCollection.AddScoped<ReportPeriodResolver<VehicleMileageReport>,MonthResolver<VehicleMileageReport>>();
        serviceCollection.AddScoped<ReportPeriodResolver<VehicleMileageReport>,YearResolver<VehicleMileageReport>>();
        serviceCollection.AddScoped<ReportService<VehicleMileageReport, VehicleMileageReportDTO>>();
    }
}