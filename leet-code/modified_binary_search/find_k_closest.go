package modified_binary_search

func FindClosestElements(arr []int, k int, target int) []int {
	if len(arr) == k {
		return arr
	}

	if target <= arr[0] {
		return arr[0:k]
	}

	if target >= arr[len(arr)-1] {
		return arr[len(arr)-k :]
	}

	firstClosest := binarySearchX(arr, target)

	windowLeft := firstClosest - 1
	windowRight := windowLeft + 1

	for (windowRight - windowLeft - 1) < k {
		if windowLeft == -1 {
			windowRight += 1
			continue
		}

		if windowRight == len(arr) || abs(arr[windowLeft]-target) <= abs(arr[windowRight]-target) {
			windowLeft -= 1
		} else {
			windowRight += 1
		}
	}

	return arr[windowLeft+1 : windowRight]
}

func binarySearchX(array []int, target int) int {
	left := 0
	right := len(array) - 1

	for left <= right {
		mid := (left + right) / 2

		if array[mid] == target {
			return mid
		}

		if array[mid] < target {
			left = mid + 1
		} else {
			right = mid - 1
		}
	}

	return left
}

func abs(n int) int {
	if n < 0 {
		return -n
	}
	return n
}