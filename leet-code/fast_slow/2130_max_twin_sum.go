package fast_slow

func PairSum(head *ListNode) int {
	slow := head

	for fast := head; fast != nil; fast = fast.Next.Next {
		slow = slow.Next;
	}

	var prev *ListNode = nil

	for slow != nil {
		var temp = slow.Next
		slow.Next = prev
		prev = slow
		slow = temp
	}

	result := 0

	for ;prev != nil; prev = prev.Next {
		result = max(result, head.Val + prev.Val);
		head = head.Next;
	}

	return result
}
