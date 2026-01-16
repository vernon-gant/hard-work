package in_place_list_manipulation

func ReverseKGroup(head *ListNode, k int) *ListNode {
	reversed := &ListNode{}
	return reverseKGroupHelper(reversed, reversed, head, k)
}

func reverseKGroupHelper(reversed, reversedTail, head *ListNode, k int) *ListNode {
	nextGroupStart, currentGroupSize := findNextGroupStart(head, head, 1, k)
	if currentGroupSize == k {
		reversedTail.Next = reverseListHelper(nil, head)
	}
	if nextGroupStart == nil {
		return reversed.Next
	}
	head.Next = nextGroupStart
	return reverseKGroupHelper(reversed, head, nextGroupStart, k)
}

func findNextGroupStart(head, currentPointer *ListNode, currentSize, k int) (*ListNode, int) {
	if currentPointer == nil {
		return nil, -currentSize
	}
	if currentSize == k {
		nextGroupStart := currentPointer.Next
		currentPointer.Next = nil
		return nextGroupStart, k
	}
	return findNextGroupStart(head, currentPointer.Next, currentSize+1, k)
}

////////////////////////////////////////////////////////

func reverseKGroups(head *EduLinkedListNode, k int) *EduLinkedListNode {
	reversed := &EduLinkedListNode{}
	return reverseKGroupHelperEdu(reversed, reversed, head, k)
}

func reverseKGroupHelperEdu(reversed, reversedTail, head *EduLinkedListNode, k int) *EduLinkedListNode {
	nextGroupStart, currentGroupSize := findNextGroupStartEdu(head, head, 1, k)
	if currentGroupSize == k {
		reversedTail.next = reverseHelper(nil, head)
	}
	if nextGroupStart == nil {
		return reversed.next
	}
	head.next = nextGroupStart
	return reverseKGroupHelperEdu(reversed, head, nextGroupStart, k)
}

func findNextGroupStartEdu(head, currentPointer *EduLinkedListNode, currentSize, k int) (*EduLinkedListNode, int) {
	if currentPointer == nil {
		return nil, -currentSize
	}
	if currentSize == k {
		nextGroupStart := currentPointer.next
		currentPointer.next = nil
		return nextGroupStart, k
	}
	return findNextGroupStartEdu(head, currentPointer.next, currentSize+1, k)
}