package top_k_elements

import (
	"container/heap"
)

type MaxHeapKClosest [][2]int

func (maxHeap * MaxHeapKClosest) Len() int  {
	return len(*maxHeap)
}

func (maxHeap* MaxHeapKClosest) Less(i, j int) bool  {
	return (*maxHeap)[i][0] > (*maxHeap)[j][0]
}

func (maxHeap* MaxHeapKClosest) Swap(i, j int)  {
	(*maxHeap)[i], (*maxHeap)[j] = (*maxHeap)[j], (*maxHeap)[i]
}

func (maxHeap* MaxHeapKClosest) Push(x interface{})  {
	*maxHeap = append(*maxHeap, x.([2]int))
}

func (maxHeap* MaxHeapKClosest) Pop() interface{}  {
	last := (*maxHeap)[maxHeap.Len() - 1]
	*maxHeap = (*maxHeap)[0 : maxHeap.Len() - 1]
	return last
}

func (maxHeap * MaxHeapKClosest) Peek() [2]int  {
	return (*maxHeap)[0]
}

func kClosest(points [][]int, k int) [][]int {
    maxHeap, arrayPointer := &MaxHeapKClosest{}, 0
	for ; arrayPointer < k; arrayPointer++ {
		heap.Push(maxHeap, [2]int{ computeDistance(points[arrayPointer]), arrayPointer })
	}
	for ; arrayPointer < len(points); arrayPointer++ {
		currentPointDistance := computeDistance(points[arrayPointer])
		if currentPointDistance > maxHeap.Peek()[0] {
			continue
		}
		heap.Pop(maxHeap)
		heap.Push(maxHeap, [2]int{ currentPointDistance, arrayPointer })
	}
	result := make([][]int, 0)
	for maxHeap.Len() > 0 {
		pointIdx := heap.Pop(maxHeap).([2]int)[1]
		result = append(result, []int{ points[pointIdx][0], points[pointIdx][1] })
	}
	return result
}

func computeDistance(point []int) int {
	return point[0] * point[0] + point[1] * point[1]
}