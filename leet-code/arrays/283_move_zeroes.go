package arrays

func moveZeroes(nums []int)  {
    nextFree := 0
    for i := 0; i < len(nums); i++ {
        if nums[i] != 0 {
            nums[nextFree] = nums[i]
            nextFree++
        }
    }
    for ; nextFree < len(nums); nextFree++ {
        nums[nextFree] = 0
    }
}

// one loop

func moveZeroes2(nums []int)  {
    nextFree := 0
    for i := 0; i < len(nums); i++ {
        if nums[i] != 0 {
            nums[nextFree], nums[i] = nums[i], nums[nextFree]
            nextFree++
        }
    }
}