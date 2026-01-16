package in_place_list_manipulation

func ReverseEvenLengthGroups(head *ListNode) *ListNode {
	reverseEvenHelper(head, head, 1)
	return head
}

func reverseEvenHelper(previous, currentPointer *ListNode, maxGroupSize int) {
	if currentPointer == nil {
		return
	}
	currentGroupSize, groupEnd := countAndReturnLast(currentPointer, 1, maxGroupSize)
	if currentGroupSize%2 != 0 {
		reverseEvenHelper(groupEnd, groupEnd.Next, maxGroupSize + 1)
		return 
	}
	reversedEvenGroup, nextGroupStart := reversePart(nil, currentPointer, 0, maxGroupSize)
	previous.Next = reversedEvenGroup
	currentPointer.Next = nextGroupStart
	reverseEvenHelper(currentPointer, nextGroupStart, maxGroupSize+1)
	return
}

func reversePart(previous, head *ListNode, alreadyReversed, toReverse int) (*ListNode, *ListNode) {
	if head == nil || alreadyReversed == toReverse {
		return previous, head
	}
	tail := head.Next
	head.Next = previous
	return reversePart(head, tail, alreadyReversed+1, toReverse)
}

func countAndReturnLast(head *ListNode, currentCounter, groupSize int) (int,*ListNode) {
	if head.Next == nil || currentCounter == groupSize {
		return currentCounter, head
	}
	return countAndReturnLast(head.Next, currentCounter + 1, groupSize)
}