package top_k_elements

import "container/heap"

type MinHeap []int

func (minHeap* MinHeap) Len() int  {
	return len(*minHeap)
}

func (minHeap* MinHeap) Less(i, j int) bool  {
	return (*minHeap)[i] < (*minHeap)[j]
}

func (minHeap* MinHeap) Swap(i, j int)  {
	(*minHeap)[i], (*minHeap)[j] = (*minHeap)[j], (*minHeap)[i]
}

func (minHeap* MinHeap) Push(x interface{})  {
	*minHeap = append(*minHeap, x.(int))
}

func (minHeap* MinHeap) Pop() interface{}  {
	last := (*minHeap)[minHeap.Len() - 1]
	*minHeap = (*minHeap)[0 : minHeap.Len() - 1]
	return last
}

func (minHeap * MinHeap) Peek() int  {
	return (*minHeap)[0]
}

type KthLargest struct {
    minHeap * MinHeap
    k int
}


func Constructor(k int, nums []int) KthLargest {
    kthLargest := KthLargest{ minHeap : &MinHeap{}, k : k }
    for _, val := range nums {
        kthLargest.Add(val)
    }
	return kthLargest
}


func (this *KthLargest) Add(val int) int {
    if this.minHeap.Len() < this.k {
        heap.Push(this.minHeap, val)
    } else if val > this.minHeap.Peek() {
		heap.Pop(this.minHeap)
	    heap.Push(this.minHeap, val)
	}
	return this.minHeap.Peek()
}