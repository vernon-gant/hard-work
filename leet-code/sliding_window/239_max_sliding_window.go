package sliding_window

func maxSlidingWindow(nums []int, k int) []int {
    window := make([]int, 0, k)
    result := make([]int, 0, len(nums) - k + 1)

    for i := 0; i < len(nums); i++ {
        for len(window) > 0 && nums[window[len(window)-1]] < nums[i] {
            window = window[:len(window)-1]
        }

        window = append(window, i)

        if i < k - 1 {
            continue
        }

        result = append(result, nums[window[0]])

        if window[0] == i-k+1 {
            window = window[1:]
        }
    }

    return result
}