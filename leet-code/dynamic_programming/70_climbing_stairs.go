package dynamic_programming

func climbStairs(n int) int {
    if n <= 2 {
        return n
    }

    first, second := 1, 2

    for i := 3; i <= n; i++ {
        third := first + second
        first = second
        second = third
    }

    return second
}

// O(2^N) solution like fibonacci

func climbStairs2(n int) int {
    return climbStairsRec(n)
}

func climbStairsRec(rest int) int {
    if rest <= 2 {
        return rest
    }
    return climbStairsRec(rest - 1) + climbStairsRec(rest - 2)
}