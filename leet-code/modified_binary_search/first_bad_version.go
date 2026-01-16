package modified_binary_search

func isBadVersion(version int) bool {
    return false
}

func firstBadVersion(n int) int {
	start, end := 1, n
	var toCheck int
	for start <= end {
		toCheck = (start + end) / 2
		if isBadVersion(toCheck) {
			end = toCheck - 1
		} else {
			start = toCheck + 1
		}
	}
	return start
}
