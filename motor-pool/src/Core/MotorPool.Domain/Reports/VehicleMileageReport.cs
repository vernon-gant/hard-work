namespace MotorPool.Domain.Reports;

public class VehicleMileageReport : AbstractReport
{
    public override string Type => "Vehicle mileage report";

    public int VehicleId { get; set; }
}