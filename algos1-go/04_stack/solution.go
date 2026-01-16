package stack

import (
	// "os"
	"errors"
)

type Node[T any] struct {
	next  *Node[T]
	value T
}

type Stack[T any] struct {
	head  *Node[T]
	count int
}

func (st *Stack[T]) Size() int {
	return st.count
}

func (st *Stack[T]) Peek() (T, error) {
	var result T
	if st.Size() == 0 {
		return result, errors.New("empty stack")
	}
	return st.head.value, nil
}

func (st *Stack[T]) Pop() (T, error) {
	var result T
	if st.Size() == 0 {
		return result, errors.New("empty stack")
	}
	result = st.head.value
	st.head = st.head.next
	st.count--
	return result, nil
}

func (st *Stack[T]) Push(itm T) {
	newTop := Node[T]{value: itm, next: st.head}
	st.head = &newTop
	st.count++
}
