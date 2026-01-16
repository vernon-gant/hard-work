package queue

import (
	"os"
	"errors"
)

type Node[T any] struct {
	value T
	next  *Node[T]
}

type Queue[T any] struct {
	head *Node[T]
	tail *Node[T]
	size int
}

func (q *Queue[T]) Size() int {
	return q.size
}

// O(1)
func (q *Queue[T]) Dequeue() (T, error) {
	var result T
	if q.head == nil {
		return result, errors.New("empty queue")
	}
	if q.head == q.tail {
		q.tail = nil
	}
	result = q.head.value
	q.head = q.head.next
	q.size--
	return result, nil
}

// O(1)
func (q *Queue[T]) Enqueue(itm T) {
	newNode := Node[T]{value: itm}
	if q.head == nil {
		q.head = &newNode
		q.tail = &newNode
	} else {
		q.tail.next = &newNode
		q.tail = q.tail.next
	}
	q.size++
}
