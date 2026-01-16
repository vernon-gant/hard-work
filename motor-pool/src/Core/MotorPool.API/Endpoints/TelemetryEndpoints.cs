using Microsoft.AspNetCore.Mvc;
using MotorPool.API.Messages;
using MotorPool.API.Producers;

namespace MotorPool.API.Endpoints;

public static class TelemetryEndpoints
{
    public static void MapTelemetryEndpoints(this IEndpointRouteBuilder managerResourcesGroupBuilder)
    {
        RouteGroupBuilder telemetryGroupBuilder = managerResourcesGroupBuilder.MapGroup("telemetry")
                                                                            .WithParameterValidation();

        telemetryGroupBuilder.MapPost("", ProduceTelemetry)
                            .WithName("ProduceTelemetry");
    }

    public static async Task<IResult> ProduceTelemetry(CANTelemetry canTelemetry, [FromServices] TelemetryProducer telemetryProducer)
    {
        await telemetryProducer.ProduceTelemetryAsync(canTelemetry);
        return Results.Ok();
    }
}