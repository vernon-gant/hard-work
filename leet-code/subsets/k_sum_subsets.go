package subsets

func getKSumSubsets(setOfIntegers[]int, targetSum int) [][]int {
	result := [][]int{}
	for i := 1; i <= 1 << len(setOfIntegers); i++ {
		tempSet, tempSetSum := []int{}, 0
		for j := 0; j < len(setOfIntegers); j++ {
			if i & (1 << j) != 0 {
				tempSet = append(tempSet, setOfIntegers[j])
				tempSetSum += setOfIntegers[j]
			}
		}
		if tempSetSum == targetSum {
			result = append(result, tempSet)
		}
	}
	return result
}