namespace MotorPool.Services.Vehicles.Exceptions;

public class VINAlreadyExistsException : Exception
{

    public VINAlreadyExistsException(string message) : base(message) { }

    public VINAlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }

    public VINAlreadyExistsException() { }

}