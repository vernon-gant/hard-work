package intervals

func Insert(intervals [][]int, newInterval []int) [][]int {
	currentPosition, result := 0, [][]int{newInterval}
	for i := 0; i < len(intervals); i++ {
		if intervals[i][1] < newInterval[0] {
			result = append(result, newInterval)
			result[currentPosition] = intervals[i]
			currentPosition++
			continue
		}
		if newInterval[1] < intervals[i][0] {
			result = append(result, intervals[i])
			continue
		}

		for ; i < len(intervals) && newInterval[1] >= intervals[i][0]; i++ {
			newInterval[0] = min(newInterval[0], intervals[i][0])
			newInterval[1] = max(newInterval[1], intervals[i][1])
		}
		i--
	}
	return result
}