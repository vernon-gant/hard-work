package queue

import (
	"testing"
	"github.com/stretchr/testify/assert"
)

// helpers

func makeQueue(values ...int) Queue[int] {
	queue := Queue[int]{}
	for _, v := range values {
		queue.Enqueue(v)
	}
	return queue
}

func dequeueAll(queue *Queue[int]) []int {
	var result []int
	for queue.Size() > 0 {
		val, _ := queue.Dequeue()
		result = append(result, val)
	}
	return result
}

// SIZE

func Test_GivenEmptyQueue_WhenGettingSize_ThenReturnsZero(t *testing.T) {
	// Given
	queue := Queue[int]{}

	// When
	size := queue.Size()

	// Then
	assert.Equal(t, 0, size)
}

func Test_GivenSingleElementQueue_WhenGettingSize_ThenReturnsOne(t *testing.T) {
	// Given
	queue := makeQueue(42)

	// When
	size := queue.Size()

	// Then
	assert.Equal(t, 1, size)
}

func Test_GivenMultipleElementsQueue_WhenGettingSize_ThenReturnsCorrectCount(t *testing.T) {
	// Given
	queue := makeQueue(1, 2, 3, 4, 5)

	// When
	size := queue.Size()

	// Then
	assert.Equal(t, 5, size)
}

func Test_GivenQueue_WhenEnqueueing_ThenSizeIncreasesCorrectly(t *testing.T) {
	// Given
	queue := Queue[int]{}

	// When/Then
	assert.Equal(t, 0, queue.Size())

	queue.Enqueue(1)
	assert.Equal(t, 1, queue.Size())

	queue.Enqueue(2)
	assert.Equal(t, 2, queue.Size())

	queue.Enqueue(3)
	assert.Equal(t, 3, queue.Size())
}

// ENQUEUE

func Test_GivenEmptyQueue_WhenEnqueuing_ThenSizeBecomesOne(t *testing.T) {
	// Given
	queue := Queue[int]{}

	// When
	queue.Enqueue(10)

	// Then
	assert.Equal(t, 1, queue.Size())
}

func Test_GivenQueue_WhenEnqueueingMultipleValues_ThenAllAreStored(t *testing.T) {
	// Given
	queue := Queue[int]{}

	// When
	queue.Enqueue(1)
	queue.Enqueue(2)
	queue.Enqueue(3)

	// Then
	assert.Equal(t, 3, queue.Size())
}

func Test_GivenNonEmptyQueue_WhenEnqueuing_ThenSizeIncreases(t *testing.T) {
	// Given
	queue := makeQueue(1, 2, 3)
	initialSize := queue.Size()

	// When
	queue.Enqueue(4)

	// Then
	assert.Equal(t, initialSize+1, queue.Size())
}

// DEQUEUE

func Test_GivenEmptyQueue_WhenDequeuing_ThenReturnsError(t *testing.T) {
	// Given
	queue := Queue[int]{}

	// When
	_, err := queue.Dequeue()

	// Then
	assert.Error(t, err)
	assert.Equal(t, 0, queue.Size())
}

func Test_GivenSingleElementQueue_WhenDequeuing_ThenReturnsElementAndBecomesEmpty(t *testing.T) {
	// Given
	queue := makeQueue(42)

	// When
	val, err := queue.Dequeue()

	// Then
	assert.NoError(t, err)
	assert.Equal(t, 42, val)
	assert.Equal(t, 0, queue.Size())
}

func Test_GivenMultipleElementsQueue_WhenDequeuing_ThenReturnsFirstEnqueued(t *testing.T) {
	// Given
	queue := makeQueue(1, 2, 3)

	// When
	val, err := queue.Dequeue()

	// Then
	assert.NoError(t, err)
	assert.Equal(t, 1, val)
	assert.Equal(t, 2, queue.Size())
}

func Test_GivenQueue_WhenDequeuingAll_ThenReturnsFIFOOrder(t *testing.T) {
	// Given
	queue := makeQueue(1, 2, 3, 4, 5)

	// When
	values := dequeueAll(&queue)

	// Then
	assert.Equal(t, []int{1, 2, 3, 4, 5}, values)
	assert.Equal(t, 0, queue.Size())
}

func Test_GivenQueue_WhenDequeuingAfterEmpty_ThenReturnsError(t *testing.T) {
	// Given
	queue := makeQueue(1)
	queue.Dequeue()

	// When
	_, err := queue.Dequeue()

	// Then
	assert.Error(t, err)
}

func Test_GivenQueue_WhenDequeuingMultipleTimes_ThenSizeDecreasesCorrectly(t *testing.T) {
	// Given
	queue := makeQueue(1, 2, 3, 4)

	// When/Then
	assert.Equal(t, 4, queue.Size())

	queue.Dequeue()
	assert.Equal(t, 3, queue.Size())

	queue.Dequeue()
	assert.Equal(t, 2, queue.Size())
}

// FIFO BEHAVIOR

func Test_GivenQueue_WhenEnqueueingThenDequeuing_ThenMaintainsFIFOOrder(t *testing.T) {
	// Given
	queue := Queue[int]{}

	// When
	queue.Enqueue(10)
	queue.Enqueue(20)
	queue.Enqueue(30)

	val1, _ := queue.Dequeue()
	val2, _ := queue.Dequeue()
	val3, _ := queue.Dequeue()

	// Then
	assert.Equal(t, 10, val1)
	assert.Equal(t, 20, val2)
	assert.Equal(t, 30, val3)
}

func Test_GivenQueue_WhenInterleavingEnqueueDequeue_ThenMaintainsFIFOOrder(t *testing.T) {
	// Given
	queue := Queue[int]{}

	// When
	queue.Enqueue(1)
	queue.Enqueue(2)
	val1, _ := queue.Dequeue()
	queue.Enqueue(3)
	val2, _ := queue.Dequeue()
	val3, _ := queue.Dequeue()

	// Then
	assert.Equal(t, 1, val1)
	assert.Equal(t, 2, val2)
	assert.Equal(t, 3, val3)
	assert.Equal(t, 0, queue.Size())
}

func Test_GivenQueue_WhenEnqueueingAfterDequeuingAll_ThenWorksCorrectly(t *testing.T) {
	// Given
	queue := makeQueue(1, 2, 3)
	dequeueAll(&queue)

	// When
	queue.Enqueue(99)

	// Then
	assert.Equal(t, 1, queue.Size())
	val, err := queue.Dequeue()
	assert.NoError(t, err)
	assert.Equal(t, 99, val)
}

func Test_GivenQueue_WhenRepeatedEnqueueDequeue_ThenMaintainsCorrectness(t *testing.T) {
	// Given
	queue := Queue[int]{}

	// When/Then
	for i := 1; i <= 10; i++ {
		queue.Enqueue(i)
		assert.Equal(t, 1, queue.Size())
		val, _ := queue.Dequeue()
		assert.Equal(t, i, val)
		assert.Equal(t, 0, queue.Size())
	}
}

func Test_GivenQueue_WhenComplexInterleavedOperations_ThenBehavesCorrectly(t *testing.T) {
	// Given
	queue := Queue[int]{}

	// When
	queue.Enqueue(1)
	queue.Enqueue(2)
	queue.Enqueue(3)

	val1, _ := queue.Dequeue()

	queue.Enqueue(4)
	queue.Enqueue(5)

	val2, _ := queue.Dequeue()
	val3, _ := queue.Dequeue()

	queue.Enqueue(6)

	remaining := dequeueAll(&queue)

	// Then
	assert.Equal(t, 1, val1)
	assert.Equal(t, 2, val2)
	assert.Equal(t, 3, val3)
	assert.Equal(t, []int{4, 5, 6}, remaining)
}

// ROTATE

func Test_GivenEmptyQueue_WhenRotating_ThenRemainsEmpty(t *testing.T) {
	// Given
	queue := Queue[int]{}

	// When
	err := queue.Rotate(3)

	// Then
	assert.NoError(t, err)
	assert.Equal(t, 0, queue.Size())
}

func Test_GivenQueue_WhenRotatingByZero_ThenReturnsError(t *testing.T) {
	// Given
	queue := makeQueue(1, 2, 3)

	// When
	err := queue.Rotate(0)

	// Then
	assert.Error(t, err)
}

func Test_GivenQueue_WhenRotatingByNegative_ThenReturnsError(t *testing.T) {
	// Given
	queue := makeQueue(1, 2, 3)

	// When
	err := queue.Rotate(-5)

	// Then
	assert.Error(t, err)
}

func Test_GivenSingleElementQueue_WhenRotating_ThenUnchanged(t *testing.T) {
	// Given
	queue := makeQueue(42)

	// When
	err := queue.Rotate(5)

	// Then
	assert.NoError(t, err)
	assert.Equal(t, 1, queue.Size())
	val, _ := queue.Dequeue()
	assert.Equal(t, 42, val)
}

func Test_GivenQueue_WhenRotatingByOne_ThenFrontMovesToBack(t *testing.T) {
	// Given
	queue := makeQueue(1, 2, 3, 4, 5)

	// When
	err := queue.Rotate(1)

	// Then
	assert.NoError(t, err)
	assert.Equal(t, []int{2, 3, 4, 5, 1}, dequeueAll(&queue))
}

func Test_GivenQueue_WhenRotatingByTwo_ThenFirstTwoMoveToBack(t *testing.T) {
	// Given
	queue := makeQueue(1, 2, 3, 4, 5)

	// When
	err := queue.Rotate(2)

	// Then
	assert.NoError(t, err)
	assert.Equal(t, []int{3, 4, 5, 1, 2}, dequeueAll(&queue))
}

func Test_GivenQueue_WhenRotatingByThree_ThenFirstThreeMoveToBack(t *testing.T) {
	// Given
	queue := makeQueue(1, 2, 3, 4, 5)

	// When
	err := queue.Rotate(3)

	// Then
	assert.NoError(t, err)
	assert.Equal(t, []int{4, 5, 1, 2, 3}, dequeueAll(&queue))
}

func Test_GivenQueue_WhenRotatingBySize_ThenReturnsToOriginal(t *testing.T) {
	// Given
	queue := makeQueue(1, 2, 3, 4)

	// When
	err := queue.Rotate(4)

	// Then
	assert.NoError(t, err)
	assert.Equal(t, []int{1, 2, 3, 4}, dequeueAll(&queue))
}

func Test_GivenQueue_WhenRotatingByMoreThanSize_ThenWrapsAround(t *testing.T) {
	// Given
	queue := makeQueue(1, 2, 3, 4)

	// When
	err := queue.Rotate(6) // equivalent to rotating by 2

	// Then
	assert.NoError(t, err)
	assert.Equal(t, []int{3, 4, 1, 2}, dequeueAll(&queue))
}

func Test_GivenQueue_WhenRotatingByExactlyTwiceSize_ThenReturnsToOriginal(t *testing.T) {
	// Given
	queue := makeQueue(1, 2, 3, 4, 5)

	// When
	err := queue.Rotate(10)

	// Then
	assert.NoError(t, err)
	assert.Equal(t, []int{1, 2, 3, 4, 5}, dequeueAll(&queue))
}

func Test_GivenQueue_WhenRotatingMultipleTimes_ThenAccumulates(t *testing.T) {
	// Given
	queue := makeQueue(1, 2, 3, 4, 5)

	// When
	queue.Rotate(2)
	queue.Rotate(1)

	// Then
	assert.Equal(t, []int{4, 5, 1, 2, 3}, dequeueAll(&queue))
}

func Test_GivenQueue_WhenRotatingLargeNumber_ThenUsesModulo(t *testing.T) {
	// Given
	queue := makeQueue(1, 2, 3, 4, 5)

	// When
	err := queue.Rotate(13) // 13 % 5 = 3

	// Then
	assert.NoError(t, err)
	assert.Equal(t, []int{4, 5, 1, 2, 3}, dequeueAll(&queue))
}

// helpers

func makeQueue2(values ...int) Queue2[int] {
	queue := Queue2[int]{}
	for _, v := range values {
		queue.Enqueue(v)
	}
	return queue
}

func dequeueAll2(queue *Queue2[int]) []int {
	var result []int
	for queue.Size() > 0 {
		val, _ := queue.Dequeue()
		result = append(result, val)
	}
	return result
}

// SIZE

func Test_GivenEmptyQueue2_WhenGettingSize_ThenReturnsZero(t *testing.T) {
	// Given
	queue := Queue2[int]{}

	// When
	size := queue.Size()

	// Then
	assert.Equal(t, 0, size)
}

func Test_GivenSingleElementQueue2_WhenGettingSize_ThenReturnsOne(t *testing.T) {
	// Given
	queue := makeQueue2(42)

	// When
	size := queue.Size()

	// Then
	assert.Equal(t, 1, size)
}

func Test_GivenMultipleElementsQueue2_WhenGettingSize_ThenReturnsCorrectCount(t *testing.T) {
	// Given
	queue := makeQueue2(1, 2, 3, 4, 5)

	// When
	size := queue.Size()

	// Then
	assert.Equal(t, 5, size)
}

func Test_GivenQueue2_WhenEnqueueing_ThenSizeIncreasesCorrectly(t *testing.T) {
	// Given
	queue := Queue2[int]{}

	// When/Then
	assert.Equal(t, 0, queue.Size())

	queue.Enqueue(1)
	assert.Equal(t, 1, queue.Size())

	queue.Enqueue(2)
	assert.Equal(t, 2, queue.Size())

	queue.Enqueue(3)
	assert.Equal(t, 3, queue.Size())
}

// ENQUEUE

func Test_GivenEmptyQueue2_WhenEnqueuing_ThenSizeBecomesOne(t *testing.T) {
	// Given
	queue := Queue2[int]{}

	// When
	queue.Enqueue(10)

	// Then
	assert.Equal(t, 1, queue.Size())
}

func Test_GivenQueue2_WhenEnqueueingMultipleValues_ThenAllAreStored(t *testing.T) {
	// Given
	queue := Queue2[int]{}

	// When
	queue.Enqueue(1)
	queue.Enqueue(2)
	queue.Enqueue(3)

	// Then
	assert.Equal(t, 3, queue.Size())
}

func Test_GivenNonEmptyQueue2_WhenEnqueuing_ThenSizeIncreases(t *testing.T) {
	// Given
	queue := makeQueue2(1, 2, 3)
	initialSize := queue.Size()

	// When
	queue.Enqueue(4)

	// Then
	assert.Equal(t, initialSize+1, queue.Size())
}

// DEQUEUE

func Test_GivenEmptyQueue2_WhenDequeuing_ThenReturnsError(t *testing.T) {
	// Given
	queue := Queue2[int]{}

	// When
	_, err := queue.Dequeue()

	// Then
	assert.Error(t, err)
	assert.Equal(t, 0, queue.Size())
}

func Test_GivenSingleElementQueue2_WhenDequeuing_ThenReturnsElementAndBecomesEmpty(t *testing.T) {
	// Given
	queue := makeQueue2(42)

	// When
	val, err := queue.Dequeue()

	// Then
	assert.NoError(t, err)
	assert.Equal(t, 42, val)
	assert.Equal(t, 0, queue.Size())
}

func Test_GivenMultipleElementsQueue2_WhenDequeuing_ThenReturnsFirstEnqueued(t *testing.T) {
	// Given
	queue := makeQueue2(1, 2, 3)

	// When
	val, err := queue.Dequeue()

	// Then
	assert.NoError(t, err)
	assert.Equal(t, 1, val)
	assert.Equal(t, 2, queue.Size())
}

func Test_GivenQueue2_WhenDequeuingAll_ThenReturnsFIFOOrder(t *testing.T) {
	// Given
	queue := makeQueue2(1, 2, 3, 4, 5)

	// When
	values := dequeueAll2(&queue)

	// Then
	assert.Equal(t, []int{1, 2, 3, 4, 5}, values)
	assert.Equal(t, 0, queue.Size())
}

func Test_GivenQueue2_WhenDequeuingAfterEmpty_ThenReturnsError(t *testing.T) {
	// Given
	queue := makeQueue2(1)
	queue.Dequeue()

	// When
	_, err := queue.Dequeue()

	// Then
	assert.Error(t, err)
}

func Test_GivenQueue2_WhenDequeuingMultipleTimes_ThenSizeDecreasesCorrectly(t *testing.T) {
	// Given
	queue := makeQueue2(1, 2, 3, 4)

	// When/Then
	assert.Equal(t, 4, queue.Size())

	queue.Dequeue()
	assert.Equal(t, 3, queue.Size())

	queue.Dequeue()
	assert.Equal(t, 2, queue.Size())
}

// FIFO BEHAVIOR

func Test_GivenQueue2_WhenEnqueueingThenDequeuing_ThenMaintainsFIFOOrder(t *testing.T) {
	// Given
	queue := Queue2[int]{}

	// When
	queue.Enqueue(10)
	queue.Enqueue(20)
	queue.Enqueue(30)

	val1, _ := queue.Dequeue()
	val2, _ := queue.Dequeue()
	val3, _ := queue.Dequeue()

	// Then
	assert.Equal(t, 10, val1)
	assert.Equal(t, 20, val2)
	assert.Equal(t, 30, val3)
}

func Test_GivenQueue2_WhenInterleavingEnqueueDequeue_ThenMaintainsFIFOOrder(t *testing.T) {
	// Given
	queue := Queue2[int]{}

	// When
	queue.Enqueue(1)
	queue.Enqueue(2)
	val1, _ := queue.Dequeue()
	queue.Enqueue(3)
	val2, _ := queue.Dequeue()
	val3, _ := queue.Dequeue()

	// Then
	assert.Equal(t, 1, val1)
	assert.Equal(t, 2, val2)
	assert.Equal(t, 3, val3)
	assert.Equal(t, 0, queue.Size())
}

func Test_GivenQueue2_WhenEnqueueingAfterDequeuingAll_ThenWorksCorrectly(t *testing.T) {
	// Given
	queue := makeQueue2(1, 2, 3)
	dequeueAll2(&queue)

	// When
	queue.Enqueue(99)

	// Then
	assert.Equal(t, 1, queue.Size())
	val, err := queue.Dequeue()
	assert.NoError(t, err)
	assert.Equal(t, 99, val)
}

func Test_GivenQueue2_WhenComplexInterleavedOperations_ThenBehavesCorrectly(t *testing.T) {
	// Given
	queue := Queue2[int]{}

	// When
	queue.Enqueue(1)
	queue.Enqueue(2)
	queue.Enqueue(3)

	val1, _ := queue.Dequeue()

	queue.Enqueue(4)
	queue.Enqueue(5)

	val2, _ := queue.Dequeue()
	val3, _ := queue.Dequeue()

	queue.Enqueue(6)

	remaining := dequeueAll2(&queue)

	// Then
	assert.Equal(t, 1, val1)
	assert.Equal(t, 2, val2)
	assert.Equal(t, 3, val3)
	assert.Equal(t, []int{4, 5, 6}, remaining)
}

// REVERSE

func Test_GivenEmptyQueue_WhenReversing_ThenRemainsEmpty(t *testing.T) {
	// Given
	queue := Queue[int]{}

	// When
	queue.Reverse()

	// Then
	assert.Equal(t, 0, queue.Size())
}

func Test_GivenSingleElementQueue_WhenReversing_ThenUnchanged(t *testing.T) {
	// Given
	queue := makeQueue(42)

	// When
	queue.Reverse()

	// Then
	assert.Equal(t, 1, queue.Size())
	val, _ := queue.Dequeue()
	assert.Equal(t, 42, val)
}

func Test_GivenTwoElementQueue_WhenReversing_ThenOrderSwapped(t *testing.T) {
	// Given
	queue := makeQueue(1, 2)

	// When
	queue.Reverse()

	// Then
	assert.Equal(t, []int{2, 1}, dequeueAll(&queue))
}

func Test_GivenThreeElementQueue_WhenReversing_ThenOrderFullyReversed(t *testing.T) {
	// Given
	queue := makeQueue(1, 2, 3)

	// When
	queue.Reverse()

	// Then
	assert.Equal(t, []int{3, 2, 1}, dequeueAll(&queue))
}

func Test_GivenMultipleElementQueue_WhenReversing_ThenOrderFullyReversed(t *testing.T) {
	// Given
	queue := makeQueue(1, 2, 3, 4, 5)

	// When
	queue.Reverse()

	// Then
	assert.Equal(t, []int{5, 4, 3, 2, 1}, dequeueAll(&queue))
}

func Test_GivenQueue_WhenReversingTwice_ThenReturnsToOriginal(t *testing.T) {
	// Given
	queue := makeQueue(1, 2, 3, 4)

	// When
	queue.Reverse()
	queue.Reverse()

	// Then
	assert.Equal(t, []int{1, 2, 3, 4}, dequeueAll(&queue))
}

func Test_GivenReversedQueue_WhenEnqueueing_ThenAddsToBack(t *testing.T) {
	// Given
	queue := makeQueue(1, 2, 3)
	queue.Reverse()

	// When
	queue.Enqueue(99)

	// Then
	assert.Equal(t, []int{3, 2, 1, 99}, dequeueAll(&queue))
}

func Test_GivenReversedQueue_WhenDequeuing_ThenRemovesFromFront(t *testing.T) {
	// Given
	queue := makeQueue(1, 2, 3, 4, 5)
	queue.Reverse()

	// When
	val, _ := queue.Dequeue()

	// Then
	assert.Equal(t, 5, val)
	assert.Equal(t, []int{4, 3, 2, 1}, dequeueAll(&queue))
}