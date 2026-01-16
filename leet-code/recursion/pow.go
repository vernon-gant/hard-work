package recursion

func MyPow(x float64, n int) float64 {
	if n < 0 {
		x , n = 1/x, -n
	}
	return myPow(x ,n)
}

func myPow(x float64, n int) float64 {
	if n == 1 {
		return x
	}
	if n % 2 == 1 {
		return x * myPow(x,n - 1)
	} else {
		return myPow(x * x,n / 2)
	}
}