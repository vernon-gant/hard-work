package doubly_linked_list

import (
	// "os"
	// "reflect"
	"fmt"
)

type Node struct {
	prev  *Node
	next  *Node
	value int
}

type LinkedList2 struct {
	head  *Node
	tail  *Node
	count int
}

func (l *LinkedList2) AddInTail(item Node) {
	if l.head == nil {
		l.head = &item
		l.head.next = nil
		l.head.prev = nil
	} else {
		l.tail.next = &item
		item.prev = l.tail
	}

	l.tail = &item
	l.tail.next = nil
	l.count++
}

func (l *LinkedList2) Count() int {
	return l.count
}

// error не nil, если узел не найден
func (l *LinkedList2) Find(n int) (Node, error) {
	for temp := l.head; temp != nil; temp = temp.next {
		if temp.value == n {
			return *temp, nil
		}
	}

	return Node{value: -1, next: nil}, fmt.Errorf("node with value %d not found", n)
}

func (l *LinkedList2) FindAll(n int) []Node {
	var nodes []Node

	for temp := l.head; temp != nil; temp = temp.next {
		if temp.value == n {
			nodes = append(nodes, *temp)
		}
	}

	return nodes
}

func (l *LinkedList2) Delete(n int, all bool) {
	l.head = l.DeleteRec(l.head, n, 0, all)

	if l.head == nil {
		l.tail = nil
	} else {
		l.head.prev = nil
	}
}

func (l *LinkedList2) DeleteRec(temp *Node, n, delCount int, all bool) *Node {
	if temp == nil {
		return nil
	}

	if temp.value == n {
		delCount++
	}

	temp.next = l.DeleteRec(temp.next, n, delCount, all)

	if temp.next == nil {
		l.tail = temp
	} else {
		temp.next.prev = temp
	}

	// we delete the node either if the user passed delete all or if it is the first occurence
	// of course under the condition that node value matches the target value
	if temp.value == n && (all || delCount == 1) {
		l.count--
		return temp.next
	}

	return temp
}

func (l *LinkedList2) Insert(after *Node, add Node) {
	if after == nil {
        l.InsertFirst(add)
        return
    }

	add.prev = after
	add.next = after.next

	if after.next != nil {
		after.next.prev = &add
	}

	after.next = &add
	l.count++

	if add.next == nil {
		l.tail = &add
	}
}

func (l *LinkedList2) InsertFirst(first Node) {
	if l.head == nil {
		first.prev = nil
		first.next = nil
		l.head = &first
		l.tail = &first
		l.count++
		return
	}

	first.prev = nil
	first.next = l.head
	l.head.prev = &first
	l.head = &first
	l.count++
}

func (l *LinkedList2) Clean() {
	l.head = nil
	l.tail = nil
	l.count = 0
}
