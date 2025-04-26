using FluentValidation;

namespace TrainBuilder;

public class InsertionValidator : AbstractValidator<InsertionContext>
{
    public InsertionValidator()
    {
        RuleFor(x => x.Idx).GreaterThanOrEqualTo(0).WithMessage("Idx must be greater than or equal to 0");
    }
}