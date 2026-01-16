namespace MotorPool.Services.Drivers;

public abstract class TransactionResult;

public class TransactionSuccess : TransactionResult;

public class TransactionError(string errorMessage) : TransactionResult
{
    public string ErrorMessage { get; } = errorMessage;
}