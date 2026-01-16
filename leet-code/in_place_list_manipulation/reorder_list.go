package in_place_list_manipulation

func ReorderList(head *ListNode) {
	insert(head, reverseListReorder(nil, getElementAfterMiddle(head, head)))
}

func insert(head, toInsert *ListNode) {
	if toInsert == nil {
		return
	}
	newInsertHead := toInsert.Next
	toInsert.Next = head.Next
	head.Next = toInsert
	insert(toInsert.Next, newInsertHead)
}

func getElementAfterMiddle(slow, fast *ListNode) *ListNode {
	if fast != nil && fast.Next != nil {
		return getElementAfterMiddle(slow.Next, fast.Next.Next)
	}
	elementAfterMiddle := slow.Next
	slow.Next = nil
	return elementAfterMiddle
}

func reverseListReorder(reversed, head *ListNode) *ListNode {
	if head == nil {
		return reversed
	}
	tail := head.Next
	head.Next = reversed
	return reverseListReorder(head, tail)
}

////////////////////////////////////////////////////////////////////

func reorderList(head *EduLinkedListNode) {
	insertEdu(head, reverseListReorderEdu(nil, getElementAfterMiddleEdu(head, head)))
}

func insertEdu(head, toInsert *EduLinkedListNode) {
	if toInsert == nil {
		return
	}
	newInsertHead := toInsert.next
	toInsert.next = head.next
	head.next = toInsert
	insertEdu(toInsert.next, newInsertHead)
}

func getElementAfterMiddleEdu(slow, fast *EduLinkedListNode) *EduLinkedListNode {
	if fast == nil || fast.next == nil {
		elementAfterMiddle := slow.next
		slow.next = nil
		return elementAfterMiddle
	}
	return getElementAfterMiddleEdu(slow.next, fast.next.next)
}

func reverseListReorderEdu(reversed, head *EduLinkedListNode) *EduLinkedListNode {
	if head == nil {
		return reversed
	}
	tail := head.next
	head.next = reversed
	return reverseListReorderEdu(head, tail)
}
