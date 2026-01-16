package recursion

type ListNode struct {
    Val int
    Next *ListNode
}

// I like this solution, although it is O(N) space it does not require any list reversal and is just 9 lines. Non rec version does not allocate anything on stack, but is ugly...
func removeNodes(head *ListNode) *ListNode {
    if head.Next == nil {
        return head
    }

    next := removeNodes(head.Next)

    if head.Val < next.Val {
        return next
    }

    head.Next = next

    return head
}

func RemoveNodesNonRec(head * ListNode) *ListNode {
    tail := reverse(head)
    second := tail.Next
    tail.Next = nil

    for temp := second; temp != nil; {
        if temp.Val < tail.Val {
            temp = temp.Next
            continue
        }

        next := temp.Next
        temp.Next = tail
        tail = temp
        temp = next
    }

    return tail
}

func reverse(head *ListNode) *ListNode {
    var tail *ListNode
    temp := head

    for temp != nil {
        next := temp.Next
        temp.Next = tail
        tail = temp
        temp = next
    }

    return tail
}