using System.ComponentModel.DataAnnotations;

namespace MotorPool.Services.Enterprise.Models;

public class EnterpriseDTO
{
    [MaxLength(100)]
    public required string Name { get; set; }

    [MaxLength(100)]
    public required string City { get; set; }

    [MaxLength(100)]
    public required string Street { get; set; }

    [MaxLength(100)]
    public required string VAT { get; set; }
}