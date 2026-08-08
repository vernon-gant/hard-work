using Algos3;

namespace Algos3.Tests;

public class Task1Tests
{
    [Fact]
    public void GIVEN_TypicalArray_WHEN_SelectionSortStepCalled_THEN_MinimumFromRemainderSwappedIntoPositionI()
    {
        var array = new[] { 5, 3, 8, 1, 4 };

        Task1.SelectionSortStep(array, 1);

        Assert.Equal(new[] { 5, 1, 8, 3, 4 }, array);
    }

    [Fact]
    public void GIVEN_EmptyArray_WHEN_SelectionSortStepCalled_THEN_ArrayRemainsEmpty()
    {
        var array = Array.Empty<int>();

        Task1.SelectionSortStep(array, 0);

        Assert.Empty(array);
    }

    [Fact]
    public void GIVEN_SingleElementArray_WHEN_SelectionSortStepCalledAtIndexZero_THEN_ArrayUnchanged()
    {
        var array = new[] { 42 };

        Task1.SelectionSortStep(array, 0);

        Assert.Equal(new[] { 42 }, array);
    }

    [Fact]
    public void GIVEN_AlreadySortedArray_WHEN_SelectionSortStepCalled_THEN_ArrayUnchanged()
    {
        var array = new[] { 1, 2, 3, 4, 5 };

        Task1.SelectionSortStep(array, 0);

        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, array);
    }

    [Fact]
    public void GIVEN_ReverseSortedArray_WHEN_SelectionSortStepCalledAtIndexZero_THEN_MinimumMovedToFront()
    {
        var array = new[] { 5, 4, 3, 2, 1 };

        Task1.SelectionSortStep(array, 0);

        Assert.Equal(new[] { 1, 4, 3, 2, 5 }, array);
    }

    [Fact]
    public void GIVEN_ArrayWithDuplicateMinimums_WHEN_SelectionSortStepCalled_THEN_FirstOccurrenceOfMinimumSwapped()
    {
        var array = new[] { 3, 1, 2, 1, 5 };

        Task1.SelectionSortStep(array, 0);

        Assert.Equal(new[] { 1, 3, 2, 1, 5 }, array);
    }

    [Fact]
    public void GIVEN_TypicalArray_WHEN_SelectionSortStepCalledAtLastValidIndex_THEN_ArrayUnchanged()
    {
        var array = new[] { 3, 1, 2, 4, 0 };

        Task1.SelectionSortStep(array, array.Length - 1);

        Assert.Equal(new[] { 3, 1, 2, 4, 0 }, array);
    }

    [Fact]
    public void GIVEN_UnsortedArrayWithSwapsNeeded_WHEN_BubbleSortStepCalled_THEN_AdjacentOutOfOrderElementsSwappedAndFalseReturned()
    {
        var array = new[] { 3, 1, 2 };

        var result = Task1.BubbleSortStep(array);

        Assert.Equal(new[] { 1, 2, 3 }, array);
        Assert.False(result);
    }

    [Fact]
    public void GIVEN_AlreadySortedArray_WHEN_BubbleSortStepCalled_THEN_NoSwapsPerformedAndTrueReturned()
    {
        var array = new[] { 1, 2, 3, 4, 5 };

        var result = Task1.BubbleSortStep(array);

        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, array);
        Assert.True(result);
    }

    [Fact]
    public void GIVEN_ReverseSortedArray_WHEN_BubbleSortStepCalled_THEN_LargestElementBubblesToEndAndFalseReturned()
    {
        var array = new[] { 5, 4, 3, 2, 1 };

        var result = Task1.BubbleSortStep(array);

        Assert.Equal(new[] { 4, 3, 2, 1, 5 }, array);
        Assert.False(result);
    }

    [Fact]
    public void GIVEN_EmptyArray_WHEN_BubbleSortStepCalled_THEN_TrueReturnedAndArrayRemainsEmpty()
    {
        var array = Array.Empty<int>();

        var result = Task1.BubbleSortStep(array);

        Assert.Empty(array);
        Assert.True(result);
    }

    [Fact]
    public void GIVEN_SingleElementArray_WHEN_BubbleSortStepCalled_THEN_TrueReturnedAndArrayUnchanged()
    {
        var array = new[] { 7 };

        var result = Task1.BubbleSortStep(array);

        Assert.Equal(new[] { 7 }, array);
        Assert.True(result);
    }

    [Fact]
    public void GIVEN_ArrayWithDuplicateElements_WHEN_BubbleSortStepCalled_THEN_EqualAdjacentElementsNotSwapped()
    {
        var array = new[] { 2, 2, 2 };

        var result = Task1.BubbleSortStep(array);

        Assert.Equal(new[] { 2, 2, 2 }, array);
        Assert.True(result);
    }
}
