package in_place_list_manipulation

func SwapPairs(head *ListNode) *ListNode {
	if head == nil || head.Next == nil {
		return head
	}
	dummy := &ListNode{Next: head}
	swapPairsHelper(dummy,head,head.Next,head.Next.Next)
	return dummy.Next
}

func swapPairsHelper(prev, left, right, next *ListNode) {
	right.Next = left
	prev.Next = right
	left.Next = next
	if next == nil || next.Next == nil {
		return
	}
	swapPairsHelper(left,next, next.Next, next.Next.Next)
}