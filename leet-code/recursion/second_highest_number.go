package recursion

import "unicode"

func secondHighest(s string) int {
	return secondHighestNumber(s,-1,-1)
}

func secondHighestNumber(s string, maxValue, secondMax int) int {
	if len(s) == 0 {
		return secondMax
	}
	firstSign := s[0]
	if unicode.IsDigit(rune(firstSign)) {
		digit := int(firstSign - '0')
		if digit > maxValue {
			secondMax = maxValue
			maxValue = digit
		} else if digit < maxValue && digit > secondMax {
			secondMax = max(digit,secondMax)
		}
	}
	return secondHighestNumber(s[1:],maxValue,secondMax)
}