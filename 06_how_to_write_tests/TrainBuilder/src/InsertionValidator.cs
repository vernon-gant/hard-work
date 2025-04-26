using FluentValidation;

namespace TrainBuilder;

public class InsertionValidator : AbstractValidator<InsertionContext>
{
    private const int MaxTrainSize = 10;

    public InsertionValidator()
    {
        RuleFor(x => x).Must(HaveIndexBetweenZeroAndCurrentTrainCount).WithMessage("Idx must be greater than or equal to 0");

        RuleFor(x => x.Train).Must(BeNotFull).WithMessage("Train is full. Insertion is not possible");
    }

    private static bool HaveIndexBetweenZeroAndCurrentTrainCount(InsertionContext context) => context.Idx >= 0 && context.Idx < context.Train.Count;

    private static bool BeNotFull(ITrain train) => train.Count < MaxTrainSize;
}