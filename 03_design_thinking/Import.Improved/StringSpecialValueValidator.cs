using FluentValidation;

namespace Import.Improved;

public class StringSpecialValueValidator : AbstractValidator<ParsableValueSpecialAttribute>
{
    public StringSpecialValueValidator()
    {
        RuleFor(x => x)
            .Must(x => x.IsParsable)
            .WithMessage(x => $"Special attribute '{x.Title}' can not be parsed. '{x.MustHaveFormatMessage}'");
    }
}