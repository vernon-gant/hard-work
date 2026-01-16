using System.Text;
using System.Text.Json;
using MotorPool.SpeedAlertService.Messages;

namespace MotorPool.SpeedAlertService;

public class HTTPLogicAppNotificationClient(HttpClient httpClient, ILogger<HTTPLogicAppNotificationClient> logger) : NotificationClient
{
    public async ValueTask PushAlert(CANTelemetryPayload telemetry)
    {
        StringContent payload = new(JsonSerializer.Serialize(telemetry), Encoding.UTF8, "application/json");
        HttpResponseMessage response = await httpClient.PostAsync(string.Empty, payload);

        if (response.IsSuccessStatusCode)
        {
            logger.LogInformation("Telemetry sent successfully");
            return;
        }

        logger.LogError("Failed to send telemetry");
    }
}