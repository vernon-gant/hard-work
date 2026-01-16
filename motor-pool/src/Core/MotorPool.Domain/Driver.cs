using System.ComponentModel.DataAnnotations;

namespace MotorPool.Domain;

public class Driver
{

    [Key]
    public int DriverId { get; set; }

    [MaxLength(100)]
    public string FirstName { get; set; } = null!;

    [MaxLength(100)]
    public string LastName { get; set; } = null!;

    public decimal Salary { get; set; }

    public int? EnterpriseId { get; set; }

    public Enterprise? Enterprise { get; set; }

    public List<DriverVehicle> DriverVehicles { get; set; } = new ();

    public int? ActiveVehicleId { get; set; }

    public Vehicle? ActiveVehicle { get; set; }

}