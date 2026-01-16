package modified_binary_search

import "math/rand"

type Solution struct {
	pivots []int
	maxValue int
}


func Constructor(w []int) Solution {
	pivots := make([]int, len(w) + 1)
	for idx, value := range w {
		pivots[idx + 1] = pivots[idx] + value
	}
	return Solution{pivots : pivots, maxValue : pivots[len(pivots) - 1]}
}


func (this *Solution) PickIndex() int {
	random := rand.Intn(this.maxValue)
	start, end := 0, len(this.pivots) - 1
	for toCheck := (start + end) / 2; start <= end; toCheck = (start + end) / 2 {
		if this.pivots[toCheck] > random {
			end = toCheck - 1
		} else {
			start = toCheck + 1
		}
	}
	return start - 1
}