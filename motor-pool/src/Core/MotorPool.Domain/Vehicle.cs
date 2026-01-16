using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MotorPool.Domain;

public class Vehicle
{
    public int VehicleId { get; set; }

    [MinLength(17)]
    [MaxLength(17)]
    [Required]
    public string MotorVIN { get; set; } = string.Empty;

    [DisplayFormat(DataFormatString = "{0:C}")]
    public decimal Cost { get; set; }

    [Required]
    public int ManufactureYear { get; set; }

    [MaxLength(100)]
    public string ManufactureLand { get; set; } = string.Empty;

    [Required]
    public decimal Mileage { get; set; }

    public DateTime AcquiredOn { get; set; }

    [NotMapped]
    public string AcquiredOnInEnterpriseTimeZone
    {
        get
        {
            string enterpriseTimeZoneId = Enterprise?.TimeZoneId ?? throw new InvalidOperationException("Enterprise is not set.");
            TimeZoneInfo enterpriseTimeZone = TimeZoneInfo.FindSystemTimeZoneById(enterpriseTimeZoneId);
            DateTime acquiredOnInEnterpriseZone = TimeZoneInfo.ConvertTimeFromUtc(AcquiredOn, enterpriseTimeZone);

            return new DateTimeOffset(acquiredOnInEnterpriseZone, enterpriseTimeZone.GetUtcOffset(AcquiredOn)).ToString("o");
        }
    }

    public int VehicleBrandId { get; set; }

    public VehicleBrand? VehicleBrand { get; set; }

    public int EnterpriseId { get; set; }

    public Enterprise? Enterprise { get; set; }

    public List<DriverVehicle> DriverVehicles { get; set; } = new();

    public List<Trip> Trips { get; set; } = new();

    public List<GeoPoint> GeoPoints { get; set; } = new();

    public bool IsManagerAccessible(int managerId) => Enterprise!.IsManagerAccessible(managerId);
}