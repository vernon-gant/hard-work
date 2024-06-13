using System.Linq;
using FluentValidation;

namespace Import.Improved;

public class HeaderValidator : AbstractValidator<HeaderValidationContext>
{
    public HeaderValidator(ISpecialAttributeRegistry registry)
    {
        RuleFor(x => x)
            .Must(x => x.AssetHeader.SequenceEqual(x.RequiredAssetHeader))
            .WithMessage("Asset header does not match required asset header");

        RuleFor(x => x.SpecialAttributeTitles)
            .Must(x => x.All(registry.Exists))
            .WithMessage((_, list) => $"Following special attributes do not exist: {string.Join(", ", list.Where(x => !registry.Exists(x)))}");
    }
}