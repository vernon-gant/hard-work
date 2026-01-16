namespace MotorPool.Services.Drivers;

public class AssignmentChangeLog
{
    public Guid LogId { get; set; } = Guid.NewGuid();
    public required int DriverId { get; set; }
    public required int VehicleId { get; set; }
    public required string Action { get; set; }
    public DateTime LoggedAt { get; set; } = DateTime.UtcNow;
}