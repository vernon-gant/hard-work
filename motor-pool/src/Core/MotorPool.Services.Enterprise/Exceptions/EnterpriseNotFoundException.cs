namespace MotorPool.Services.Enterprise.Exceptions;

public class EnterpriseNotFoundException : Exception
{

    public EnterpriseNotFoundException(string message) : base(message) { }

    public EnterpriseNotFoundException(string message, Exception innerException) : base(message, innerException) { }

}