using FluentValidation;
using OneOf;
using OneOf.Types;

namespace TrainBuilder;

public class ListTrain(IValidator<InsertionContext> insertionValidator) : ITrain
{
    private readonly List<ICarriage> _carriages = [];

    public OneOf<Success, Error<string>> InsertCarriage(ICarriage carriage, int idx)
    {
        throw new NotImplementedException();
    }

    public OneOf<List<ICarriage>, IdxOutOfRange> GetFromIdxInclusive(int idx)
    {
        throw new NotImplementedException();
    }

    public int Count => _carriages.Count;

    public OneOf<string, EmptyTrain> Print
    {
        get
        {
            if (_carriages.Count == 0)
                return new EmptyTrain();

            return string.Empty;
        }
    }
}