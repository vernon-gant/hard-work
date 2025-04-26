using FluentValidation;
using OneOf;
using OneOf.Types;

namespace TrainBuilder;

public class ListTrain(IValidator<InsertionContext> insertionValidator) : ITrain
{
    private readonly List<ICarriage> _carriages = [];

    public OneOf<Success, Error<string>> InsertCarriage(ICarriage carriage, int idx)
    {
        var validationResult = insertionValidator.Validate(new InsertionContext(this, idx, carriage));

        if (!validationResult.IsValid)
            return new Error<string>(validationResult.Errors[0].ErrorMessage);

        _carriages.Insert(idx, carriage);

        return new Success();
    }

    public OneOf<List<ICarriage>, IdxOutOfRange> GetFromIdxInclusive(int idx)
    {
        if (_carriages.Count == 0)
            return new IdxOutOfRange();

        return _carriages.GetRange(idx, Count - idx);
    }

    public int Count => _carriages.Count;

    public OneOf<string, EmptyTrain> Printing
    {
        get
        {
            if (_carriages.Count == 0)
                return new EmptyTrain();

            return string.Empty;
        }
    }
}