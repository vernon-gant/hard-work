package sliding_window

func maxProfit(prices []int) int {
	if len(prices) == 0 { return 0 }
	head, biggestProfit := 0, 0
	for tail := 1; tail < len(prices); tail++ {
		if prices[tail] > prices[head] {
			biggestProfit = max(prices[tail] - prices[head], biggestProfit)
		} else {
			head = tail
		}
	}
    
    return biggestProfit
}