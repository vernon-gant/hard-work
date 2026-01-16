using System.ComponentModel.DataAnnotations;

namespace MotorPool.Utils.ValidationAttributes;

[AttributeUsage(AttributeTargets.Property)]
public class ValidUTCDateTimeAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not DateTime inputDateTime || inputDateTime.Kind != DateTimeKind.Utc)
            return new ValidationResult("The input values must be a utc DateTime");

        if (inputDateTime < DateTime.Now.ToUniversalTime() && inputDateTime > DateTime.MinValue)
            return ValidationResult.Success!;

        return new ValidationResult("Negative integer or can be null is not satisfied.");
    }
}