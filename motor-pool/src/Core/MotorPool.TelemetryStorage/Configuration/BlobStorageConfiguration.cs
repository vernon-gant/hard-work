namespace MotorPool.TelemetryStorage.Configuration;

public class BlobStorageConfiguration
{
    public string ConnectionString { get; set; } = string.Empty;

    public string ContainerName { get; set; } = string.Empty;
}