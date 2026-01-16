using MotorPool.Domain;
using Serilog.Core;
using Serilog.Events;

namespace MotorPool.API.Serilog;

public class VehicleDestructuringPolicy : IDestructuringPolicy
{
    public bool TryDestructure(object value, ILogEventPropertyValueFactory propertyValueFactory, out LogEventPropertyValue? result)
    {
        if (value is not Vehicle vehicle)
        {
            result = null;
            return false;
        }

        result = new StructureValue(new[]
                                    {
                                        new LogEventProperty("VehicleId", new ScalarValue(vehicle.VehicleId)),
                                        new LogEventProperty("VehicleBrandId", new ScalarValue(vehicle.VehicleBrandId)),
                                        new LogEventProperty("EnterpriseId", new ScalarValue(vehicle.EnterpriseId)),
                                        new LogEventProperty("MotorVin", new ScalarValue(vehicle.MotorVIN))
                                    });

        return true;
    }
}