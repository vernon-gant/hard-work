package top_k_elements

import "container/heap"

type WordFrequency struct {
    Word string
    Frequency int
}

type MinHeapTopKWords []WordFrequency

func(minHeap * MinHeapTopKWords) Len() int {
	return len(*minHeap)
}

func(minHeap * MinHeapTopKWords) Less(i, j int) bool {
    if (*minHeap)[i].Frequency == (*minHeap)[j].Frequency {
        return (*minHeap)[i].Word > (*minHeap)[j].Word
    }
    return (*minHeap)[i].Frequency < (*minHeap)[j].Frequency
}

func(minHeap * MinHeapTopKWords) Swap(i, j int) {
    (*minHeap)[i], (*minHeap)[j] = (*minHeap)[j], (*minHeap)[i]
}

func(minHeap * MinHeapTopKWords) Push(x interface{}) {
	*minHeap = append(*minHeap, x.(WordFrequency))
}

func(minHeap * MinHeapTopKWords) Pop() interface{}  {
	last := (*minHeap)[minHeap.Len() - 1]
	*minHeap = (*minHeap)[0 : minHeap.Len() - 1]
	return last
}

func(minHeap * MinHeapTopKWords) Peek() WordFrequency {
	return (*minHeap)[0]
}

func topKFrequentWords(words []string, k int) []string {
	frequencies := make(map[string]int)
	for _, word := range words {
		frequencies[word]++
	}
	minHeap := &MinHeapTopKWords{}
	for word, frequency := range frequencies {
		if minHeap.Len() < k {
			heap.Push(minHeap, WordFrequency{ Word : word, Frequency : frequency })
		} else if top := minHeap.Peek(); top.Frequency < frequency || (top.Frequency == frequency && top.Word > word) {
			heap.Pop(minHeap)
			heap.Push(minHeap, WordFrequency{ Word : word, Frequency : frequency })
		}
	}
	result := make([]string, k)
	k--
	for ; minHeap.Len() > 0; k-- {
		result[k] = heap.Pop(minHeap).(WordFrequency).Word
	}
	return result
}