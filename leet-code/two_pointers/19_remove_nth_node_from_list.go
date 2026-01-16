package two_pointers

type ListNode struct {
    Val  int
    Next *ListNode
}

func removeNthFromEnd(head *ListNode, n int) *ListNode {
    fast := head
    for i := 0; i < n+1; i++ {
        if fast == nil {
            return head.Next
        }
        fast = fast.Next
    }
    slow := head
    for fast != nil {
        slow = slow.Next
        fast = fast.Next
    }
    slow.Next = slow.Next.Next
    return head
}
