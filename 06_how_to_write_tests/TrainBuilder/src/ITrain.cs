using OneOf;
using OneOf.Types;

namespace TrainBuilder;

public interface ITrain
{
    OneOf<Success, Error<string>> InsertCarriage(BaseCarriage baseCarriage, int idx);

    OneOf<List<BaseCarriage>, CarriageOutOfRange> GetAfterIdx(int idx);

    int Count { get; }

    string Print { get; }
}

public struct CarriageOutOfRange;