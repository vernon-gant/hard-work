package stack

import (
	"testing"
	"github.com/stretchr/testify/assert"
)

// helpers

func makeStack(values ...int) Stack[int] {
	stack := Stack[int]{}
	for _, v := range values {
		stack.Push(v)
	}
	return stack
}

func popAll(stack *Stack[int]) []int {
	var result []int
	for stack.Size() > 0 {
		val, _ := stack.Pop()
		result = append(result, val)
	}
	return result
}

// SIZE

func Test_GivenEmptyStack_WhenGettingSize_ThenReturnsZero(t *testing.T) {
	// Given
	stack := Stack[int]{}

	// When
	size := stack.Size()

	// Then
	assert.Equal(t, 0, size)
}

func Test_GivenSingleElementStack_WhenGettingSize_ThenReturnsOne(t *testing.T) {
	// Given
	stack := makeStack(42)

	// When
	size := stack.Size()

	// Then
	assert.Equal(t, 1, size)
}

func Test_GivenMultipleElementsStack_WhenGettingSize_ThenReturnsCorrectCount(t *testing.T) {
	// Given
	stack := makeStack(1, 2, 3, 4, 5)

	// When
	size := stack.Size()

	// Then
	assert.Equal(t, 5, size)
}

func Test_GivenStack_WhenPushingElements_ThenSizeIncreasesCorrectly(t *testing.T) {
	// Given
	stack := Stack[int]{}

	// When/Then
	assert.Equal(t, 0, stack.Size())

	stack.Push(1)
	assert.Equal(t, 1, stack.Size())

	stack.Push(2)
	assert.Equal(t, 2, stack.Size())

	stack.Push(3)
	assert.Equal(t, 3, stack.Size())
}

// PUSH

func Test_GivenEmptyStack_WhenPushing_ThenSizeBecomesOne(t *testing.T) {
	// Given
	stack := Stack[int]{}

	// When
	stack.Push(10)

	// Then
	assert.Equal(t, 1, stack.Size())
}

func Test_GivenStack_WhenPushingMultipleValues_ThenAllAreStored(t *testing.T) {
	// Given
	stack := Stack[int]{}

	// When
	stack.Push(1)
	stack.Push(2)
	stack.Push(3)

	// Then
	assert.Equal(t, 3, stack.Size())
}

func Test_GivenNonEmptyStack_WhenPushing_ThenSizeIncreases(t *testing.T) {
	// Given
	stack := makeStack(1, 2, 3)
	initialSize := stack.Size()

	// When
	stack.Push(4)

	// Then
	assert.Equal(t, initialSize+1, stack.Size())
}

// POP

func Test_GivenEmptyStack_WhenPopping_ThenReturnsError(t *testing.T) {
	// Given
	stack := Stack[int]{}

	// When
	_, err := stack.Pop()

	// Then
	assert.Error(t, err)
	assert.Equal(t, 0, stack.Size())
}

func Test_GivenSingleElementStack_WhenPopping_ThenReturnsElementAndBecomesEmpty(t *testing.T) {
	// Given
	stack := makeStack(42)

	// When
	val, err := stack.Pop()

	// Then
	assert.NoError(t, err)
	assert.Equal(t, 42, val)
	assert.Equal(t, 0, stack.Size())
}

func Test_GivenMultipleElementsStack_WhenPopping_ThenReturnsLastPushed(t *testing.T) {
	// Given
	stack := makeStack(1, 2, 3)

	// When
	val, err := stack.Pop()

	// Then
	assert.NoError(t, err)
	assert.Equal(t, 3, val)
	assert.Equal(t, 2, stack.Size())
}

func Test_GivenStack_WhenPoppingAll_ThenReturnsElementsInLIFOOrder(t *testing.T) {
	// Given
	stack := makeStack(1, 2, 3, 4, 5)

	// When
	values := popAll(&stack)

	// Then
	assert.Equal(t, []int{5, 4, 3, 2, 1}, values)
	assert.Equal(t, 0, stack.Size())
}

func Test_GivenStack_WhenPoppingAfterEmpty_ThenReturnsError(t *testing.T) {
	// Given
	stack := makeStack(1)
	stack.Pop()

	// When
	_, err := stack.Pop()

	// Then
	assert.Error(t, err)
}

func Test_GivenStack_WhenPoppingMultipleTimes_ThenSizeDecreasesCorrectly(t *testing.T) {
	// Given
	stack := makeStack(1, 2, 3, 4)

	// When/Then
	assert.Equal(t, 4, stack.Size())

	stack.Pop()
	assert.Equal(t, 3, stack.Size())

	stack.Pop()
	assert.Equal(t, 2, stack.Size())
}

// PEEK

func Test_GivenEmptyStack_WhenPeeking_ThenReturnsError(t *testing.T) {
	// Given
	stack := Stack[int]{}

	// When
	_, err := stack.Peek()

	// Then
	assert.Error(t, err)
}

func Test_GivenSingleElementStack_WhenPeeking_ThenReturnsElementWithoutRemoving(t *testing.T) {
	// Given
	stack := makeStack(42)

	// When
	val, err := stack.Peek()

	// Then
	assert.NoError(t, err)
	assert.Equal(t, 42, val)
	assert.Equal(t, 1, stack.Size())
}

func Test_GivenMultipleElementsStack_WhenPeeking_ThenReturnsTopWithoutRemoving(t *testing.T) {
	// Given
	stack := makeStack(1, 2, 3)

	// When
	val, err := stack.Peek()

	// Then
	assert.NoError(t, err)
	assert.Equal(t, 3, val)
	assert.Equal(t, 3, stack.Size())
}

func Test_GivenStack_WhenPeekingMultipleTimes_ThenReturnsSameValue(t *testing.T) {
	// Given
	stack := makeStack(10, 20, 30)

	// When
	val1, err1 := stack.Peek()
	val2, err2 := stack.Peek()
	val3, err3 := stack.Peek()

	// Then
	assert.NoError(t, err1)
	assert.NoError(t, err2)
	assert.NoError(t, err3)
	assert.Equal(t, 30, val1)
	assert.Equal(t, 30, val2)
	assert.Equal(t, 30, val3)
	assert.Equal(t, 3, stack.Size())
}

func Test_GivenStack_WhenPeekingAfterPop_ThenReturnsNewTop(t *testing.T) {
	// Given
	stack := makeStack(1, 2, 3)

	// When
	stack.Pop()
	val, err := stack.Peek()

	// Then
	assert.NoError(t, err)
	assert.Equal(t, 2, val)
	assert.Equal(t, 2, stack.Size())
}

// INTERLEAVED OPERATIONS

func Test_GivenStack_WhenInterleavingPushAndPop_ThenMaintainsLIFOOrder(t *testing.T) {
	// Given
	stack := Stack[int]{}

	// When
	stack.Push(1)
	stack.Push(2)
	val1, _ := stack.Pop()
	stack.Push(3)
	val2, _ := stack.Pop()
	val3, _ := stack.Pop()

	// Then
	assert.Equal(t, 2, val1)
	assert.Equal(t, 3, val2)
	assert.Equal(t, 1, val3)
	assert.Equal(t, 0, stack.Size())
}

func Test_GivenStack_WhenInterleavingPushAndPeek_ThenAlwaysReturnsTop(t *testing.T) {
	// Given
	stack := Stack[int]{}

	// When/Then
	stack.Push(10)
	val1, _ := stack.Peek()
	assert.Equal(t, 10, val1)

	stack.Push(20)
	val2, _ := stack.Peek()
	assert.Equal(t, 20, val2)

	stack.Push(30)
	val3, _ := stack.Peek()
	assert.Equal(t, 30, val3)

	assert.Equal(t, 3, stack.Size())
}

func Test_GivenStack_WhenPushingAfterPoppingAll_ThenWorksCorrectly(t *testing.T) {
	// Given
	stack := makeStack(1, 2, 3)
	popAll(&stack)

	// When
	stack.Push(99)

	// Then
	assert.Equal(t, 1, stack.Size())
	val, err := stack.Peek()
	assert.NoError(t, err)
	assert.Equal(t, 99, val)
}

// BALANCED PARENTHESES

func Test_GivenEmptyString_WhenCheckingBalance_ThenReturnsTrue(t *testing.T) {
	// Given
	input := ""

	// When
	result := IsBalanced(input)

	// Then
	assert.True(t, result)
}

func Test_GivenSingleOpeningParenthesis_WhenCheckingBalance_ThenReturnsFalse(t *testing.T) {
	// Given
	input := "("

	// When
	result := IsBalanced(input)

	// Then
	assert.False(t, result)
}

func Test_GivenSingleClosingParenthesis_WhenCheckingBalance_ThenReturnsFalse(t *testing.T) {
	// Given
	input := ")"

	// When
	result := IsBalanced(input)

	// Then
	assert.False(t, result)
}

func Test_GivenSinglePair_WhenCheckingBalance_ThenReturnsTrue(t *testing.T) {
	// Given
	input := "()"

	// When
	result := IsBalanced(input)

	// Then
	assert.True(t, result)
}

func Test_GivenMultipleSequentialPairs_WhenCheckingBalance_ThenReturnsTrue(t *testing.T) {
	// Given
	input := "()()()"

	// When
	result := IsBalanced(input)

	// Then
	assert.True(t, result)
}

func Test_GivenSimpleNestedPair_WhenCheckingBalance_ThenReturnsTrue(t *testing.T) {
	// Given
	input := "(())"

	// When
	result := IsBalanced(input)

	// Then
	assert.True(t, result)
}

func Test_GivenMultipleNestedPairs_WhenCheckingBalance_ThenReturnsTrue(t *testing.T) {
	// Given
	input := "((()))"

	// When
	result := IsBalanced(input)

	// Then
	assert.True(t, result)
}

func Test_GivenComplexBalancedExpression_WhenCheckingBalance_ThenReturnsTrue(t *testing.T) {
	// Given
	input := "(()((())()))"

	// When
	result := IsBalanced(input)

	// Then
	assert.True(t, result)
}

func Test_GivenMixedNestedAndSequential_WhenCheckingBalance_ThenReturnsTrue(t *testing.T) {
	// Given
	input := "(())(())"

	// When
	result := IsBalanced(input)

	// Then
	assert.True(t, result)
}

func Test_GivenClosingBeforeOpening_WhenCheckingBalance_ThenReturnsFalse(t *testing.T) {
	// Given
	input := ")("

	// When
	result := IsBalanced(input)

	// Then
	assert.False(t, result)
}

func Test_GivenMoreOpeningThanClosing_WhenCheckingBalance_ThenReturnsFalse(t *testing.T) {
	// Given
	input := "((("

	// When
	result := IsBalanced(input)

	// Then
	assert.False(t, result)
}

func Test_GivenMoreClosingThanOpening_WhenCheckingBalance_ThenReturnsFalse(t *testing.T) {
	// Given
	input := ")))"

	// When
	result := IsBalanced(input)

	// Then
	assert.False(t, result)
}

func Test_GivenUnbalancedWithMixedOrder_WhenCheckingBalance_ThenReturnsFalse(t *testing.T) {
	// Given
	input := "())("

	// When
	result := IsBalanced(input)

	// Then
	assert.False(t, result)
}

func Test_GivenStartsWithClosing_WhenCheckingBalance_ThenReturnsFalse(t *testing.T) {
	// Given
	input := "))(("

	// When
	result := IsBalanced(input)

	// Then
	assert.False(t, result)
}

func Test_GivenEndsWithOpening_WhenCheckingBalance_ThenReturnsFalse(t *testing.T) {
	// Given
	input := "((())"

	// When
	result := IsBalanced(input)

	// Then
	assert.False(t, result)
}

func Test_GivenIncompleteExpression_WhenCheckingBalance_ThenReturnsFalse(t *testing.T) {
	// Given
	input := "(()()(("

	// When
	result := IsBalanced(input)

	// Then
	assert.False(t, result)
}

func Test_GivenDeeplyNestedBalanced_WhenCheckingBalance_ThenReturnsTrue(t *testing.T) {
	// Given
	input := "((((((()))))))"

	// When
	result := IsBalanced(input)

	// Then
	assert.True(t, result)
}

func Test_GivenComplexMixedPattern_WhenCheckingBalance_ThenReturnsTrue(t *testing.T) {
	// Given
	input := "()(())(()())"

	// When
	result := IsBalanced(input)

	// Then
	assert.True(t, result)
}

func Test_GivenUnbalancedInMiddle_WhenCheckingBalance_ThenReturnsFalse(t *testing.T) {
	// Given
	input := "(()()"

	// When
	result := IsBalanced(input)

	// Then
	assert.False(t, result)
}

// BALANCED PARENTHESES - MULTIPLE TYPES

func Test_GivenEmptyString_WhenCheckingMultipleTypes_ThenReturnsTrue(t *testing.T) {
	// Given
	input := ""

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.True(t, result)
}

func Test_GivenSingleOpeningRound_WhenCheckingMultipleTypes_ThenReturnsFalse(t *testing.T) {
	// Given
	input := "("

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.False(t, result)
}

func Test_GivenSingleOpeningCurly_WhenCheckingMultipleTypes_ThenReturnsFalse(t *testing.T) {
	// Given
	input := "{"

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.False(t, result)
}

func Test_GivenSingleOpeningSquare_WhenCheckingMultipleTypes_ThenReturnsFalse(t *testing.T) {
	// Given
	input := "["

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.False(t, result)
}

func Test_GivenSingleClosingRound_WhenCheckingMultipleTypes_ThenReturnsFalse(t *testing.T) {
	// Given
	input := ")"

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.False(t, result)
}

func Test_GivenSingleClosingCurly_WhenCheckingMultipleTypes_ThenReturnsFalse(t *testing.T) {
	// Given
	input := "}"

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.False(t, result)
}

func Test_GivenSingleClosingSquare_WhenCheckingMultipleTypes_ThenReturnsFalse(t *testing.T) {
	// Given
	input := "]"

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.False(t, result)
}

func Test_GivenSingleRoundPair_WhenCheckingMultipleTypes_ThenReturnsTrue(t *testing.T) {
	// Given
	input := "()"

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.True(t, result)
}

func Test_GivenSingleCurlyPair_WhenCheckingMultipleTypes_ThenReturnsTrue(t *testing.T) {
	// Given
	input := "{}"

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.True(t, result)
}

func Test_GivenSingleSquarePair_WhenCheckingMultipleTypes_ThenReturnsTrue(t *testing.T) {
	// Given
	input := "[]"

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.True(t, result)
}

func Test_GivenAllThreeTypesSequential_WhenCheckingMultipleTypes_ThenReturnsTrue(t *testing.T) {
	// Given
	input := "(){}[]"

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.True(t, result)
}

func Test_GivenMixedTypesSequential_WhenCheckingMultipleTypes_ThenReturnsTrue(t *testing.T) {
	// Given
	input := "()[]{}[](){}"

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.True(t, result)
}

func Test_GivenNestedDifferentTypes_WhenCheckingMultipleTypes_ThenReturnsTrue(t *testing.T) {
	// Given
	input := "({[]})"

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.True(t, result)
}

func Test_GivenDeeplyNestedMixedTypes_WhenCheckingMultipleTypes_ThenReturnsTrue(t *testing.T) {
	// Given
	input := "{[()]}"

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.True(t, result)
}

func Test_GivenComplexNestedPattern_WhenCheckingMultipleTypes_ThenReturnsTrue(t *testing.T) {
	// Given
	input := "[({})]"

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.True(t, result)
}

func Test_GivenMixedNestedAndSequential_WhenCheckingMultipleTypes_ThenReturnsTrue(t *testing.T) {
	// Given
	input := "[()]{}{[()()]}"

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.True(t, result)
}

func Test_GivenComplexValidExpression_WhenCheckingMultipleTypes_ThenReturnsTrue(t *testing.T) {
	// Given
	input := "(()){[()]}[{}]"

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.True(t, result)
}

// MISMATCHED TYPES - Key difference from task 4

func Test_GivenRoundOpenSquareClose_WhenCheckingMultipleTypes_ThenReturnsFalse(t *testing.T) {
	// Given
	input := "(]"

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.False(t, result)
}

func Test_GivenCurlyOpenRoundClose_WhenCheckingMultipleTypes_ThenReturnsFalse(t *testing.T) {
	// Given
	input := "{)"

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.False(t, result)
}

func Test_GivenSquareOpenCurlyClose_WhenCheckingMultipleTypes_ThenReturnsFalse(t *testing.T) {
	// Given
	input := "[}"

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.False(t, result)
}

func Test_GivenWrongOrderOfClosing_WhenCheckingMultipleTypes_ThenReturnsFalse(t *testing.T) {
	// Given
	input := "([)]"

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.False(t, result)
}

func Test_GivenMismatchedInNested_WhenCheckingMultipleTypes_ThenReturnsFalse(t *testing.T) {
	// Given
	input := "({[}])"

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.False(t, result)
}

func Test_GivenMismatchedInMiddle_WhenCheckingMultipleTypes_ThenReturnsFalse(t *testing.T) {
	// Given
	input := "[(])"

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.False(t, result)
}

func Test_GivenComplexMismatch_WhenCheckingMultipleTypes_ThenReturnsFalse(t *testing.T) {
	// Given
	input := "[({})]("

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.False(t, result)
}

func Test_GivenMismatchAtEnd_WhenCheckingMultipleTypes_ThenReturnsFalse(t *testing.T) {
	// Given
	input := "{[(])}"

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.False(t, result)
}

// UNBALANCED COUNTS

func Test_GivenMoreOpeningThanClosing_WhenCheckingMultipleTypes_ThenReturnsFalse(t *testing.T) {
	// Given
	input := "({["

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.False(t, result)
}

func Test_GivenMoreClosingThanOpening_WhenCheckingMultipleTypes_ThenReturnsFalse(t *testing.T) {
	// Given
	input := "]})"

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.False(t, result)
}

func Test_GivenOnlyClosing_WhenCheckingMultipleTypes_ThenReturnsFalse(t *testing.T) {
	// Given
	input := "}}])"

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.False(t, result)
}

func Test_GivenOnlyOpening_WhenCheckingMultipleTypes_ThenReturnsFalse(t *testing.T) {
	// Given
	input := "{{[("

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.False(t, result)
}

func Test_GivenIncompleteNested_WhenCheckingMultipleTypes_ThenReturnsFalse(t *testing.T) {
	// Given
	input := "[{()]"

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.False(t, result)
}

func Test_GivenExtraClosingAtEnd_WhenCheckingMultipleTypes_ThenReturnsFalse(t *testing.T) {
	// Given
	input := "()[]{})"

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.False(t, result)
}

func Test_GivenVeryComplexBalanced_WhenCheckingMultipleTypes_ThenReturnsTrue(t *testing.T) {
	// Given
	input := "{[()()]}[{()}](())"

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.True(t, result)
}

func Test_GivenDeeplyNestedAllTypes_WhenCheckingMultipleTypes_ThenReturnsTrue(t *testing.T) {
	// Given
	input := "{[({[()]})]}"

	// When
	result := IsBalancedMultiple(input)

	// Then
	assert.True(t, result)
}

func Test_GivenEmptyMinStack_WhenGettingMin_ThenReturnsError(t *testing.T) {
	// Given
	stack := MinStack[int]{}

	// When
	_, err := stack.GetMin()

	// Then
	assert.Error(t, err)
}

func Test_GivenMinStack_WhenPushingDuplicateMinimums_ThenHandlesCorrectly(t *testing.T) {
	// Given
	stack := MinStack[int]{}
	stack.Push(5)
	stack.Push(2)
	stack.Push(2)
	stack.Push(2)
	stack.Push(8)

	// When/Then
	min1, _ := stack.GetMin()
	assert.Equal(t, 2, min1)

	stack.Pop() // remove 8
	min2, _ := stack.GetMin()
	assert.Equal(t, 2, min2)

	stack.Pop() // remove first 2
	min3, _ := stack.GetMin()
	assert.Equal(t, 2, min3)

	stack.Pop() // remove second 2
	min4, _ := stack.GetMin()
	assert.Equal(t, 2, min4)

	stack.Pop() // remove last 2
	min5, _ := stack.GetMin()
	assert.Equal(t, 5, min5)
}

func Test_GivenMinStack_WhenAlternatingHighLowValues_ThenMinTracksCorrectly(t *testing.T) {
	// Given
	stack := MinStack[int]{}

	// When
	stack.Push(10)
	stack.Push(1)
	stack.Push(20)
	stack.Push(2)
	stack.Push(30)
	stack.Push(3)

	// Then
	min1, _ := stack.GetMin()
	assert.Equal(t, 1, min1)

	stack.Pop() // 3
	stack.Pop() // 30
	min2, _ := stack.GetMin()
	assert.Equal(t, 1, min2)

	stack.Pop() // 2
	min3, _ := stack.GetMin()
	assert.Equal(t, 1, min3)

	stack.Pop() // 20
	min4, _ := stack.GetMin()
	assert.Equal(t, 1, min4)

	stack.Pop() // 1
	min5, _ := stack.GetMin()
	assert.Equal(t, 10, min5)
}

func Test_GivenMinStack_WhenRandomSequence_ThenMinAlwaysCorrect(t *testing.T) {
	// Given
	stack := MinStack[int]{}

	// When/Then - simulating random operations
	stack.Push(15)
	assert.Equal(t, 15, mustGetMin(&stack))

	stack.Push(8)
	assert.Equal(t, 8, mustGetMin(&stack))

	stack.Push(23)
	assert.Equal(t, 8, mustGetMin(&stack))

	stack.Push(3)
	assert.Equal(t, 3, mustGetMin(&stack))

	stack.Push(42)
	assert.Equal(t, 3, mustGetMin(&stack))

	stack.Push(1)
	assert.Equal(t, 1, mustGetMin(&stack))

	stack.Push(17)
	assert.Equal(t, 1, mustGetMin(&stack))

	stack.Pop() // 17
	assert.Equal(t, 1, mustGetMin(&stack))

	stack.Pop() // 1
	assert.Equal(t, 3, mustGetMin(&stack))

	stack.Pop() // 42
	assert.Equal(t, 3, mustGetMin(&stack))

	stack.Pop() // 3
	assert.Equal(t, 8, mustGetMin(&stack))

	stack.Push(5)
	assert.Equal(t, 5, mustGetMin(&stack))

	stack.Pop() // 5
	stack.Pop() // 23
	assert.Equal(t, 8, mustGetMin(&stack))
}

func Test_GivenMinStack_WhenDescendingThenAscending_ThenMinFollowsPattern(t *testing.T) {
	// Given
	stack := MinStack[int]{}

	// When - push descending
	stack.Push(100)
	stack.Push(90)
	stack.Push(80)
	stack.Push(70)
	stack.Push(60)

	// Then
	assert.Equal(t, 60, mustGetMin(&stack))

	// When - push ascending
	stack.Push(70)
	stack.Push(80)
	stack.Push(90)
	stack.Push(100)

	// Then - min should still be 60
	assert.Equal(t, 60, mustGetMin(&stack))
	assert.Equal(t, 9, stack.Size())

	// When - pop until we hit the minimum
	for i := 0; i < 4; i++ {
		stack.Pop()
	}

	// Then - still 60
	assert.Equal(t, 60, mustGetMin(&stack))

	// When - pop the minimum
	stack.Pop()

	// Then - next minimum is 70
	assert.Equal(t, 70, mustGetMin(&stack))
}

func Test_GivenMinStack_WhenNegativeNumbers_ThenHandlesCorrectly(t *testing.T) {
	// Given
	stack := MinStack[int]{}

	// When
	stack.Push(5)
	stack.Push(-3)
	stack.Push(10)
	stack.Push(-7)
	stack.Push(2)
	stack.Push(-1)

	// Then
	assert.Equal(t, -7, mustGetMin(&stack))

	stack.Pop() // -1
	assert.Equal(t, -7, mustGetMin(&stack))

	stack.Pop() // 2
	assert.Equal(t, -7, mustGetMin(&stack))

	stack.Pop() // -7
	assert.Equal(t, -3, mustGetMin(&stack))

	stack.Pop() // 10
	assert.Equal(t, -3, mustGetMin(&stack))

	stack.Pop() // -3
	assert.Equal(t, 5, mustGetMin(&stack))
}

func Test_GivenMinStack_WhenPushingAllSameValues_ThenMinStaysConstant(t *testing.T) {
	// Given
	stack := MinStack[int]{}

	// When
	for i := 0; i < 10; i++ {
		stack.Push(42)
		// Then
		assert.Equal(t, 42, mustGetMin(&stack))
	}

	// When popping
	for i := 0; i < 9; i++ {
		stack.Pop()
		// Then
		assert.Equal(t, 42, mustGetMin(&stack))
	}
}

func Test_GivenMinStack_WhenEmptyAfterOperations_ThenGetMinReturnsError(t *testing.T) {
	// Given
	stack := MinStack[int]{}
	stack.Push(1)
	stack.Push(2)
	stack.Push(3)

	// When
	stack.Pop()
	stack.Pop()
	stack.Pop()

	// Then
	_, err := stack.GetMin()
	assert.Error(t, err)
	assert.Equal(t, 0, stack.Size())
}

// helper
func mustGetMin(stack *MinStack[int]) int {
	curMin, err := stack.GetMin()
	if err != nil {
		panic(err)
	}
	return curMin
}