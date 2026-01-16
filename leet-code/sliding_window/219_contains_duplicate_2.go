package sliding_window

func containsNearbyDuplicate(nums []int, k int) bool {
    windowFreq := make(map[int]bool)
    for i := 0; i < len(nums); i++ {
        if inWindow, found := windowFreq[nums[i]]; found && inWindow {
            return true
        }
        windowFreq[nums[i]] = true
        if len(windowFreq) > k {
            delete(windowFreq, nums[i - k])
        }
    }
    return false
}