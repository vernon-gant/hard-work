using System.Text.Json;
using System.Transactions;
using Microsoft.Extensions.Logging;
using MotorPool.Domain;

namespace MotorPool.Services.Drivers;

public class TransactionalAssignmentFileLogger(string filePath, ILogger<TransactionalAssignmentFileLogger> logger) : AssignmentChangeLogger(filePath), IEnlistmentNotification
{
    private bool _enlisted;

    public override void AppendLog(DriverVehicle assignment)
    {
        AssignmentChangeLog log = new AssignmentChangeLog
                                  {
                                      LogId = Guid.NewGuid(),
                                      DriverId = assignment.DriverId,
                                      VehicleId = assignment.VehicleId,
                                      Action = "Assigned",
                                      LoggedAt = DateTime.Now
                                  };
        if (Enlist()) AppendLog(log);
    }

    private bool Enlist()
    {
        if (_enlisted) return true;

        var currentTx = Transaction.Current;

        if (currentTx == null) return false;

        currentTx.EnlistVolatile(this, EnlistmentOptions.None);
        _enlisted = true;
        return true;
    }

    private void AppendLog(AssignmentChangeLog log)
    {
        try
        {
            string json = SerializeLog(log);
            File.AppendAllText(FilePath, json + Environment.NewLine);
            _lastAppendResult = new Success();
        }
        catch (Exception)
        {
            _lastAppendResult = new IOError();
        }
    }

    public override void DeleteLog(Guid logId)
    {
        try
        {
            if (!File.Exists(FilePath))
            {
                _lastDeleteResult = new FileNotFound();
                return;
            }

            string[] lines = File.ReadAllLines(FilePath);
            string[] newLines = lines.Where(line => !line.Contains(logId.ToString()))
                                     .ToArray();
            File.WriteAllLines(FilePath, newLines);
            _lastDeleteResult = new Success();
        }
        catch (Exception)
        {
            _lastDeleteResult = new IOError();
        }
    }

    public override LoggingResult ReadLog(Guid logId)
    {
        try
        {
            if (!File.Exists(FilePath))
            {
                _lastReadResult = new LogNotFound();
                return _lastReadResult;
            }

            string[] lines = File.ReadAllLines(FilePath);
            string? line = lines.FirstOrDefault(l => l.Contains(logId.ToString()));
            if (line == null)
            {
                _lastReadResult = new LogNotFound();
                return _lastReadResult;
            }

            AssignmentChangeLog log = DeserializeLog(line);
            _lastReadResult = new ReadSuccess { Log = log };
            return _lastReadResult;
        }
        catch (Exception)
        {
            _lastReadResult = new IOError();
            return _lastReadResult;
        }
    }

    public override LoggingResult LastAppendResult => _lastAppendResult;

    public override LoggingResult LastDeleteResult => _lastDeleteResult;

    public override LoggingResult LastReadResult => _lastReadResult;

    private string SerializeLog(AssignmentChangeLog log) => $"{log.LogId},{log.DriverId},{log.VehicleId},{log.Action},{log.LoggedAt}";

    private AssignmentChangeLog DeserializeLog(string line)
    {
        string[] parts = line.Split(',');
        return new AssignmentChangeLog
               {
                   LogId = Guid.Parse(parts[0]),
                   DriverId = int.Parse(parts[1]),
                   VehicleId = int.Parse(parts[2]),
                   Action = parts[3],
                   LoggedAt = DateTime.Parse(parts[4])
               };
    }

    public void Commit(Enlistment enlistment)
    {
        _enlisted = false;
        enlistment.Done();
    }

    public void InDoubt(Enlistment enlistment)
    {
        _enlisted = false;
        enlistment.Done();
    }

    public void Prepare(PreparingEnlistment preparingEnlistment)
    {
        preparingEnlistment.Prepared();
    }

    public void Rollback(Enlistment enlistment)
    {
        try
        {
            var lines = File.ReadAllLines(FilePath);
            File.WriteAllLines(FilePath, lines[..^1]);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to rollback transaction");
            throw;
        }

        _enlisted = false;
        enlistment.Done();
    }
}