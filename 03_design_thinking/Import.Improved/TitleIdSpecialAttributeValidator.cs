using FluentValidation;

namespace Import.Improved;

public class TitleIdSpecialAttributeValidator : AbstractValidator<TitleIdSpecialAttribute>
{
    public TitleIdSpecialAttributeValidator(ISpecialAttributeValueRegistry valueRegistry)
    {
        RuleFor(x => x)
            .Must(x => x.ContainsValidTitles(valueRegistry))
            .WithMessage(x => $"Special attribute '{x.Title}' values are not valid. Check the possible values.");
    }
}