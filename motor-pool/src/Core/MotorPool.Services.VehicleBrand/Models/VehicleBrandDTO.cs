using System.ComponentModel.DataAnnotations;
using MotorPool.Domain;

namespace MotorPool.Services.VehicleBrand.Models;

public class VehicleBrandDTO
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    [Display(Name = "Company name")]
    public required string CompanyName { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 2)]
    [Display(Name = "Model name")]
    public required string ModelName { get; set; }

    [Required]
    public required VehicleType Type { get; set; }

    [Required]
    [Range(0, 1000000)]
    [Display(Name = "Fuel tank capacity (liters)")]
    public decimal FuelTankCapacityLiters { get; set; }

    [Required]
    [Range(0, 1000000)]
    [Display(Name = "Payload capacity (kg)")]
    public decimal PayloadCapacityKg { get; set; }

    [Required]
    [Range(0, 1000000)]
    [Display(Name = "Number of seats")]
    public int NumberOfSeats { get; set; }

    [Required]
    [Range(1900, 2100)]
    [Display(Name = "Release year")]
    public int ReleaseYear { get; set; }
}