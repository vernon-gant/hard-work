package in_place_list_manipulation


func getIntersectionNode(headA, headB *ListNode) *ListNode {
    listALen, listBLen := getListLen(headA), getListLen(headB)
    for i := 0; i < listALen - min(listALen, listBLen); i++ {
        headA = headA.Next
    }
    for i := 0; i < listBLen - min(listALen, listBLen); i++ {
        headB = headB.Next
    }
    for headA != nil && headA != headB {
        headA = headA.Next
        headB = headB.Next
    }
    return headA
}

func getListLen(head * ListNode) int {
    counter := 0
    for head != nil {
        counter++
		head = head.Next
    }
    return counter
}