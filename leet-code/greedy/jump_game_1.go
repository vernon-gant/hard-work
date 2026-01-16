package greedy

func JumpGame(nums []int) bool {
    for i := len(nums) - 1; i > 0; {
        j := i - 1
        for ; j >= 0 && j+nums[j] < i; j-- {
        }
        if j == -1 {
            return false
        } else {
            i = j
        }
    }
    return true
}

func CanJump2(nums []int) bool {
    for i, maxJump := 0, 0; i < len(nums); i++ {
        if nums[i] > maxJump {
            maxJump = nums[i]
        }

        if maxJump == 0 && i != len(nums)-1 {
            return false
        }

        maxJump--
    }

    return true
}
