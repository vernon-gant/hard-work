package dynamic_programming

func countBits(n int) []int {
    result := make([]int, n + 1)

    if n == 0 {
        return result
    }

    result[1] = 1

    if n == 1 {
        return result
    }

    result[2] = 1

    if n == 2 {
        return result
    }

    for i := 3; i <= n; i++ {
        if i % 2 == 0 {
            result[i] = result[i / 2]
        } else {
            result[i] = result[i - 1] + 1
        }
    }

    return result
}