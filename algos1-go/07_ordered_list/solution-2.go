package ordered_list

import (
	"constraints"
	"errors"
)

/*
* 7. Ordered list - task number 8 - remove duplicates
*
* The idea here is that we pick a node and find the node after it with different value and relink the pointers. While skipping
* nodes we also decrement the nodes counter. There is also an edge case when the next node with different value is null, in that
* case we do not relink the previous pointer to avoid null dereference.
 */
func (l *OrderedList[T]) RemoveDuplicates() {
	if l.head == nil || l.head == l.tail {
		return
	}

	for temp := l.head; temp != nil; {
		innerTemp := temp.next
		for ; innerTemp != nil && innerTemp.value == temp.value; innerTemp = innerTemp.next {
			l.count--
		}
		temp.next = innerTemp
		if innerTemp != nil {
			innerTemp.prev = temp
		}
		temp = innerTemp
	}
}

/*
* 7. Ordered list - task number 9 - merge
*
* Same idea as with the merging in normal linked list, except that we do not allow merging lists with different ordering
* and merging with itself. It neither makes sense, nor makes the method easier in usage - so reject. The rest is just
* to recursively form the merged list AND also add liking of the prev pointer after recursion unwinding. Finally we relink
* the tail and that's it.
*/

func (l *OrderedList[T]) Merge(toMerge *OrderedList[T]) error {
	if l._ascending != toMerge._ascending {
		return errors.New("invalid ascending flag for given list")
	}

	if l == toMerge {
		return errors.New("can not merge with itself")
	}

	if l.head == nil {
		*l = *toMerge
		return nil
	}

	l.head = l.mergeRec(l.head, toMerge.head)
	l.count += toMerge.count

	temp := l.head
	for ; temp.next != nil; temp = temp.next {}

	l.tail = temp

	return nil
}

func (l *OrderedList[T]) mergeRec(list1, list2 *Node[T]) *Node[T] {
	if list1 == nil {
		return list2
	}

	if list2 == nil {
		return list1
	}

	var toReturn, next *Node[T]

	if l.isPivot(list1, list2.value, false) {
		next = l.mergeRec(list1.next, list2)
		list1.next = next
		toReturn = list1
	} else {
		next = l.mergeRec(list1, list2.next)
		list2.next = next
		toReturn = list2
	}

	next.prev = toReturn

	return toReturn
}


/*
* 7. Ordered list - task number 10 - containts sublist
*
* Here we also add some preconditions and explicitly handle edge cases. Main algorithm is pretty easy - find start of the sublist
* in the main list, if not found -> false. Then we keep iterating both lists. If after iteration the sublist pointer is null
* -> all nodes were traversed and we return true. In all other cases - false.
*/

func (l *OrderedList[T]) ContainsSublist(subList *OrderedList[T]) (bool, error) {
	if l.head == nil {
		return false, errors.New("empty list")
	}

	if l._ascending != subList._ascending {
		return false, errors.New("invalid ascending flag for given list")
	}

	if subList.head == nil {
		return true, nil
	}

	if !l.isPivot(l.head, subList.tail.value, false) || !l.isPivot(l.tail, subList.head.value, true) {
		return false, nil
	}

	start := l.head
	for ; start != nil && start.value != subList.head.value; start = start.next {}

	if start == nil {
		return false, nil
	}

	subPointer := subList.head
	for ; subPointer != nil && start != nil; subPointer = subPointer.next {
		if subPointer.value != start.value {
			return false, nil
		}
		start = start.next
	}

	return subPointer == nil, nil
}

/*
* 7. Ordered list - task number 11 - most frequent
*
* Simple sliding window variation with one interesting edge case when we reach the nil after tail. In that case we do not get access
* to the element of the window, we only know its size. But we know the tail -> this solves the problem! So after iterating over each window
* we just set the end of the window to tail if needed and temp list pointer to nil(to break the main loop) and continue normally
* our baseline algorithm.
*/

func (l *OrderedList[T]) MostFrequent() (T, error) {
	var result T

	if l.head == nil {
		return result, errors.New("empty list")
	}

	maxWindow := 1
	result = l.head.value

	for temp := l.head; temp != nil; {
		window, end := 1, temp.next
		for ; end != nil && temp.value == end.value; end = end.next {
			window++
		}
		temp = end
		if end == nil {
			end = l.tail
			temp = nil
		}
		if window > maxWindow {
			result = end.prev.value
			maxWindow = window
		}
	}

	return result, nil
}

/*
* 7. Ordered list - task number 12 - O(log(n)) search
*
* Because I repeat the course, I can use the BST here :) The only problem to solve was the detection of indices, but simply
* adding the size of the subtree fo each node solves the problem. The leftSize for example tells us how many elements come before
* current node and give the idx of current node. And we can use it to : define if we need to search in the left subtree
* or we can skip it(subtract its size from the idx and node itself) and search in the right tree, OR we found the node!
* Using recursion can be solved very elegantly!
*/

type BSTNode[T constraints.Ordered] struct {
	value T
	left  *BSTNode[T]
	right *BSTNode[T]
	size  int
}

type OrderedListBST[T constraints.Ordered] struct {
	root      *BSTNode[T]
	ascending bool
}

func (l *OrderedListBST[T]) GetByIndex(index int) (T, error) {
	var zero T

	if l.root == nil || index < 0 || index >= l.root.size {
		return zero, errors.New("index out of range")
	}

	return l.getByIndexRec(l.root, index), nil
}

func (l *OrderedListBST[T]) getByIndexRec(node *BSTNode[T], index int) T {
	leftSize := 0

	if node.left != nil {
		leftSize = node.left.size
	}

	if index == leftSize {
		return node.value
	}

	if index < leftSize {
		return l.getByIndexRec(node.left, index)
	}

	return l.getByIndexRec(node.right, index - leftSize - 1)
}