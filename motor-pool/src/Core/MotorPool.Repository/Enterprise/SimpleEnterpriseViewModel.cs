using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MotorPool.Repository.Enterprise;

public class SimpleEnterpriseViewModel
{
    public int EnterpriseId { get; set; }

    [MaxLength(100)]
    public required string Name { get; set; }

    [MaxLength(100)]
    public required string City { get; set; }

    [MaxLength(100)]
    public required string Street { get; set; }

    [MaxLength(100)]
    public required string VAT { get; set; }

    [Display(Name = "Founded on")]
    public DateOnly FoundedOn { get; set; }

    [Display(Name = "Time Zone")]
    public string TimeZoneId { get; set; } = string.Empty;

    [JsonIgnore]
    public List<int> ManagerIds { get; set; } = new();
}