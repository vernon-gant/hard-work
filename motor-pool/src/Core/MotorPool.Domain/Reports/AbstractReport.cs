namespace MotorPool.Domain.Reports;

public abstract class AbstractReport
{
    public required DateOnly StartTime { get; init; }

    public required DateOnly EndTime { get; init; }

    public required Period Period { get; init; }

    public Dictionary<string, string> Result { get; private set; } = new ();

    public abstract string Type { get; }
}