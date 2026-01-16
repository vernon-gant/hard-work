package two_pointers

import "sort"

func TwoSum(nums []int, target int) []int {
    indices := make([]int, len(nums))
    for i := 0; i < len(nums); i++ {
        indices[i] = i
    }
    sort.Slice(indices, func(i,j int) bool {
        return nums[indices[i]] < nums[indices[j]]
    })
    start, end := 0, len(nums) - 1
    var current int
    for {
        current = nums[indices[start]] + nums[indices[end]]
        if current == target {
            return []int{indices[start], indices[end]}
        }
        if current < target {
            start++
        } else {
            end--
        }
    }
    return []int{}
}


// more elegant O(n) solution with a map

func twoSum(nums []int, target int) []int {
    numToIdx := make(map[int]int)

    for idx, num := range nums {
        numToIdx[num] = idx
    }

    for idx, num := range nums {
        idx2, found := numToIdx[target - num]
        if found && idx != idx2 {
            return []int{idx, idx2}
        }
    }

    return []int{}
}