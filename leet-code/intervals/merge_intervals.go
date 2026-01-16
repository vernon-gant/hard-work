package intervals

import "sort"

func MergeIntervals(intervals [][]int) [][]int {
	sort.Slice(intervals, func(i, j int) bool {
		return intervals[i][0] < intervals[j][0]
	})
	result := make([][]int, 0)
	currentIntervalStart, currenIntervalEnd := intervals[0][0], intervals[0][1]
	for _, interval := range intervals {
		if isOverlappingWithCurrentInterval(interval, currenIntervalEnd) {
			currentIntervalStart = min(currentIntervalStart, interval[0])
			currenIntervalEnd = max(currenIntervalEnd, interval[1])
			continue
		}
		result = append(result, []int{currentIntervalStart, currenIntervalEnd})
		currentIntervalStart = interval[0]
		currenIntervalEnd = interval[1]
	}
	result = append(result, []int{currentIntervalStart, currenIntervalEnd})
	return result
}

func isOverlappingWithCurrentInterval(incomingInterval []int, currentIntervalEnd int) bool {
	return incomingInterval[0] <= currentIntervalEnd
}
