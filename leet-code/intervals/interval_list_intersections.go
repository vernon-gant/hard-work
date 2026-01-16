package intervals

func IntervalIntersection(firstList [][]int, secondList [][]int) [][]int {
	result := [][]int{}
	firstPointer, secondPointer := 0, 0
	for ; firstPointer < len(firstList) && secondPointer < len(secondList); {
		currentFirst, currentSecond := firstList[firstPointer], secondList[secondPointer]
		if areDisjoint(currentFirst, currentSecond) {
			moveSmaller(currentFirst,currentSecond,&firstPointer,&secondPointer)
			continue
		}
		result = append(result, getInterseciont(currentFirst, currentSecond))
		if firstIntervalEndsEarlier(currentFirst, currentSecond) {
			firstPointer++
			continue
		}
		secondPointer++
	}
	return result
}

func moveSmaller(currentFirst, currentSecond []int, firstPointer, secondPointer *int) {
	if firstIntervalEndsEarlier(currentFirst,currentSecond) {
		*firstPointer++
		return
	}
	*secondPointer++
}

func areDisjoint(first, second []int) bool {
	if first[1] < second[0] || second[1] < first[0] {
		return true
	}
	return false
}

func firstIntervalEndsEarlier(first, second []int) bool {
	if first[1] < second[1] {
		return true
	}
	return false
}

func getInterseciont(first,second []int) []int {
	return []int{max(first[0],second[0]),min(first[1],second[1])}
}

//###############################################################################

func IntervalsIntersectionClean(firstList [][]int, secondList [][]int) [][]int {
	firstPointer, secondPointer := 0, 0
	intersections := make([][]int, 0)

	for firstPointer < len(firstList) && secondPointer < len(secondList) {
		intersectionStart := max(firstList[firstPointer][0], secondList[secondPointer][0])
		intersectionEnd := min(firstList[firstPointer][1], secondList[secondPointer][1])

		if intersectionStart <= intersectionEnd {
			intersections = append(intersections, []int{intersectionStart, intersectionEnd})
		}

		if firstList[firstPointer][1] < secondList[secondPointer][1] {
			firstPointer++
		} else {
			secondPointer++
		}
	}

	return intersections
}