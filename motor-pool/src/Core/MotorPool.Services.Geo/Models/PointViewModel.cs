using System.Globalization;
using System.Text.Json.Serialization;

namespace MotorPool.Services.Geo.Models;

public class PointViewModel
{

    public string Latitude { get; private set; } = string.Empty;

    [JsonIgnore]
    public double LatitudeDouble
    {
        init => Latitude = value.ToString("F6", CultureInfo.InvariantCulture);
    }

    public string Longitude { get; private set; } = string.Empty;

    [JsonIgnore]
    public double LongitudeDouble
    {
        init => Longitude = value.ToString("F6", CultureInfo.InvariantCulture);
    }

}