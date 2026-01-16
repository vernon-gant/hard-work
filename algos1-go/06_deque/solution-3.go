package deque

import (
	"testing"
	"github.com/stretchr/testify/assert"
)

// helpers

func makeDeque(values ...int) Deque[int] {
	deque := Deque[int]{}
	for _, v := range values {
		deque.AddTail(v)
	}
	return deque
}

func toSlice(deque *Deque[int]) []int {
	var result []int
	for deque.Size() > 0 {
		val, _ := deque.RemoveFront()
		result = append(result, val)
	}
	return result
}

// SIZE

func Test_GivenEmptyDeque_WhenGettingSize_ThenReturnsZero(t *testing.T) {
	// Given
	deque := Deque[int]{}

	// When
	size := deque.Size()

	// Then
	assert.Equal(t, 0, size)
}

func Test_GivenDeque_WhenAddingElements_ThenSizeIncreasesCorrectly(t *testing.T) {
	// Given
	deque := Deque[int]{}

	// When/Then
	deque.AddFront(1)
	assert.Equal(t, 1, deque.Size())

	deque.AddTail(2)
	assert.Equal(t, 2, deque.Size())

	deque.AddFront(3)
	assert.Equal(t, 3, deque.Size())
}

// ADD FRONT

func Test_GivenEmptyDeque_WhenAddingFront_ThenBecomesHeadAndTail(t *testing.T) {
	// Given
	deque := Deque[int]{}

	// When
	deque.AddFront(42)

	// Then
	assert.Equal(t, 1, deque.Size())
	val, _ := deque.RemoveFront()
	assert.Equal(t, 42, val)
}

func Test_GivenNonEmptyDeque_WhenAddingFront_ThenBecomesNewHead(t *testing.T) {
	// Given
	deque := makeDeque(2, 3, 4)

	// When
	deque.AddFront(1)

	// Then
	assert.Equal(t, []int{1, 2, 3, 4}, toSlice(&deque))
}

// ADD TAIL

func Test_GivenEmptyDeque_WhenAddingTail_ThenBecomesHeadAndTail(t *testing.T) {
	// Given
	deque := Deque[int]{}

	// When
	deque.AddTail(42)

	// Then
	assert.Equal(t, 1, deque.Size())
	val, _ := deque.RemoveTail()
	assert.Equal(t, 42, val)
}

func Test_GivenNonEmptyDeque_WhenAddingTail_ThenBecomesNewTail(t *testing.T) {
	// Given
	deque := makeDeque(1, 2, 3)

	// When
	deque.AddTail(4)

	// Then
	assert.Equal(t, []int{1, 2, 3, 4}, toSlice(&deque))
}

// REMOVE FRONT

func Test_GivenEmptyDeque_WhenRemovingFront_ThenReturnsError(t *testing.T) {
	// Given
	deque := Deque[int]{}

	// When
	_, err := deque.RemoveFront()

	// Then
	assert.Error(t, err)
}

func Test_GivenSingleElementDeque_WhenRemovingFront_ThenBecomesEmpty(t *testing.T) {
	// Given
	deque := makeDeque(42)

	// When
	val, err := deque.RemoveFront()

	// Then
	assert.NoError(t, err)
	assert.Equal(t, 42, val)
	assert.Equal(t, 0, deque.Size())
}

// REMOVE TAIL

func Test_GivenEmptyDeque_WhenRemovingTail_ThenReturnsError(t *testing.T) {
	// Given
	deque := Deque[int]{}

	// When
	_, err := deque.RemoveTail()

	// Then
	assert.Error(t, err)
}

func Test_GivenSingleElementDeque_WhenRemovingTail_ThenBecomesEmpty(t *testing.T) {
	// Given
	deque := makeDeque(42)

	// When
	val, err := deque.RemoveTail()

	// Then
	assert.NoError(t, err)
	assert.Equal(t, 42, val)
	assert.Equal(t, 0, deque.Size())
}

// COMBINED BEHAVIORS - DEQUE AS STACK (LIFO)

func Test_GivenDeque_WhenUsedAsStackFromFront_ThenBehavesLIFO(t *testing.T) {
	// Given
	deque := Deque[int]{}

	// When - push and pop from front only
	deque.AddFront(1)
	deque.AddFront(2)
	deque.AddFront(3)

	val1, _ := deque.RemoveFront()
	val2, _ := deque.RemoveFront()
	val3, _ := deque.RemoveFront()

	// Then - LIFO order
	assert.Equal(t, 3, val1)
	assert.Equal(t, 2, val2)
	assert.Equal(t, 1, val3)
}

func Test_GivenDeque_WhenUsedAsStackFromTail_ThenBehavesLIFO(t *testing.T) {
	// Given
	deque := Deque[int]{}

	// When - push and pop from tail only
	deque.AddTail(1)
	deque.AddTail(2)
	deque.AddTail(3)

	val1, _ := deque.RemoveTail()
	val2, _ := deque.RemoveTail()
	val3, _ := deque.RemoveTail()

	// Then - LIFO order
	assert.Equal(t, 3, val1)
	assert.Equal(t, 2, val2)
	assert.Equal(t, 1, val3)
}

// COMBINED BEHAVIORS - DEQUE AS QUEUE (FIFO)

func Test_GivenDeque_WhenUsedAsQueueFrontToTail_ThenBehavesFIFO(t *testing.T) {
	// Given
	deque := Deque[int]{}

	// When - enqueue at front, dequeue from tail
	deque.AddFront(1)
	deque.AddFront(2)
	deque.AddFront(3)

	val1, _ := deque.RemoveTail()
	val2, _ := deque.RemoveTail()
	val3, _ := deque.RemoveTail()

	// Then - FIFO order
	assert.Equal(t, 1, val1)
	assert.Equal(t, 2, val2)
	assert.Equal(t, 3, val3)
}

func Test_GivenDeque_WhenUsedAsQueueTailToFront_ThenBehavesFIFO(t *testing.T) {
	// Given
	deque := Deque[int]{}

	// When - enqueue at tail, dequeue from front
	deque.AddTail(1)
	deque.AddTail(2)
	deque.AddTail(3)

	val1, _ := deque.RemoveFront()
	val2, _ := deque.RemoveFront()
	val3, _ := deque.RemoveFront()

	// Then - FIFO order
	assert.Equal(t, 1, val1)
	assert.Equal(t, 2, val2)
	assert.Equal(t, 3, val3)
}

// COMBINED BEHAVIORS - MIXED OPERATIONS

func Test_GivenDeque_WhenMixingFrontAndTailOperations_ThenMaintainsCorrectOrder(t *testing.T) {
	// Given
	deque := Deque[int]{}

	// When - complex interleaved operations
	deque.AddFront(3)    // [3]
	deque.AddFront(2)    // [2, 3]
	deque.AddTail(4)     // [2, 3, 4]
	deque.AddFront(1)    // [1, 2, 3, 4]
	deque.AddTail(5)     // [1, 2, 3, 4, 5]

	val1, _ := deque.RemoveFront() // remove 1
	val2, _ := deque.RemoveTail()  // remove 5
	val3, _ := deque.RemoveFront() // remove 2

	// Then
	assert.Equal(t, 1, val1)
	assert.Equal(t, 5, val2)
	assert.Equal(t, 2, val3)
	assert.Equal(t, []int{3, 4}, toSlice(&deque))
}

func Test_GivenDeque_WhenAlternatingBetweenEnds_ThenBehavesCorrectly(t *testing.T) {
	// Given
	deque := Deque[int]{}

	// When - build from both ends alternately
	deque.AddTail(5)     // [5]
	deque.AddFront(4)    // [4, 5]
	deque.AddTail(6)     // [4, 5, 6]
	deque.AddFront(3)    // [3, 4, 5, 6]
	deque.AddTail(7)     // [3, 4, 5, 6, 7]
	deque.AddFront(2)    // [2, 3, 4, 5, 6, 7]
	deque.AddFront(1)    // [1, 2, 3, 4, 5, 6, 7]

	// Then - remove all from front to verify order
	assert.Equal(t, []int{1, 2, 3, 4, 5, 6, 7}, toSlice(&deque))
}

func Test_GivenDeque_WhenRemovingFromOppositeEnds_ThenConvergesCorrectly(t *testing.T) {
	// Given
	deque := makeDeque(1, 2, 3, 4, 5, 6, 7)

	// When - remove alternately from both ends
	front1, _ := deque.RemoveFront()
	tail1, _ := deque.RemoveTail()
	front2, _ := deque.RemoveFront()
	tail2, _ := deque.RemoveTail()

	// Then
	assert.Equal(t, 1, front1)
	assert.Equal(t, 7, tail1)
	assert.Equal(t, 2, front2)
	assert.Equal(t, 6, tail2)
	assert.Equal(t, []int{3, 4, 5}, toSlice(&deque))
}

func Test_GivenDeque_WhenComplexSequenceOfOperations_ThenMaintainsIntegrity(t *testing.T) {
	// Given
	deque := Deque[int]{}

	// When - simulate real-world usage pattern
	deque.AddTail(10)
	deque.AddTail(20)
	deque.AddFront(5)

	val1, _ := deque.RemoveTail()

	deque.AddTail(30)
	deque.AddFront(1)

	val2, _ := deque.RemoveFront()
	val3, _ := deque.RemoveTail()

	deque.AddTail(40)
	deque.AddFront(2)

	// Then - verify final state
	assert.Equal(t, 20, val1)
	assert.Equal(t, 1, val2)
	assert.Equal(t, 30, val3)
	assert.Equal(t, 4, deque.Size())
	assert.Equal(t, []int{2, 5, 10, 40}, toSlice(&deque))
}