namespace MotorPool.SpeedAlertService.Messages;

public class CANTelemetry
{
    public int VehicleId { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public double Speed { get; set; }

    public DateTime Timestamp { get; set; }

    public CANTelemetryPayload ToPayload => new()
                                            {
                                                Speed = (int)Speed,
                                                VehicleId = VehicleId
                                            };
}