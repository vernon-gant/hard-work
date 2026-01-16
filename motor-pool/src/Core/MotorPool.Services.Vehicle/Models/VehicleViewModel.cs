using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MotorPool.Services.Vehicles.Models;

public class VehicleViewModel : VehicleDTO
{
    public int VehicleId { get; set; }

    [Display(Name = "Company name")]
    public string CompanyName { get; set; } = string.Empty;

    [Display(Name = "Model name")]
    public string ModelName { get; set; } = string.Empty;

    [Display(Name = "Acquired on")] public new string AcquiredOn { get; set; } = string.Empty;

    [Display(Name = "All trips amount")]
    public int TotalTripsCount { get; set; }

    public List<int> DriverIds { get; set; } = new ();

    public List<int> TripIds { get; set; } = new ();

    [JsonIgnore]
    public List<int> ManagerIds { get; set; } = new ();

}