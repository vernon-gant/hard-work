package intervals

import "sort"

func LeastInterval(tasks []byte, n int) int {
	taskFrequenciesMap := make(map[byte]int, 0)
	for _, task := range tasks {
		taskFrequenciesMap[task] = taskFrequenciesMap[task] + 1
	}
	sortedTaskFrequencies := make([]int, 0)
	for _, occurence := range taskFrequenciesMap {
		sortedTaskFrequencies = append(sortedTaskFrequencies, occurence)
	}
	sort.Slice(sortedTaskFrequencies, func(i, j int) bool {
		return sortedTaskFrequencies[i] > sortedTaskFrequencies[j]
	})
	maxFrequency := sortedTaskFrequencies[0]
	idleTime := (maxFrequency - 1) * n
	for _, frequency := range sortedTaskFrequencies[1:] {
		idleTime -= min(maxFrequency-1, frequency)
	}
	if idleTime < 0 {
		idleTime = 0
	}
	return len(tasks) + idleTime
}

//###########################################################################

func leastInterval(tasks []byte, n int) int {
	if n == 0 {
		return len(tasks)
	}

	cnt := make([]int, 26)
	for _, task := range tasks {
		cnt[task-'A']++
	}

	var maxCount, sameMaxCount int
	for _, c := range cnt {
		if c > maxCount {
			maxCount = c
			sameMaxCount = 1
		} else if c == maxCount {
			sameMaxCount++
		}
	}

	res := (n+1)*(maxCount-1) + sameMaxCount
	if res > len(tasks) {
		return res
	}

	return len(tasks)
}
