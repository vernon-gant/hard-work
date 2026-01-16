namespace MotorPool.SpeedAlertService;

public interface NotificationClient
{
    ValueTask PushAlert(CANTelemetryPayload telemetry);
}