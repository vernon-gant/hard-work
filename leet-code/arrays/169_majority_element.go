package arrays

import "slices"

func MajorityElement(nums []int) int {
    slices.Sort(nums)
    counter := 0
    for i := 0; i < len(nums); i++ {
        for i != len(nums) - 1 && nums[i] == nums[i + 1] {
            i++
            counter++
        }
        if counter + 1 >= len(nums) / 2 + 1 {
            return nums[i]
        }
        counter = 0
    }
    return 0
}

// O(n)

func majorityElement(nums []int) int {
	result, count := 0, 0
	for _, num := range nums {
		switch {
		case count == 0:
			result = num
			count = 1
		case num == result:
			count++
		default:
			count--
		}
	}
	return result
}