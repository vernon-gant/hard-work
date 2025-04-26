using FluentValidation;

namespace TrainBuilder;

public class InsertionValidator : AbstractValidator<InsertionContext>
{
    private const int MaxTrainSize = 10;

    public InsertionValidator()
    {
        RuleFor(x => x.Idx).GreaterThanOrEqualTo(0).WithMessage("Idx must be greater than or equal to 0");

        RuleFor(x => x.Train).Must(BeNotFull).WithMessage("Train is full. Insertion is not possible");
    }

    private static bool BeNotFull(ITrain train) => train.Count < MaxTrainSize;
}