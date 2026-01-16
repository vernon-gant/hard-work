package two_pointers

import "strconv"

func ValidWordAbbreviation(word string, abbr string) bool {
	if len(word) < len(abbr) {
		return false
	}

	wordPointer, abbrPointer := 0, 0

	for wordPointer < len(word) && abbrPointer < len(abbr) {
		if abbr[abbrPointer] == '0' {
			return false
		}

		if isNumber(abbr[abbrPointer]) {
			number, numberLen := getNumberWithLen(abbr, abbrPointer)
			wordPointer += number
			abbrPointer += numberLen
			continue
		}

		if word[wordPointer] != abbr[abbrPointer] {
			return false
		}

		wordPointer++
		abbrPointer++
	}

	return wordPointer == len(word) && abbrPointer == len(abbr)
}

func isNumber(letter byte) bool {
	return letter-'0' <= 9
}

func getNumberWithLen(abbr string, curIdx int) (int, int) {
	j := curIdx
	for ; j < len(abbr) && isNumber(abbr[j]); j++ {
	}
	number, _ := strconv.Atoi(abbr[curIdx:j])
	return number, j - curIdx
}
