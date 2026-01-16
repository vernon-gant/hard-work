using MotorPool.Domain;
using Serilog.Core;
using Serilog.Events;

namespace MotorPool.API.Serilog;

public class GeoPointDestructuringPolicy : IDestructuringPolicy
{
    public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue? result)
    {
        if (value is not GeoPoint geoPoint)
        {
            result = null;
            return false;
        }

        result = new StructureValue(new[]
                                    {
                                        new LogEventProperty("Latitude", new ScalarValue(geoPoint.Latitude)),
                                        new LogEventProperty("Longitude", new ScalarValue(geoPoint.Longitude)),
                                        new LogEventProperty("VehicleId", new ScalarValue(geoPoint.VehicleId))
                                    });

        return true;
    }
}