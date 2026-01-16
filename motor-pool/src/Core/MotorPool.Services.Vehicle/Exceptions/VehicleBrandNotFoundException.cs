namespace MotorPool.Services.Vehicles.Exceptions;

public class VehicleBrandNotFoundException : Exception
{

    public VehicleBrandNotFoundException(string message) : base(message) { }

    public VehicleBrandNotFoundException(string message, Exception innerException) : base(message, innerException) { }

}