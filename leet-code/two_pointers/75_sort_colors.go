package two_pointers

func SortColors(nums []int) {
    start, end := 0, len(nums)-1
    for start <= end {
        if nums[start] == 2 {
            nums[start], nums[end] = nums[end], nums[start]
            end--
        } else if nums[end] == 0 {
            nums[start], nums[end] = nums[end], nums[start]
            start++
        } else if nums[start] == 0 {
            start++
        } else if nums[end] == 2 {
            end--
        } else if nums[start] == 1 && nums[end] == 1 {
            var temp int
            for temp = start; temp < len(nums) && nums[temp] == 1; temp++ {
            }
            if temp > end {
                break
            }
            nums[start], nums[temp] = nums[temp], nums[start]
        }
    }
}

func sortColor(nums []int) {
    red, white, blue := 0, 0, 0
    for _, value := range nums {
        if value == 0 {
            red++
        } else if value == 1 {
            white++
        } else {
            blue++
        }
    }
    for idx, _ := range nums {
        if red > 0 {
            nums[idx] = 0
            red--
        } else if white > 0 {
            nums[idx] = 1
            white--
        } else {
            nums[idx] = 2
            blue--
        }
    }
}
