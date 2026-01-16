package greedy

func Jump2(nums []int) int {
    if len(nums) == 1 {
        return 0
    }
    steps, maxVision := 0, nums[0]
    for i := 0; i < len(nums); {
        if maxVision >= len(nums) - 1 {
            return steps + 1
        }
        steps++
        currentVision := maxVision
        for j := i; j <= currentVision; j++ {
            maxVision = max(maxVision, j + nums[j])
        }
        i = currentVision + 1
    }
    return steps
}