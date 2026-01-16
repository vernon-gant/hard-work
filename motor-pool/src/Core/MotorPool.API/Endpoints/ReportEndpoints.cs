using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MotorPool.Domain.Reports;
using MotorPool.Services.Reporting.Core;
using MotorPool.Services.Reporting.DTO;

namespace MotorPool.API.Endpoints;

public static class ReportEndpoints
{
    public static void MapReportEndpoints(this IEndpointRouteBuilder managerResourcesGroupBuilder)
    {
        RouteGroupBuilder enterprisesGroupBuilder = managerResourcesGroupBuilder.MapGroup("reports");

        enterprisesGroupBuilder.MapPost("vehicle-mileage", VehicleMileage);
    }

    private static async ValueTask<IResult> VehicleMileage([FromBody] VehicleMileageReportDTO reportDto, [FromServices] ReportService<VehicleMileageReport, VehicleMileageReportDTO> reportService, IMemoryCache memoryCache)
    {
        string key = $"vehicle-mileage-report-{JsonSerializer.Serialize(reportDto)}";

        if (memoryCache.TryGetValue<VehicleMileageReport>(key, out var report)) return Results.Ok(report);

        VehicleMileageReport freshReport = await reportService.Generate(reportDto);

        memoryCache.Set(key, freshReport);

        return Results.Ok(freshReport);
    }
}