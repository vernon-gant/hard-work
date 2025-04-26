using System.Diagnostics;
using FluentValidation;

namespace TrainBuilder;

public class InsertionValidator : AbstractValidator<InsertionContext>
{
    private const int MaxTrainSize = 10;
    private const int SleeperCarriageRulePassengerCarriageCount = 2;

    public InsertionValidator()
    {
        RuleFor(x => x)
            .Must(HaveIndexBetweenZeroAndCurrentTrainCount)
            .WithMessage("Position not possible!")
            .Must(BeNotFull)
            .WithMessage("Train too long!")
            .DependentRules(() =>
            {
                RuleFor(x => x)
                    .Must(SatisfySleeperCarriageRule)
                    .WithMessage("Sleeper only possible directly before two passenger carriages!")
                    .When(x => x.Carriage is SleeperCarriage);
            });
    }

    private static bool HaveIndexBetweenZeroAndCurrentTrainCount(InsertionContext context) => context.Idx >= 0 && context.Idx < context.Train.Count;

    private static bool BeNotFull(InsertionContext context) => context.Train.Count < MaxTrainSize;

    private static bool SatisfySleeperCarriageRule(InsertionContext context)
    {
        var afterIdxCarriagesResult = context.Train.GetFromIdxInclusive(context.Idx);

        Debug.Assert(afterIdxCarriagesResult.IsT0);

        var afterIdxCarriages = afterIdxCarriagesResult.AsT0;

        return afterIdxCarriages.Count >= SleeperCarriageRulePassengerCarriageCount && afterIdxCarriages.Take(SleeperCarriageRulePassengerCarriageCount).All(carriage => carriage is PassengerCarriage);
    }
}