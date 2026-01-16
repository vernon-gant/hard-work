namespace MotorPool.SpeedAlertService.Configuration;

public class ApacheKafkaConfiguration
{
    public string BootstrapServers { get; set; } = string.Empty;

    public string TelemetryTopic { get; set; } = string.Empty;

    public string GroupId { get; set; } = string.Empty;
}