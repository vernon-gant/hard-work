package two_heaps

import (
	"container/heap"
)

type MaxHeap []int

func (heap *MaxHeap) Len() int {
	return len(*heap)
}

func (heap *MaxHeap) Less(i, j int) bool {
	return (*heap)[i] > (*heap)[j]
}

func (heap *MaxHeap) Swap(i, j int) {
	(*heap)[i], (*heap)[j] = (*heap)[j], (*heap)[i]
}

func (heap *MaxHeap) Push(x interface{}) {
	*heap = append(*heap, x.(int))
}

func (heap *MaxHeap) Pop() interface{} {
	last := (*heap)[heap.Len()-1]
	*heap = (*heap)[0 : heap.Len()-1]
	return last
}

func (heap *MaxHeap) Peek() interface{} {
	if heap.Len() == 0 {
		panic(heap)
	}
	return (*heap)[0]
}

type MinHeap [][2]int

func (minHeap *MinHeap) Len() int {
	return len(*minHeap)
}

func (minHeap *MinHeap) Less(i, j int) bool {
	return (*minHeap)[i][0] < (*minHeap)[j][0]
}

func (minHeap *MinHeap) Swap(i, j int) {
	(*minHeap)[i], (*minHeap)[j] = (*minHeap)[j], (*minHeap)[i]
}

func (minHeap *MinHeap) Push(x interface{}) {
	*minHeap = append(*minHeap, x.([2]int))
}

func (minHeap *MinHeap) Pop() interface{} {
	heapLen := len(*minHeap)
	last := (*minHeap)[heapLen-1]
	*minHeap = (*minHeap)[0 : heapLen-1]
	return last
}

func (minHeap *MinHeap) Peek() interface{} {
	if minHeap.Len() == 0 {
		panic(minHeap)
	}
	return (*minHeap)[0]
}

func MaximumCapital(c int, k int, capitals []int, profits []int) int {
	projectDoneCounter := 0
	maxProfitHeap, minCapitalHeap := &MaxHeap{}, &MinHeap{}
	for idx, capital := range capitals {
		heap.Push(minCapitalHeap, [2]int{capital, idx})
	}
	if minCapitalHeap.Peek().([2]int)[0] > c {
		return c
	}
	for projectDoneCounter < k {
		for ; minCapitalHeap.Len() > 0 && c >= minCapitalHeap.Peek().([2]int)[0]; {
			currentCapitalWithIdx := heap.Pop(minCapitalHeap).([2]int)
			heap.Push(maxProfitHeap, profits[currentCapitalWithIdx[1]])
		}
		c += heap.Pop(maxProfitHeap).(int)
		projectDoneCounter++
	}
	return c
}

func FindMaximizedCapital(k int, w int, profits []int, capital []int) int {
	projectDoneCounter := 0
	maxProfitHeap, minCapitalHeap := &MaxHeap{}, &MinHeap{}
	for idx, singleCapital := range capital {
		heap.Push(minCapitalHeap, [2]int{singleCapital, idx})
	}
	if minCapitalHeap.Peek().([2]int)[0] > w {
		return w
	}
	for ; projectDoneCounter < k; projectDoneCounter++ {
		for ; minCapitalHeap.Len() > 0 && w >= minCapitalHeap.Peek().([2]int)[0]; {
			currentCapitalWithIdx := heap.Pop(minCapitalHeap).([2]int)
			heap.Push(maxProfitHeap, profits[currentCapitalWithIdx[1]])
		}
		if maxProfitHeap.Len() == 0 {
			continue
		}
		w += heap.Pop(maxProfitHeap).(int)
	}
	return w
}
