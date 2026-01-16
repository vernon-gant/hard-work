package modified_binary_search

func SingleNonDuplicate(nums []int) int {
    low, high := 0, len(nums)-1
    for low < high {
        mid := (low + high) / 2
        if mid%2 == 1 {
            mid--
        }
        if nums[mid] != nums[mid+1] {
            high = mid
        } else {
            low = mid + 2
        }
    }
    return nums[low]
}