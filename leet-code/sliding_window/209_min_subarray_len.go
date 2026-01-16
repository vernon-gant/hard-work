package sliding_window

func minSubArrayLen(target int, nums []int) int {
    start, sum, result := 0, 0, 100001
    for end := 0; end < len(nums); end++ {
        sum += nums[end]
        for ;sum >= target; start++ {
            result = min(result, end - start + 1)
            sum -= nums[start]
        }
    }
    if result == 100001 {
        return 0
    }
    return result
}