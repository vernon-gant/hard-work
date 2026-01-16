package two_pointers

import "math"

func MinMovesToMakePalindrome(s string) int {
	return minRec([]byte(s), 0)
}

func minRec(s []byte, counter int) int {
	if len(s) <= 1 {
		return counter
	}
	var minDistanceByte byte
	minDistance, distances := math.MaxInt, make(map[byte][3]int)
	for i, val := range s {
		distance, found := distances[val]
		if !found {
			distances[val] = [3]int{i, -1, -1}
			continue
		}
		distance[0] = min(i, distance[0])
		distance[1] = max(i, distance[1])
		distance[2] = distance[0] + len(s) - 1 - distance[1]
		distances[val] = distance
		if minDistance > distance[2] {
			minDistance = distance[2]
			minDistanceByte = val
		}
	}
	minDistanceArr := distances[minDistanceByte]
	toLeft, toRight := minDistanceArr[0], minDistanceArr[1]
	for toLeft > 0 {
		s[toLeft - 1], s[toLeft] = s[toLeft], s[toLeft - 1]
		toLeft--
		counter++
	}
	for toRight < len(s) - 1 {
		s[toRight], s[toRight + 1] = s[toRight + 1], s[toRight]
		toRight++
		counter++
	}
	return minRec(s[1 : len(s) - 1], counter)
}
