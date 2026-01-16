using System.ComponentModel.DataAnnotations;
using MotorPool.Utils.ValidationAttributes;

namespace MotorPool.Services.Vehicles.Models;

public class VehicleDTO
{
    [MinLength(17)]
    [MaxLength(17)]
    [Display(Name = "VIN")]
    [Required]
    public string MotorVIN { get; set; } = string.Empty;

    [Required]
    [Range(1, 1000000)]
    public decimal Cost { get; set; }

    [Display(Name = "Manufacture year")]
    [YearRange(MinYear = 2000)]
    [Required]
    public int ManufactureYear { get; set; }

    [Display(Name = "Manufacture land")]
    [Required]
    [ExistingCounty]
    public string ManufactureLand { get; set; } = string.Empty;

    [Display(Name = "Mileage (km)")]
    [Range(0, 1000000)]
    public decimal Mileage { get; set; }

    [Required]
    [ValidUTCDateTime]
    public DateTime AcquiredOn { get; set; }

    [Display(Name = "Vehicle brand")]
    [Required]
    public int VehicleBrandId { get; set; }

    [Required]
    public int EnterpriseId { get; set; }
}