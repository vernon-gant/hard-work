package k_way_merge

import "container/heap"

type MinHeapKthSmallestMatrix [][3]int

func(minHeap * MinHeapKthSmallestMatrix) Len() int {
	return len(*minHeap)
}

func(minHeap * MinHeapKthSmallestMatrix) Less(i, j int) bool {
	return (*minHeap)[i][2] < (*minHeap)[j][2]
}

func(minHeap * MinHeapKthSmallestMatrix) Swap(i, j int) {
	(*minHeap)[i], (*minHeap)[j] = (*minHeap)[j], (*minHeap)[i]
}

func(minHeap * MinHeapKthSmallestMatrix) Push(x interface{}) {
	*minHeap = append(*minHeap, x.([3]int))
}

func(minHeap * MinHeapKthSmallestMatrix) Pop() interface{} {
	last := (*minHeap)[minHeap.Len() - 1]
	*minHeap = (*minHeap)[0 : minHeap.Len() - 1]
	return last
}

func(minHeap * MinHeapKthSmallestMatrix) Peek() [3]int {
	return (*minHeap)[0]
}

func kthSmallestElement(matrix [][]int, k int) int {
    processed, minHeap, counter, first := make(map[[3]int]bool), &MinHeapKthSmallestMatrix{}, 1, [3]int{0,0, matrix[0][0]}
	processed[first] = true
	heap.Push(minHeap, first)
	for ;counter != k; counter++ {
		currentSmallest := heap.Pop(minHeap).([3]int)
		for _, neighbour := range getNeighbours(matrix, currentSmallest) {
			if found, _ := processed[neighbour]; !found {
				heap.Push(minHeap, neighbour)
				processed[neighbour] = true
			}
		}
	}
	return minHeap.Peek()[2]
}

func getNeighbours(matrix [][]int, currentElement [3]int) [][3]int {
	result := [][3]int{}
	currentElementRow, currentElementColumn := currentElement[0], currentElement[1]
	neighbour := [3]int{currentElementRow - 1, currentElementColumn - 1, 0}
	if validNeighbour(len(matrix), neighbour) {
		neighbour[2] = matrix[currentElementRow - 1][currentElementColumn - 1]
		result = append(result, neighbour)
	}
	neighbour[0] = currentElementRow - 1
	neighbour[1] = currentElementColumn
	if validNeighbour(len(matrix), neighbour) {
		neighbour[2] = matrix[currentElementRow - 1][currentElementColumn]
		result = append(result, neighbour)
	}
	neighbour[0] = currentElementRow - 1
	neighbour[1] = currentElementColumn + 1
	if validNeighbour(len(matrix), neighbour) {
		neighbour[2] = matrix[currentElementRow - 1][currentElementColumn + 1]
		result = append(result, neighbour)
	}
	neighbour[0] = currentElementRow
	neighbour[1] = currentElementColumn - 1
	if validNeighbour(len(matrix), neighbour) {
		neighbour[2] = matrix[currentElementRow][currentElementColumn - 1]
		result = append(result, neighbour)
	}
	neighbour[0] = currentElementRow
	neighbour[1] = currentElementColumn + 1
	if validNeighbour(len(matrix), neighbour) {
		neighbour[2] = matrix[currentElementRow][currentElementColumn + 1]
		result = append(result, neighbour)
	}
	neighbour[0] = currentElementRow + 1
	neighbour[1] = currentElementColumn - 1
	if validNeighbour(len(matrix), neighbour) {
		neighbour[2] = matrix[currentElementRow + 1][currentElementColumn - 1]
		result = append(result, neighbour)
	}
	neighbour[0] = currentElementRow + 1
	neighbour[1] = currentElementColumn
	if validNeighbour(len(matrix), neighbour) {
		neighbour[2] = matrix[currentElementRow + 1][currentElementColumn]
		result = append(result, neighbour)
	}
	neighbour[0] = currentElementRow + 1
	neighbour[1] = currentElementColumn + 1
	if validNeighbour(len(matrix), neighbour) {
		neighbour[2] = matrix[currentElementRow + 1][currentElementColumn + 1]
		result = append(result, neighbour)
	}
	return result
}

func validNeighbour(matrixLen int, neighbour [3]int) bool {
	return neighbour[0] >= 0 && neighbour[0] < matrixLen && neighbour[1] >= 0 && neighbour[1] < matrixLen
}