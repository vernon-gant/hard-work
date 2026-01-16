package dynamic_programming

func findMaxKnapsackProfit(capacity int, weights []int, values []int) int{
    dp := make([]int, capacity + 1)
    for i := 0; i < len(weights); i++ {
		for j := capacity; j >= weights[i]; j-- {
			dp[j] = max(dp[j], dp[j - weights[i]] + values[i])
		}
	}
	return dp[capacity]
}