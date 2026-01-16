package dynamic_programming

func CoinChange(coins []int, amount int) int {
    if amount == 0 {
        return 0
    }

    dp := make([]int, amount + 1)

    for i := 1; i <= amount; i++ {
        for j := 0; j < len(coins); j++ {
            if coins[j] > i { continue }

            if i == coins[j] {
                dp[i] = 1
                continue
            }

            if dp[i - coins[j]] == 0 { continue }

            if dp[i] == 0 {
                dp[i] = dp[i - coins[j]] + 1
                continue
            }

            dp[i] = min(dp[i], dp[i - coins[j]] + 1)
        }
    }

    if dp[amount] == 0 {
        return -1
    }

    return dp[amount]
}