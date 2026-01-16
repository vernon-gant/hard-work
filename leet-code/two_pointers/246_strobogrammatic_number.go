package two_pointers

import "strings"

func IsStrobogrammatic(num string) bool {
    builder := strings.Builder{}
	var sixAscii, nineAscii uint8 = '0' + 6, '0' + 9
	for i := len(num) - 1; i >= 0; i-- {
		currentNum := num[i]
		if currentNum == sixAscii {
			currentNum = nineAscii
		} else if currentNum == nineAscii {
			currentNum = sixAscii
		}
		builder.WriteByte(currentNum)
	}
	reversed := builder.String()
	return reversed == num
}

func IsStrobogrammatic2(num string) bool {
    numToRotated := make(map[uint8]uint8)
	numToRotated[6] = 9
	numToRotated[9] = 6
	numToRotated[1] = 1
	numToRotated[8] = 8
	start, end := 0, len(num) - 1
	for start < end {
		currentStart := num[start] - '0'
		currentEnd := num[end] - '0'
		rotatedEnd, ok := numToRotated[currentEnd]
		if !ok || !(currentStart == rotatedEnd) {
			return false
		}
		start++
		end--
	}
	return true
}