package modified_binary_search

func binarySearch(nums [] int, target int) int {
	startIdx, endIdx := 0, len(nums) - 1
    var middleIdx int
	for startIdx <= endIdx {
		middleIdx = (startIdx + endIdx) / 2
		if nums[middleIdx] == target {
			return middleIdx
		} else if nums[middleIdx] < target {
			startIdx = middleIdx + 1
		} else {
			endIdx = middleIdx - 1
		}
	}
	return -1
}