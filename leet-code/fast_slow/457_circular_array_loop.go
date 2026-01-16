package fast_slow

func CircularArrayLoop(nums []int) bool {
	 n := len(nums)

	 next := func(idx int) int {
        result := (idx + nums[idx]) % n
        if result < 0 {
            result += n
        }
        return result
    }

    for i := 0; i < n; i++ {
        slow, fast := i, next(i)

        signMismatch := nums[i]*nums[fast] < 0 || nums[fast]*nums[next(fast)] < 0

        for fast != slow && !signMismatch {
            slow = next(slow)
            fast = next(next(fast))

            signMismatch = nums[slow]*nums[next(slow)] < 0 || nums[fast]*nums[next(fast)] < 0
        }

        if slow == fast && slow != next(slow) && !signMismatch {
            return true
        }
    }

    return false
}