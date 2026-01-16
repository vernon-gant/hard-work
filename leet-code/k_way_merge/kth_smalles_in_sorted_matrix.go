package k_way_merge

import "container/heap"

type MinHeap struct {
	Heap [][2]int
	Matrix [][]int
}

func(minHeap * MinHeap) Len() int {
	return len((*minHeap).Heap)
}

func(minHeap * MinHeap) Less(i, j int) bool {
	return (*minHeap).Matrix[(*minHeap).Heap[i][0]][(*minHeap).Heap[i][1]] < (*minHeap).Matrix[(*minHeap).Heap[j][0]][(*minHeap).Heap[j][1]]
}

func(minHeap * MinHeap) Swap(i, j int) {
	(*minHeap).Heap[i], (*minHeap).Heap[j] = (*minHeap).Heap[j], (*minHeap).Heap[i]
}

func(minHeap * MinHeap) Push(x interface{}) {
	(*minHeap).Heap = append((*minHeap).Heap, x.([2]int))
}

func(minHeap * MinHeap) Pop() interface{} {
    last := (*minHeap).Heap[minHeap.Len() - 1]
	(*minHeap).Heap = (*minHeap).Heap[0 : minHeap.Len() - 1]
	return last
}

func kthSmallest(matrix [][]int, k int) int {
    minHeap := &MinHeap{ Matrix : matrix }
	for idx, _ := range matrix {
		heap.Push(minHeap, [2]int{ idx, 0 })
	}
	currentSmallest, poppedCounter := 0, 0
	for ;poppedCounter != k; poppedCounter++ {
		popped := heap.Pop(minHeap).([2]int)
		currentSmallest = matrix[popped[0]][popped[1]]
		if popped[1] == len(matrix[popped[0]]) - 1 {
			continue
		}
		popped[1] = popped[1] + 1
		heap.Push(minHeap, popped)
	}
	return currentSmallest
}