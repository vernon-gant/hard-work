package modified_binary_search

func search(nums []int, target int) int {
    start, end := 0, len(nums) - 1
    var mid int
    for start <= end {
        mid = (start + end) / 2
        if nums[mid] == target {
            return mid
        } else if (nums[mid] > nums[end] && (target <= nums[end] || target > nums[mid])) || (target > nums[mid] && target <= nums[end]) {
            start = mid + 1
        } else {
            end = mid - 1
        }
    }
    return -1
}
