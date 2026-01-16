namespace MotorPool.Services.Enterprise.Exceptions;

public class NameIsTakenException : Exception
{

    public NameIsTakenException(string message) : base(message)
    {
    }

    public NameIsTakenException(string message, Exception innerException) : base(message, innerException)
    {
    }

}