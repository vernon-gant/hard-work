package top_k_elements

import "container/heap"

func findKthLargest(nums []int, k int) int {
    minHeap := &MinHeap{}
    for _, value := range nums {
        if minHeap.Len() < k {
            heap.Push(minHeap, value)
        } else if value > minHeap.Peek() {
            heap.Pop(minHeap)
            heap.Push(minHeap, value)
        }
    }
    return minHeap.Peek()
}