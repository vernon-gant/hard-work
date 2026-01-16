using System.ComponentModel.DataAnnotations;

namespace MotorPool.Services.Reporting.DTO;

public class VehicleMileageReportDTO : ReportDTO
{
    [Required]
    public int VehicleId { get; set; }

    public static VehicleMileageReportDTO FromReportDTO(ReportDTO reportDTO)
    {
        return new VehicleMileageReportDTO
               {
                   StartTime = reportDTO.StartTime,
                   EndTime = reportDTO.EndTime,
                   Period = reportDTO.Period
               };
    }
}