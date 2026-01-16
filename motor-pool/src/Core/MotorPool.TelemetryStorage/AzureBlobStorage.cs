using System.Text;
using System.Text.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Extensions.Options;
using MotorPool.TelemetryStorage.Configuration;
using MotorPool.TelemetryStorage.Messages;

namespace MotorPool.TelemetryStorage;

public class AzureBlobStorage(IOptions<BlobStorageConfiguration> options, ILogger<AzureBlobStorage> logger) : BackingStorage
{
    private readonly BlobServiceClient _blobServiceClient = new(options.Value.ConnectionString);

    public async ValueTask AppendTelemetryAsync(CANTelemetry telemetry, CancellationToken cancellationToken)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(options.Value.ContainerName);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        AppendBlobClient blobClient = containerClient.GetAppendBlobClient($"{telemetry.VehicleId.ToString()}/{telemetry.Timestamp.Date:dd-MM-yyyy}");
        await blobClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        string serializedTelemetry = JsonSerializer.Serialize(telemetry);
        byte[] data = Encoding.UTF8.GetBytes(serializedTelemetry);
        MemoryStream stream = new(data);
        await blobClient.AppendBlockAsync(stream, cancellationToken: cancellationToken);
        logger.LogTrace("Telemetry appended: {Telemetry}", serializedTelemetry);
    }

    public async ValueTask<List<CANTelemetry>> GetTelemetryAsync(int vehicleId, CancellationToken cancellationToken)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(options.Value.ContainerName);
        BlobClient blobClient = containerClient.GetBlobClient(vehicleId.ToString());
        await using var telemetryStream = await blobClient.OpenReadAsync(cancellationToken: cancellationToken);
        using MemoryStream memoryStream = new();
        await telemetryStream.CopyToAsync(memoryStream, cancellationToken);
        string serializedTelemetry = Encoding.UTF8.GetString(memoryStream.ToArray());
        return JsonSerializer.Deserialize<List<CANTelemetry>>(serializedTelemetry) ?? new List<CANTelemetry>();
    }
}