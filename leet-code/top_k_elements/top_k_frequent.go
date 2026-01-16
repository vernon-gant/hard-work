package top_k_elements

import "container/heap"

type MinHeapTopKFrequent [][2]int

func(minHeap * MinHeapTopKFrequent) Len() int {
	return len(*minHeap)
}

func(minHeap * MinHeapTopKFrequent) Less(i, j int) bool {
	return (*minHeap)[i][1] < (*minHeap)[j][1]
}

func(minHeap * MinHeapTopKFrequent) Swap(i, j int) {
	(*minHeap)[i], (*minHeap)[j] = (*minHeap)[j], (*minHeap)[i]
}

func(minHeap * MinHeapTopKFrequent) Push(x interface{}) {
	*minHeap = append(*minHeap, x.([2]int))
}

func(minHeap * MinHeapTopKFrequent) Pop() interface{} {
	last := (*minHeap)[minHeap.Len() - 1]
	*minHeap = (*minHeap)[0 : minHeap.Len() - 1]
	return last
}

func(minHeap * MinHeapTopKFrequent) PeekFrequency() int {
	return (*minHeap)[0][1]
}

func topKFrequent(nums []int, k int) []int {
	frequencies := make(map[int]int)
	for _, val := range nums {
		frequencies[val]++
	}
	minHeap := &MinHeapTopKFrequent{}
	for number, frequency := range frequencies {
		if minHeap.Len() < k {
			heap.Push(minHeap, [2]int{number, frequency})
		} else if minHeap.PeekFrequency() < frequency {
			heap.Pop(minHeap)
			heap.Push(minHeap, [2]int{number, frequency})
		}
	}
	result := make([]int,0)
	for _, val := range *minHeap {
		result = append(result, val[0])
	}
	return result
}