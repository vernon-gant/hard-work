package sliding_window

import "slices"

func maxFrequency(nums []int, k int) int {
	slices.Sort(nums)

	start, sum, result := 0, 0, 1

	for end := 0; end < len(nums); end++ {
		sum += nums[end]
		for ; nums[end]*(end-start+1) > sum+k; start++ {
			sum -= nums[start]
		}
		result = max(result, end-start+1)
	}

	return result
}
