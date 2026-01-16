package two_pointers

func RemoveDuplicates(nums []int) int {
	nextInsertIdx := 1
	for i := 1; i < len(nums); i++ {
		if nums[i - 1] != nums[i] {
			nums[nextInsertIdx] = nums[i]
			nextInsertIdx++
		}
	}
	return nextInsertIdx
}
