package two_heaps

import "container/heap"

type MinStart [][]int

func (minHeap * MinStart) Len() int {
	return len(*minHeap)
}

func (minHeap * MinStart) Less(i,j int) bool {
	return (*minHeap)[i][0] < (*minHeap)[j][0]
}

func (minHeap * MinStart) Swap(i,j int) {
	(*minHeap)[i], (*minHeap)[j] = (*minHeap)[j], (*minHeap)[i]
}

func (minHeap * MinStart) Push(x interface{}) {
	*minHeap = append(*minHeap, x.([]int))
}

func (minHeap * MinStart) Pop() interface{} {
	last := (*minHeap)[minHeap.Len() - 1]
	*minHeap = (*minHeap)[0 : minHeap.Len() - 1]
	return last
}

func (minHeap * MinStart) Peek() []int {
	return (*minHeap)[0]
}

type MinEnd [][]int

func (minHeap * MinEnd) Len() int {
	return len(*minHeap)
}

func (minHeap * MinEnd) Less(i,j int) bool {
	return (*minHeap)[i][1] < (*minHeap)[j][1]
}

func (minHeap * MinEnd) Swap(i,j int) {
	(*minHeap)[i], (*minHeap)[j] = (*minHeap)[j], (*minHeap)[i]
}

func (minHeap * MinEnd) Push(x interface{}) {
	*minHeap = append(*minHeap, x.([]int))
}

func (minHeap * MinEnd) Pop() interface{} {
	last := (*minHeap)[minHeap.Len() - 1]
	*minHeap = (*minHeap)[0 : minHeap.Len() - 1]
	return last
}

func (minHeap * MinEnd) Peek() []int {
	return (*minHeap)[0]
}


func tasks(T [][]int) int {
	minStartHeap, minEndHeap := &MinStart{}, &MinEnd{}
	for _, task := range T {
		heap.Push(minStartHeap, task)
	}
	heap.Push(minEndHeap, heap.Pop(minStartHeap))
	machinesCount := 1
	for minStartHeap.Len() > 0 {
		top := heap.Pop(minStartHeap).([]int)
		if top[0] >= minEndHeap.Peek()[1] {
			heap.Pop(minEndHeap)
		} else {
			machinesCount++
		}
		heap.Push(minEndHeap, top)
	}
	return machinesCount
}