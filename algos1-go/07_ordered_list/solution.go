package ordered_list

import (
	"constraints"
	// "os"
	"errors"
)

type Node[T constraints.Ordered] struct {
	prev  *Node[T]
	next  *Node[T]
	value T
}

type OrderedList[T constraints.Ordered] struct {
	head       *Node[T]
	tail       *Node[T]
	count      int
	_ascending bool
}

func (l *OrderedList[T]) Count() int {
	return l.count
}

func (l *OrderedList[T]) Add(item T) {
	newNode := &Node[T]{value: item}
	l.count++

	if l.head == nil {
		l.head = newNode
		l.tail = newNode
		return
	}

	if l.isPivot(l.head, item, true) {
		newNode.next = l.head
		l.head.prev = newNode
		l.head = newNode
		return
	}

	if l.isPivot(l.tail, item,false) {
		newNode.prev = l.tail
		l.tail.next = newNode
		l.tail = newNode
		return
	}

	toInsert := l.head
	for ; !l.isPivot(toInsert, item, true); toInsert = toInsert.next {}

	newNode.next = toInsert
	newNode.prev = toInsert.prev
	toInsert.prev.next = newNode
	toInsert.prev = newNode
}

func (l *OrderedList[T]) isPivot(node *Node[T], item T, before bool) bool {
	if before {
		return (l._ascending && item <= node.value) || (!l._ascending && item >= node.value)
	}
	return (l._ascending && item >= node.value) || (!l._ascending && item <= node.value)
}

func (l *OrderedList[T]) Find(n T) (Node[T], error) {
	var result Node[T]

	if l.head == nil {
		return result, errors.New("empty list")
	}

	if !l.isPivot(l.head, n, false) || !l.isPivot(l.tail, n, true) {
		return result, errors.New("not found")
	}

	for temp := l.head; temp != nil; temp = temp.next {
		if temp.value == n {
			return *temp, nil
		}
		if l.isPivot(temp, n, true) {
			break
		}
	}

	return result, errors.New("not found")
}

func (l *OrderedList[T]) Delete(n T) {
	if l.head == nil || !l.isPivot(l.head, n, false) || !l.isPivot(l.tail, n, true) {
		return
	}

	l.count--

	if l.head == l.tail {
		l.Clear(l._ascending)
		return
	}

	if l.head.value == n {
		l.head = l.head.next
		l.head.prev = nil
		return
	}

	if l.tail.value == n {
		l.tail = l.tail.prev
		l.tail.next = nil
		return
	}

	toDelete := l.head
	for ; toDelete != nil && toDelete.value != n; toDelete = toDelete.next {}

	if toDelete == nil {
		l.count++
		return
	}

	toDelete.prev.next = toDelete.next
	toDelete.next.prev = toDelete.prev
}

func (l *OrderedList[T]) Clear(asc bool) {
	l.head = nil
	l.tail = nil
	l.count = 0
	l._ascending = asc
}

func (l * OrderedList[T]) FindPosition(value T) (int, bool) {
	if l.head == nil {
		return 0, false
	}

	position := 0
	current := l.head

	for current != nil {
		if current.value == value {
			return position, true
		}

		// Early exit if we've passed where value would be
		if l.isPivot(current, value, true) {
			return position, false
		}

		current = current.next
		position++
	}

	return position, false
}

func (l *OrderedList[T]) Compare(v1 T, v2 T) int {
	if v1 < v2 {
		return -1
	}
	if v1 > v2 {
		return +1
	}
	return 0
}
