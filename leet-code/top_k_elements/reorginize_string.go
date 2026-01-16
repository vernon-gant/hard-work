package top_k_elements

import (
	"container/heap"
	"strings"
)

type MaxHeap [][2]int

func (maxHeap * MaxHeap) Len() int  {
	return len(*maxHeap)
}

func (maxHeap* MaxHeap) Less(i, j int) bool  {
	return (*maxHeap)[i][1] > (*maxHeap)[j][1]
}

func (maxHeap* MaxHeap) Swap(i, j int)  {
	(*maxHeap)[i], (*maxHeap)[j] = (*maxHeap)[j], (*maxHeap)[i]
}

func (maxHeap* MaxHeap) Push(x interface{})  {
	*maxHeap = append(*maxHeap, x.([2]int))
}

func (maxHeap* MaxHeap) Pop() interface{}  {
	last := (*maxHeap)[maxHeap.Len() - 1]
	*maxHeap = (*maxHeap)[0 : maxHeap.Len() - 1]
	return last
}

func (maxHeap * MaxHeap) Peek() [2]int  {
	return (*maxHeap)[0]
}

func ReorganizeString(s string) string {
	frequencies := [26]int{}
	for _, letter := range s {
		frequencies[letter - 'a']++
	}
	maxHeap := &MaxHeap{}
	for idx, value := range frequencies {
		if value != 0 {
			heap.Push(maxHeap,[2]int{idx, value})
		}
	}
	if (*maxHeap)[0][1] > (len(s)+1)/2 {
		return ""
	}
	stringBuilder, prev := strings.Builder{}, [2]int{}
	for maxHeap.Len() > 0 {
		current := heap.Pop(maxHeap).([2]int)
		stringBuilder.WriteByte(byte('a' + current[0]))
		current[1]--
		if prev[1] > 0 {
			heap.Push(maxHeap, prev)
		}
		prev = current
	}
	return stringBuilder.String()
}