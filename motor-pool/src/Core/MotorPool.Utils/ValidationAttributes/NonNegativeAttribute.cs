using System.ComponentModel.DataAnnotations;

namespace MotorPool.Utils.ValidationAttributes;

public class NonNegativeAttribute : ValidationAttribute
{
    public required bool CanBeZero { get; set; } = true;

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null) return ValidationResult.Success!;

        if (value is not int inputValue) return new ValidationResult("The input values must be an int");

        if (CanBeZero && inputValue >= 0 || !CanBeZero && inputValue >= 1) return ValidationResult.Success!;

        return new ValidationResult("Negative integer or can be null is not satisfied.");
    }

}