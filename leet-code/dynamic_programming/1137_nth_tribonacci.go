package dynamic_programming

func tribonacci(n int) int {
    if n == 0 {
        return 0
    }

    if n == 1 || n == 2 {
        return 1
    }

    first, second, third := 0, 1, 1
	var temp int

	for i := 3; i <= n; i++ {
		temp = first + second + third
		first = second
		second = third
		third = temp
	}

	return third
}