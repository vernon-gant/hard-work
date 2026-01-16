using System.ComponentModel.DataAnnotations;

namespace MotorPool.Domain;

public class VehicleBrand
{
    public int VehicleBrandId { get; set; }

    [Required]
    [MaxLength(100)]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ModelName { get; set; } = string.Empty;

    [Required]
    public VehicleType Type { get; set; }

    [Required]
    public decimal FuelTankCapacityLiters { get; set; }

    [Required]
    public decimal PayloadCapacityKg { get; set; }

    [Required]
    public int NumberOfSeats { get; set; }

    public int ReleaseYear { get; set; }

    public List<Vehicle> Vehicles { get; set; } = new();
}

public enum VehicleType
{
    [Display(Name = "Passenger car")]
    PassengerCar,

    [Display(Name = "Truck")]
    Truck,

    [Display(Name = "Bus")]
    Bus
}