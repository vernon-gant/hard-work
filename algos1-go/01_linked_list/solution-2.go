package linkedlist

import "fmt"

/*
* 1. Linked lists - task number 8
*
* Merge two linked lists with type int into a resulting one where each resulting node value
* is the sum of two input linked lists values respecting their order. But only if their lenghts
are the same.
*
* The time complexity of the solution is O(n) : count for both lists is O(1) in this implementation
* and AddTail is also O(1) - so the loop makes it to O(n). If we consider the result itself for space complexity
* then we get also O(n). But to achieve the result the complexity is O(1) due to constant two first and second
* list pointers. We do not have non nullable annotations in go - this sucks...
*/

func listsSum(first, second * LinkedList) (*LinkedList, error) {
	if first == nil || second == nil {
		return nil, fmt.Errorf("one or both lists are nil")
	}

	if first.Count() != second.Count() {
		return nil, fmt.Errorf("not equal length lists")
	}

	firstPointer, secondPointer := first.head, second.head
	result := &LinkedList{}

	for firstPointer != nil && secondPointer != nil {
		result.AddInTail(Node{value: firstPointer.value + secondPointer.value})
		firstPointer = firstPointer.next
		secondPointer = secondPointer.next
	}

	return result, nil
}