package two_heaps

import (
	"container/heap"
	"math"
)

type MaxHeapSliding []int

func (maxHeap * MaxHeapSliding) Len() int {
	return len(*maxHeap)
}

func (maxHeap * MaxHeapSliding) Swap(i, j int) {
	(*maxHeap)[i],(*maxHeap)[j] = (*maxHeap)[j], (*maxHeap)[i]
}

func (maxHeap * MaxHeapSliding) Less(i, j int) bool {
	return (*maxHeap)[i] > (*maxHeap)[j]
}

func (maxHeap * MaxHeapSliding) Pop() interface{} {
	last := (*maxHeap)[maxHeap.Len() - 1]
	*maxHeap = (*maxHeap)[0 : maxHeap.Len() - 1]
	return last
}

func (maxHeap * MaxHeapSliding) Peek() int {
	return (*maxHeap)[0]
}

func (maxHeap * MaxHeapSliding) Push(x interface{}) {
	*maxHeap = append(*maxHeap, x.(int))
}

type MinHeapSliding []int

func (minHeap * MinHeapSliding) Len() int {
	return len(*minHeap)
}

func (minHeap * MinHeapSliding) Swap(i, j int) {
	(*minHeap)[i],(*minHeap)[j] = (*minHeap)[j], (*minHeap)[i]
}

func (minHeap * MinHeapSliding) Less(i, j int) bool {
	return (*minHeap)[i] < (*minHeap)[j]
}

func (minHeap * MinHeapSliding) Pop() interface{} {
	last := (*minHeap)[minHeap.Len() - 1]
	*minHeap = (*minHeap)[0 : minHeap.Len() - 1]
	return last
}

func (minHeap * MinHeapSliding) Peek() int {
	return (*minHeap)[0]
}

func (minHeap * MinHeapSliding) Push(x interface{}) {
	*minHeap = append(*minHeap, x.(int))
}

func MedianSlidingWindow(nums []int, k int) []float64 {
	if len(nums) == 1 {
		return []float64{float64(nums[0])}
	}
	maxHeap, minHeap := &MaxHeapSliding{}, &MinHeapSliding{}
	for i := 0; i < k; i++ {
		heap.Push(maxHeap, nums[i])
	}
	pushToMin(maxHeap,minHeap,k)
	medians := []float64{computeMeadian(maxHeap,minHeap)}
	for i := k; i < len(nums); i++ {
		addNew(maxHeap,minHeap,nums[i])
		toRemove := nums[i - k]
		if toRemove > maxHeap.Peek() {
			removeOldFromMin(maxHeap,minHeap,toRemove,k)
		} else {
			removeOldFromMax(maxHeap,minHeap,toRemove,k)
		}
		medians = append(medians, computeMeadian(maxHeap,minHeap))
	}
	return medians
}

func computeMeadian(maxHeap * MaxHeapSliding, minHeap * MinHeapSliding) float64 {
	if (maxHeap.Len() + minHeap.Len()) % 2 == 0 {
		return float64(maxHeap.Peek() + minHeap.Peek()) / 2
	}
	return float64(maxHeap.Peek())
}

func addNew(maxHeap * MaxHeapSliding, minHeap * MinHeapSliding, newElement int) {
	if newElement > maxHeap.Peek() {
		heap.Push(minHeap, newElement)
		return
	}
	heap.Push(maxHeap, newElement)
}	

func pushToMin(maxHeap * MaxHeapSliding, minHeap * MinHeapSliding, k int) {
	for i := 0; i < k / 2; i++ {
		heap.Push(minHeap, heap.Pop(maxHeap))
	}	
}

func removeOldFromMin(maxHeap * MaxHeapSliding, minHeap * MinHeapSliding, toRemove, k int) {
	removed := 0
	for minHeap.Len() > 0 {
		top := heap.Pop(minHeap)
		if top == toRemove && removed == 0 {
			removed++
			continue
		}
		heap.Push(maxHeap,top)
	}
	pushToMin(maxHeap,minHeap,k)
}

func removeOldFromMax(maxHeap * MaxHeapSliding, minHeap * MinHeapSliding, toRemove, k int) {
	removed := 0
	for maxHeap.Len() > 0 {
		top := heap.Pop(maxHeap)
		if top == toRemove && removed == 0 {
			removed++
			continue
		}
		heap.Push(minHeap, top)
	}
	for i := 0; i < int(math.Ceil(float64(k) / 2)); i++ {
		heap.Push(maxHeap, heap.Pop(minHeap))
	}	
}