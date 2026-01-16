package recursion

func mergeTwoLists(list1 *ListNode, list2 *ListNode) *ListNode {
    if list1 == nil {
        return list2
    }

    if list2 == nil {
        return list1
    }

    var head, tail *ListNode

    if list1.Val < list2.Val {
        head = list1
        tail = mergeTwoLists(list1.Next, list2)
    } else {
        head = list2
        tail = mergeTwoLists(list1, list2.Next)
    }

    head.Next = tail
    return head
}