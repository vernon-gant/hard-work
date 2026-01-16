package sorting

type ListNode struct {
	Val  int
	Next *ListNode
}

func sortList(head *ListNode) *ListNode {
	if head == nil || head.Next == nil {
		return head
	}

	var slow, fast, temp *ListNode = head, head, nil

	for fast != nil && fast.Next != nil {
		temp = slow
		slow = slow.Next
		fast = fast.Next.Next
	}

	temp.Next = nil

	left := sortList(head)
	right := sortList(slow)

	return mergeLists(left, right)
}

func mergeLists(list1, list2 *ListNode) *ListNode {
	if list1 == nil {
		return list2
	}

	if list2 == nil {
		return list1
	}

	if list1.Val <= list2.Val {
		list1.Next = mergeLists(list1.Next, list2)
		return list1
	}

	list2.Next = mergeLists(list1, list2.Next)
	return list2
}
