package in_place_list_manipulation

type EduLinkedListNode struct {
	data int
	next *EduLinkedListNode
}

func reverse(head *EduLinkedListNode) *EduLinkedListNode {
	return reverseHelper(nil, head)
}

func reverseHelper(reversed, currentHead *EduLinkedListNode) *EduLinkedListNode {
	if currentHead == nil {
		return reversed
	}
	tail := currentHead.next
	currentHead.next = reversed
	return reverseHelper(currentHead, tail)
}

type ListNode struct {
	Val  int
	Next *ListNode
}

func reverseList(head *ListNode) *ListNode {
	return reverseListHelper(nil, head)
}

func reverseListHelper(reversed, head *ListNode) *ListNode {
	if head == nil {
		return reversed
	}
	tail := head.Next
	head.Next = reversed
	return reverseListHelper(head, tail)
}