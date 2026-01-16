package subsets

func Subsets(nums []int) [][]int {
    n, result := len(nums), [][]int{}
	for i := 0; i < 1 << n; i++ {
		subset := []int{}
		for j := 0; j < n; j++ {
			if i & (1 << j) != 0 {
				subset = append(subset, nums[j])
			}
		}
		result = append(result, subset)
	}
	return result
}