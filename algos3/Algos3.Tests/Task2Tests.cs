using Algos3;

namespace Algos3.Tests;

public class Task2Tests
{
    [Fact]
    public void GIVEN_TeacherWorkedExample_WHEN_InsertionSortStepAppliedSequentiallyForEachIndex_THEN_ArrayMatchesEachTracedState()
    {
        var array = new[] { 7, 6, 5, 4, 3, 2, 1 };

        Task2.InsertionSortStep(array, 3, 0);
        Assert.Equal(new[] { 1, 6, 5, 4, 3, 2, 7 }, array);

        Task2.InsertionSortStep(array, 3, 1);
        Assert.Equal(new[] { 1, 3, 5, 4, 6, 2, 7 }, array);

        Task2.InsertionSortStep(array, 3, 2);
        Assert.Equal(new[] { 1, 3, 2, 4, 6, 5, 7 }, array);

        Task2.InsertionSortStep(array, 3, 3);
        Assert.Equal(new[] { 1, 3, 2, 4, 6, 5, 7 }, array);
    }

    [Fact]
    public void GIVEN_TaskPromptExampleArray_WHEN_InsertionSortStepCalledWithStepThreeAtIndexOne_THEN_MinimumOfGappedTailSwappedIn()
    {
        var array = new[] { 1, 6, 5, 4, 3, 2, 7 };

        Task2.InsertionSortStep(array, 3, 1);

        Assert.Equal(new[] { 1, 3, 5, 4, 6, 2, 7 }, array);
    }

    [Fact]
    public void GIVEN_UnitStepWithMultiElementForwardTail_WHEN_InsertionSortStepCalled_THEN_OnlyGlobalMinimumSwappedIntoIndexI()
    {
        var array = new[] { 1, 3, 5, 7, 2, 6 };

        Task2.InsertionSortStep(array, 1, 3);

        Assert.Equal(new[] { 1, 3, 5, 2, 6, 7 }, array);
    }

    [Fact]
    public void GIVEN_ReverseSortedForwardTail_WHEN_InsertionSortStepCalled_THEN_SmallestOfTailSwappedInAndElementsBeforeIUntouched()
    {
        var array = new[] { 5, 4, 3, 2, 1 };

        Task2.InsertionSortStep(array, 1, 1);

        Assert.Equal(new[] { 5, 1, 2, 3, 4 }, array);
    }

    [Fact]
    public void GIVEN_AlreadySortedTail_WHEN_InsertionSortStepCalled_THEN_ArrayUnchanged()
    {
        var array = new[] { 1, 2, 3, 4, 5 };

        Task2.InsertionSortStep(array, 1, 2);

        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, array);
    }

    [Fact]
    public void GIVEN_DuplicateValuesInForwardTail_WHEN_InsertionSortStepCalled_THEN_FirstOccurrenceOfMinimumKeptAsReferenceUntilStrictlySmallerFound()
    {
        var array = new[] { 5, 3, 3, 3, 1 };

        Task2.InsertionSortStep(array, 1, 1);

        Assert.Equal(new[] { 5, 1, 3, 3, 3 }, array);
    }

    [Fact]
    public void GIVEN_MinimalTwoElementArray_WHEN_InsertionSortStepCalledAtIndexZero_THEN_OutOfOrderPairSwapped()
    {
        var array = new[] { 3, 1 };

        Task2.InsertionSortStep(array, 1, 0);

        Assert.Equal(new[] { 1, 3 }, array);
    }

    [Fact]
    public void GIVEN_UnitStep_WHEN_InsertionSortStepCalledAtLastValidIndex_THEN_OnlyTrailingPairSwapped()
    {
        var array = new[] { 1, 2, 3, 0 };

        Task2.InsertionSortStep(array, 1, 2);

        Assert.Equal(new[] { 1, 2, 0, 3 }, array);
    }

    [Fact]
    public void GIVEN_StepGreaterThanOneAtIndexZero_WHEN_InsertionSortStepCalled_THEN_OnlyGappedPairIsAffected()
    {
        var array = new[] { 9, 1, 5, 2 };

        Task2.InsertionSortStep(array, 2, 0);

        Assert.Equal(new[] { 5, 1, 9, 2 }, array);
    }
}
