package two_heaps

import "container/heap"

type MaxHeapStream []int

func (maxHeap * MaxHeapStream) Len() int {
	return len(*maxHeap)
}

func (maxHeap * MaxHeapStream) Swap(i, j int) {
	(*maxHeap)[i],(*maxHeap)[j] = (*maxHeap)[j], (*maxHeap)[i]
}

func (maxHeap * MaxHeapStream) Less(i, j int) bool {
	return (*maxHeap)[i] > (*maxHeap)[j]
}

func (maxHeap * MaxHeapStream) Pop() interface{} {
	last := (*maxHeap)[maxHeap.Len() - 1]
	*maxHeap = (*maxHeap)[0 : maxHeap.Len() - 1]
	return last
}

func (maxHeap * MaxHeapStream) Peek() interface{} {
	return (*maxHeap)[0]
}

func (maxHeap * MaxHeapStream) Push(x interface{}) {
	*maxHeap = append(*maxHeap, x.(int))
}

type MinHeapStream []int

func (minHeap * MinHeapStream) Len() int {
	return len(*minHeap)
}

func (minHeap * MinHeapStream) Swap(i, j int) {
	(*minHeap)[i],(*minHeap)[j] = (*minHeap)[j], (*minHeap)[i]
}

func (minHeap * MinHeapStream) Less(i, j int) bool {
	return (*minHeap)[i] < (*minHeap)[j]
}

func (minHeap * MinHeapStream) Pop() interface{} {
	last := (*minHeap)[minHeap.Len() - 1]
	*minHeap = (*minHeap)[0 : minHeap.Len() - 1]
	return last
}

func (minHeap * MinHeapStream) Peek() interface{} {
	return (*minHeap)[0]
}

func (minHeap * MinHeapStream) Push(x interface{}) {
	*minHeap = append(*minHeap, x.(int))
}


type MedianFinder struct {
	minHeap * MinHeapStream
	maxHeap * MaxHeapStream
}

func Constructor() MedianFinder {
    minHeap := &MinHeapStream{}
	maxHeap := &MaxHeapStream{}
	return MedianFinder{ minHeap : minHeap, maxHeap : maxHeap }
}

func (this *MedianFinder) AddNum(num int)  {
	if this.StreamLen() == 0 || float64(num) >= this.FindMedian() {
		heap.Push(this.minHeap, num)
	} else {
		heap.Push(this.maxHeap, num)
	}

    if this.NeedPullFromMinHeap() {
		heap.Push(this.maxHeap,heap.Pop(this.minHeap))
	} else if this.NeedPullFromMaxHeap() {
		heap.Push(this.minHeap,heap.Pop(this.maxHeap))
	}
}

func (this *MedianFinder) FindMedian() float64 {
    if this.StreamLen() % 2 == 0 {
		return float64(this.minHeap.Peek().(int) + this.maxHeap.Peek().(int)) / 2
	}
	return float64(this.minHeap.Peek().(int))
}

func (this *MedianFinder) NeedPullFromMinHeap() bool {
	return this.minHeap.Len() == this.maxHeap.Len() + 2
}

func (this *MedianFinder) NeedPullFromMaxHeap() bool {
	return this.minHeap.Len() + 1 == this.maxHeap.Len()
}

func (this *MedianFinder) StreamLen() int {
	return this.minHeap.Len() + this.maxHeap.Len()
}