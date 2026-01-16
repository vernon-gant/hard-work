using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace MotorPool.TelemetryService;


public static class TelemetryProcessor
{
    private static readonly Subject<CANTelemetry> _telemetrySubject = new();

    public static IObservable<CANTelemetry> TelemetryStream => _telemetrySubject.AsObservable();

    public static void Process(CANTelemetry telemetry)
    {
        _telemetrySubject.OnNext(telemetry);
    }

    public static void SpeedFilter()
    {
        var filtered = TelemetryStream
                      .Where(t => t.Speed > 50)
                      .Select(t => new
                                   {
                                       t.VehicleId,
                                       t.Latitude,
                                       t.Longitude,
                                       t.Speed,
                                       t.Timestamp,
                                       Alert = t.Speed > 80 ? "Over Speeding" : "Normal Speed"
                                   });

        filtered.Subscribe(data =>
        {
            Console.WriteLine($"Vehicle {data.VehicleId} at ({data.Latitude}, {data.Longitude}) " + $"is moving at {data.Speed} km/h. Status: {data.Alert}");
        });
    }
}