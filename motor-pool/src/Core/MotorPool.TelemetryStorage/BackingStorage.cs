using MotorPool.TelemetryStorage.Messages;

namespace MotorPool.TelemetryStorage;

public interface BackingStorage
{
    ValueTask AppendTelemetryAsync(CANTelemetry telemetry, CancellationToken cancellationToken);

    ValueTask<List<CANTelemetry>> GetTelemetryAsync(int vehicleId, CancellationToken cancellationToken);
}