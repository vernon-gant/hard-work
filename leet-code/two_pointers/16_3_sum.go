package two_pointers

import "slices"

func threeSum(nums []int) [][]int {
    slices.Sort(nums)
    result := make([][]int, 0)
    var start, end, sum int
    for i := 0; i < len(nums)-2; i++ {
		if i != 0 && nums[i] == nums[i - 1] {
			continue
		}
        start, end = i+1, len(nums)-1
        for start < end {
			if start != i + 1 && nums[start] == nums[start - 1] {
				start++
				continue
			}
			if end != len(nums) - 1 && nums[end] == nums[start + 1] {
				end--
				continue
			}
            sum = nums[i] + nums[start] + nums[end]
            if sum == 0 {
                result = append(result, []int{nums[i], nums[start], nums[end]})
            }
            if sum > 0 {
                end--
                continue
            }
            start++
        }
    }
    return result
}