package intervals

import (
	"container/heap"
	"sort"
)

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
	heapLen := len(*minHeap)
	last := (*minHeap)[heapLen - 1]
	*minHeap = (*minHeap)[0:heapLen - 1]
	return last
}

func (minHeap * MinHeap) Peek() interface{}  {
	if minHeap.Len() == 0 {
		panic(minHeap)
	}
	return (*minHeap)[0]
}

func findSets(intervals [][]int) int{
	sort.Slice(intervals, func(i, j int) bool {
		if intervals[i][0] == intervals[j][0] {
			return intervals[i][1] < intervals[j][1]
		}
		return intervals[i][0] < intervals[j][0]
	})
	meetingEndTimes := &MinHeap{ intervals[0][1] }
	for _, interval := range intervals[1:] {
		if interval[0] >= meetingEndTimes.Peek().(int) {
			heap.Pop(meetingEndTimes)
		}
		heap.Push(meetingEndTimes,interval[1])
	}
	return meetingEndTimes.Len()
}