package dynamic_programming

import "math"

func rob(nums []int) int {
	if len(nums) < 4 {
		maxNum := math.MinInt
		for i := 0; i < len(nums); i++ {
			maxNum = max(maxNum,nums[i])
		}
		return maxNum
	}

	withoutFirst := maxReward(nums[1:])
	withoutLast := maxReward(nums[:len(nums) - 1])

	return max(withoutFirst, withoutLast)
}

func maxReward(nums []int) int {
	dp := make([]int, len(nums))
	dp[0] = nums[0]
	dp[1] = max(nums[0], nums[1])
	for i := 2; i < len(nums); i++ {
		dp[i] = max(dp[i - 2] + nums[i], dp[i - 1])
	}

	return dp[len(dp) - 1]
}