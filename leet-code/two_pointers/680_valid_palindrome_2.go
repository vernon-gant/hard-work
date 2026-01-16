package two_pointers

func ValidPalindrome2(s string) bool {
	if isPalindrome(s) { return true }
	start, end := 0, len(s) - 1
	for start < end {
		if s[start] != s[end] {
			return isPalindrome(s[start + 1 : end + 1]) || isPalindrome(s[start : end])
		}
		start++
		end--
	}
	return true
}

func isPalindrome(s string) bool {
	if len(s) == 0 || len(s) == 1 {
		return true
	}
	if s[0] != s[len(s) - 1] {
		return false
	}
	return isPalindrome(s[1:len(s) - 1])
}