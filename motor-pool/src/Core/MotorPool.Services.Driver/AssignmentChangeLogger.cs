using MotorPool.Domain;

namespace MotorPool.Services.Drivers;

public abstract class AssignmentChangeLogger(string filePath)
{
    protected string FilePath { get; set; } = filePath;

    protected LoggingResult _lastAppendResult = new NotRunYet();
    protected LoggingResult _lastDeleteResult = new NotRunYet();
    protected LoggingResult _lastReadResult = new NotRunYet();

    // Commands
    public abstract void AppendLog(DriverVehicle assignment); // post-condition: log is appended
    public abstract void DeleteLog(Guid logId); // post-condition: log with logId is deleted

    // Queries
    public abstract LoggingResult ReadLog(Guid logId); // pre-condition: file and log with logId exist
    public abstract LoggingResult LastAppendResult { get; } // returns status of the last append operation
    public abstract LoggingResult LastDeleteResult { get; } // returns status of the last delete operation
    public abstract LoggingResult LastReadResult { get; } // returns status of the last read operation
}
