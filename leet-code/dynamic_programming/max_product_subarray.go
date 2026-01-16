package dynamic_programming

func NaxProduct(nums []int) int {
	maxWithNeg, maxOnlyPos, curWithNeg, curOnlyPos := 1, 1, 1, 1
	for i := 0; i < len(nums); i++ {
		if nums[i] == 0 {
			curWithNeg = 0
			curOnlyPos = 0
		} else if nums[i] < 0 {
			curOnlyPos = 1
			curWithNeg *= nums[i]
			maxWithNeg = max(maxWithNeg, curWithNeg)
		} else {
			curOnlyPos *= nums[i]
			maxOnlyPos = max(maxOnlyPos, nums[i])
		}
	}

	return max(maxWithNeg, maxOnlyPos)
}
