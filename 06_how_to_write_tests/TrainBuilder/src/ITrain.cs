using OneOf;
using OneOf.Types;

namespace TrainBuilder;

public interface ITrain
{
    OneOf<Success, Error<string>> InsertCarriage(BaseCarriage baseCarriage, int idx);

    OneOf<List<ICarriage>, IdxOutOfRange> GetAfterIdx(int idx);

    int Count { get; }

    OneOf<string, EmptyTrain> Print { get; }
}

public struct EmptyTrain;

public struct IdxOutOfRange;