package dynamic_programming

func canPartition(nums []int) bool {
    sum := 0

    for _, v := range nums {
        sum += v
    }

    if sum % 2 != 0 {
        return false
    }

    halfSum := sum / 2

    dp := make([]bool, halfSum + 1)
    dp[0] = true

    for i := 0; i < len(nums) - 1; i++ {
        for j := halfSum; j >= nums[i]; j-- {
            dp[j] = dp[j] || dp[j - nums[i]]
        }
    }

    return dp[halfSum] && dp[halfSum - nums[len(nums) - 1]]
}