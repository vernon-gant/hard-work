package recursion

/**
 * Definition for singly-linked list.
 * type ListNode struct {
 *     Val int
 *     Next *ListNode
 * }
 */
/**
 * Definition for singly-linked list.
 * type ListNode struct {
 *     Val int
 *     Next *ListNode
 * }
 */

func AddTwoNumbers(l1 *ListNode, l2 *ListNode) *ListNode {
    return addTwoNumbersRec(l1, l2, 0)
}

func addTwoNumbersRec(l1, l2 *ListNode, overflow int) *ListNode {
    if l1 == nil && l2 == nil && overflow == 0 {
        return nil
    }

    firstVal, secondVal := 0, 0
    var firstNext, secondNext, toReturn *ListNode

    if l1 != nil {
        firstVal = l1.Val
        firstNext = l1.Next
        toReturn = l1
    }

    if l2 != nil {
        secondVal = l2.Val
        secondNext = l2.Next
        toReturn = l2
    }

    if toReturn == nil {
        toReturn = &ListNode{}
    }

    sum := firstVal + secondVal + overflow
    toReturn.Val = sum % 10
    toReturn.Next = addTwoNumbersRec(firstNext, secondNext, sum / 10)
    return toReturn
}

func addTwoNumbersRec2(l1, l2 *ListNode, overflow int) *ListNode {
    if l1 == nil && l2 == nil && overflow == 0 {
        return nil
    }

    firstVal, secondVal := 0, 0
    var firstNext, secondNext *ListNode

    if l1 != nil {
        firstVal = l1.Val
        firstNext = l1.Next
    }

    if l2 != nil {
        secondVal = l2.Val
        secondNext = l2.Next
    }

    sum := firstVal + secondVal + overflow
    return &ListNode{
        Val: sum % 10,
        Next: addTwoNumbersRec2(firstNext, secondNext, sum / 10),
    }
}