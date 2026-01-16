using OneOf;
using OneOf.Types;

namespace TrainBuilder;

/// <summary>
/// Used to store <see cref="ICarriage"/>. Insertion happens with the 0 based indexes where the last possible index is the append index(equal to current count).
/// We call it a "valid index" when we can perform the insert command when the train is not full. The user manipulates train to add new carriages or print it.
/// </summary>
public interface ITrain
{
    #region Commands

    // Precondition: train is not full, idx in range
    // Postcondition: All carriages starting from the provided idx are shifted to the right and new carriage is inserted
    OneOf<Success, Error<string>> InsertCarriage(ICarriage carriage, int idx);

    #endregion

    #region Queries

    // Precondition: idx is in valid range
    OneOf<List<ICarriage>, IdxOutOfRange> GetFromIdxInclusive(int idx);

    int Count { get; }

    // Precondition: train is not full
    OneOf<string, EmptyTrain> Printing { get; }

    #endregion
}

public struct EmptyTrain;

public struct IdxOutOfRange;