package sliding_window

func FindMaxAverage(nums []int, k int) float64 {
    sum := 0

    for i := 0; i < k; i++ {
        sum += nums[i]
    }

    m := sum

    for i := 1; i + k <= len(nums); i++ {
        sum += nums[i+k-1] - nums[i-1]
        m = max(m, sum)
    }

    return float64(m) / float64(k)
}