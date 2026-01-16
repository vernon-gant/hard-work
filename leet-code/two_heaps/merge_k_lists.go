package k_way_merge

import "container/heap"

type ListNode struct {
	Val int
    Next *ListNode
}

type MinHeapKLists []*ListNode

func(minHeap * MinHeapKLists) Len() int {
	return len(*minHeap)
}

func(minHeap * MinHeapKLists) Less(i, j int) bool {
	return (*minHeap)[i].Val < (*minHeap)[j].Val
}

func(minHeap * MinHeapKLists) Swap(i, j int) {
	(*minHeap)[i], (*minHeap)[j] = (*minHeap)[j], (*minHeap)[i]
}

func(minHeap * MinHeapKLists) Push(x interface{}) {
	*minHeap = append(*minHeap, x.(*ListNode))
}

func(minHeap * MinHeapKLists) Pop() interface{} {
    last := (*minHeap)[minHeap.Len() - 1]
	*minHeap = (*minHeap)[0 : minHeap.Len() - 1]
	return last
}


func mergeKLists(lists []*ListNode) *ListNode {
	minHeap := &MinHeapKLists{}
	
	for idx, value := range lists {
		heap.Push(minHeap, value)
	}

	result := &ListNode{}
	tail := result
	
	for minHeap.Len() > 0 {
		currentMinimum := heap.Pop(minHeap)
		currentMinumumNext := currentMinimum.Next
		currentMinumum.Next := nil
		tail.Next = currentMinimum
		tail = currentMinimum
		if currentMinumumNext == nil {
			continue
		}
		heap.Push(minHeap, currentMinimumNext)
	}

	return result.Next
}