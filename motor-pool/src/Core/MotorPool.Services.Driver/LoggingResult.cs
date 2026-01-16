namespace MotorPool.Services.Drivers;

public abstract class LoggingResult { }

public class NotRunYet : LoggingResult { }

public abstract class Failure : LoggingResult
{
    public abstract string ErrorMessage { get; }
}

public class FileNotFound : Failure
{
    public override string ErrorMessage => "File not found";
}

public class LogNotFound : Failure
{
    public override string ErrorMessage => "Log not found";
}

public class IOError : Failure
{
    public override string ErrorMessage => "Input/output error";
}

public class Success : LoggingResult { }

public class ReadSuccess : Success
{
    public required AssignmentChangeLog Log { get; init; }
}