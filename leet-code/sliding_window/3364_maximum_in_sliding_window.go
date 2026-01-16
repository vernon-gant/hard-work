package sliding_window

import "math"

type MaxHeap struct {
	count, size int
	nums        []int
}

func New(size int) *MaxHeap {
	maxHeap := &MaxHeap{}
	maxHeap.count = 0
	maxHeap.size = size
	maxHeap.nums = make([]int, size)
	return maxHeap
}

func (maxHeap *MaxHeap) Push(newElement int) {
	if maxHeap.count == maxHeap.size {
		return
	}

	newElementIdx := maxHeap.count
	maxHeap.nums[newElementIdx] = newElement

	for parentIdx := (newElementIdx - 1) / 2; newElement > maxHeap.nums[parentIdx]; parentIdx = (newElementIdx - 1) / 2 {
		maxHeap.nums[newElementIdx] = maxHeap.nums[parentIdx]
		maxHeap.nums[parentIdx] = newElement
		newElementIdx = parentIdx
	}
	maxHeap.count++
}

func (maxHeap *MaxHeap) GetMax() int {
	return maxHeap.nums[0]
}

func (maxHeap *MaxHeap) getGretestChildIdx(parentIdx int) int {
	leftChildIdx, rightChildIdx := parentIdx+1, parentIdx+2

	if leftChildIdx >= maxHeap.size {
		return -1
	}

	if rightChildIdx >= maxHeap.size || maxHeap.nums[leftChildIdx] > maxHeap.nums[rightChildIdx] {
		return leftChildIdx
	}
	
	return rightChildIdx
}

func (maxHeap *MaxHeap) Pop() {
	if maxHeap.count == 0 {
		return
	}
	
	maxHeap.nums[0] = math.MinInt
	
	if maxHeap.count == 1 {
		maxHeap.count--
		return
	}
	
	tempRootIdx := 0
	maxHeap.nums[tempRootIdx] = maxHeap.nums[maxHeap.count - 1]
	maxHeap.nums[maxHeap.count - 1] = math.MinInt
}
