package linkedlist

import (
	"fmt"
)

type Node struct {
	next  *Node
	value int
}

type LinkedList struct {
	head  *Node
	tail  *Node
	count int
}

func (l *LinkedList) AddInTail(item Node) {
	if l.head == nil {
		l.head = &item
	} else {
		l.tail.next = &item
	}

	l.count++
	l.tail = &item
}

func (l *LinkedList) Count() int {
	return l.count
}

// error не nil, если узел не найден
func (l *LinkedList) Find(n int) (Node, error) {
	for temp := l.head; temp != nil; temp = temp.next {
		if temp.value == n {
			return *temp, nil
		}
	}

	return Node{value: -1, next: nil}, fmt.Errorf("node with value %d not found", n)
}

func (l *LinkedList) FindAll(n int) []Node {
	var nodes []Node

	for temp := l.head; temp != nil; temp = temp.next {
		if temp.value == n {
			nodes = append(nodes, *temp)
		}
	}

	return nodes
}

func (l *LinkedList) Delete(n int, all bool) {
	l.head = l.DeleteRec(l.head, n, 0, all)

	if l.head == nil {
		l.tail = nil
	}
}

func (l *LinkedList) DeleteRec(temp *Node, n, delCount int, all bool) *Node {
	if temp == nil {
		return nil
	}

	if temp.value == n {
		delCount++
	}

	temp.next = l.DeleteRec(temp.next, n, delCount, all)

	if temp.next == nil {
		l.tail = temp
	}

	// we delete the node either if the user passed delete all or if it is the first occurence
	// of course under the condition that node value matches the target value
	if temp.value == n && (all || delCount == 1) {
		l.count--
		return temp.next
	}

	return temp
}

func (l *LinkedList) Insert(after *Node, add Node) {
	if after == nil {
        l.InsertFirst(add)
        return
    }
	add.next = after.next
	after.next = &add
	l.count++
	if add.next == nil {
		l.tail = &add
	}
}

func (l *LinkedList) InsertFirst(first Node) {
	if l.head == nil {
        l.head = &first
        l.tail = &first
    } else {
        first.next = l.head
        l.head = &first
    }
    l.count++
}

func (l *LinkedList) Clean() {
	l.head = nil
	l.tail = nil
	l.count = 0
}
