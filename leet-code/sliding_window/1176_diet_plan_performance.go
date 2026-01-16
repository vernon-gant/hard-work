package sliding_window

func dietPlanPerformance(calories []int, k int, lower int, upper int) int {
	sum := 0
	result := 0

	for i := 0; i < k; i++ {
		sum += calories[i]
	}

	getScore := func() int {
		if sum < lower {
			return -1
		}
		if sum > upper {
			return 1
		}
		return 0
	}

	result += getScore()

	for i := 1; i+k <= len(calories); i++ {
		sum += calories[i+k-1] - calories[i-1]
		result += getScore()
	}

	return result
}