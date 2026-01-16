package k_way_merge

import "container/heap"

type MinHeapKSmallestPairs [][3]int

func NewMinHeap() *MinHeapKSmallestPairs {
    min := &MinHeapKSmallestPairs{}
    heap.Init(min)
    return min
}


func (minHeap * MinHeapKSmallestPairs) Len() int {
	return len(*minHeap)
}


func (minHeap * MinHeapKSmallestPairs) Empty() bool {
	return len(*minHeap) == 0
}


func (minHeap * MinHeapKSmallestPairs) Less(i, j int) bool {
	 return (*minHeap)[i][0] < (*minHeap)[j][0] 
}


func (minHeap * MinHeapKSmallestPairs) Swap(i, j int) {
	(*minHeap)[i], (*minHeap)[j] = (*minHeap)[j], (*minHeap)[i]
}


func (minHeap * MinHeapKSmallestPairs) Top() interface{} {
	return (*minHeap)[0]
}


func (minHeap * MinHeapKSmallestPairs) Push(x interface{}) {
	*minHeap = append(*minHeap, x.([3]int))
}


func (minHeap *MinHeapKSmallestPairs) Pop() interface{} {
	old := *minHeap
	n := len(old)
	x := old[n-1]
	*minHeap = old[0 : n-1]
	return x
}

func kSmallestPairs(list1 []int, list2 []int, k int) [][]int {
	if k > (len(list1) * len(list2)) {
		k = len(list1) * len(list2)
	}

    result, minHeap := make([][]int, 0), NewMinHeap()
	
	for idx, value := range list1 {
		heap.Push(minHeap, [3]int{value + list2[0], idx, 0})
	}

	for len(result) != k {
		currentSmallestSumPair := heap.Pop(minHeap).([3]int)
		listOnePivotIdx, listTwoPointer := currentSmallestSumPair[1], currentSmallestSumPair[2]
		result = append(result, []int{list1[listOnePivotIdx],list2[listTwoPointer]})
		listTwoNextPointer := listTwoPointer + 1
		if listTwoNextPointer == len(list2) {
			continue
		}
		heap.Push(minHeap, [3]int{list1[listOnePivotIdx] + list2[listTwoNextPointer], listOnePivotIdx, listTwoNextPointer})
	}
	return result
}
