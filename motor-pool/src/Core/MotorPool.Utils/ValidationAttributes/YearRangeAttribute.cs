using System.ComponentModel.DataAnnotations;

namespace MotorPool.Utils.ValidationAttributes;

public class YearRangeAttribute : ValidationAttribute
{
    public required int MinYear { get; init; }

    public override bool IsValid(object? value)
    {
        if (value is not int year) return false;

        return year >= MinYear && year <= DateTime.Now.Year;
    }
}