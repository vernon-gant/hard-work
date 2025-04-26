using FluentValidation;

namespace TrainBuilder;

public class InsertionValidator : AbstractValidator<InsertionContext>
{
    private const int MaxTrainSize = 10;

    public InsertionValidator()
    {
        RuleFor(x => x).Must(HaveIndexBetweenZeroAndCurrentTrainCount).WithMessage("Position not possible!");

        RuleFor(x => x.Train).Must(BeNotFull).WithMessage("Train too long!");
    }

    private static bool HaveIndexBetweenZeroAndCurrentTrainCount(InsertionContext context) => context.Idx >= 0 && context.Idx < context.Train.Count;

    private static bool BeNotFull(ITrain train) => train.Count < MaxTrainSize;
}