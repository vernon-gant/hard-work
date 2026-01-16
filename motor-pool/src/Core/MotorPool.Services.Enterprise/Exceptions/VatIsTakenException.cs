namespace MotorPool.Services.Enterprise.Exceptions;

public class VatIsTakenException : Exception
{

    public VatIsTakenException(string message) : base(message) { }

    public VatIsTakenException(string message, Exception innerException) : base(message, innerException) { }

}