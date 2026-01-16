using System.ComponentModel.DataAnnotations;

namespace MotorPool.Services.Drivers.Models;

public class DriverDTO
{

    [Display(Name = "First name")]
    public required string FirstName { get; set; }

    [Display(Name = "Last name")]
    public required string LastName { get; set; }

    public decimal Salary { get; set; }

}