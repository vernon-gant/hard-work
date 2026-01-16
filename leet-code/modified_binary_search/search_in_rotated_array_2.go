package modified_binary_search

func Search2(nums []int, target int) bool {
	start, end := 0, len(nums) - 1
	for mid := (start + end) / 2; start <= end; mid = (start + end) / 2 {
		if nums[mid] == target {
			return true
		}
		if nums[start] == nums[mid] {
            start++
        } else if nums[start] < nums[mid] { // Left part is sorted
            if nums[start] <= target && target < nums[mid] {
                end = mid - 1
            } else {
                start = mid + 1
            }
        } else { // Right part is sorted
            if nums[mid] < target && target <= nums[end] {
                start = mid + 1
            } else {
                end = mid - 1
            }
        }
	}
	return false
}