using System.ComponentModel.DataAnnotations;

using MotorPool.Domain.Reports;

namespace MotorPool.Services.Reporting.DTO;

public class ReportDTO
{
    [Required]
    public DateOnly StartTime { get; set; }

    [Required]
    public DateOnly EndTime { get; set; }

    [Required]
    public Period Period { get; set; }
}