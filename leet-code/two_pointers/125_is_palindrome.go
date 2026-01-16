package two_pointers

func IsPalindrome(s string) bool {
	start, end := 0, len(s) - 1
	for start < end {
		if !isCharacter(s[start]) {
			start++
			continue
		}
		if !isCharacter(s[end]) {
			end--
			continue
		}
		if toLower(s[start]) != toLower(s[end]) {
			return false
		}
		start++
		end--
	}
	return true
}

func isCharacter(character byte) bool {
	return (character >= '0' && character <= '9') || (character >= 'A' && character <= 'Z') || (character >= 'a' && character <= 'z')
}

func toLower(character byte) byte {
	if character >= 'A' && character <= 'Z' {
		return character + 32
	}
	return character
}