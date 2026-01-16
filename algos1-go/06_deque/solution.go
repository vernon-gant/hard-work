package main

import (
  "os"
  "errors"
)

type Node[T any] struct {
  value T
  next  *Node[T]
  prev  *Node[T]
}

type Deque[T any] struct {
  head *Node[T]
  tail *Node[T]
  size int
}

func (d *Deque[T]) Size() int {
  return d.size
}

func (d *Deque[T]) AddFront(itm T) {
  newNode := &Node[T]{value: itm}

  if d.head == nil {
    d.head = newNode
    d.tail = newNode
  } else {
    newNode.next = d.head
    d.head.prev = newNode
    d.head = newNode
  }

  d.size++
}

func (d *Deque[T]) AddTail(itm T) {
  newNode := &Node[T]{value: itm}

  if d.tail == nil {
    d.head = newNode
    d.tail = newNode
  } else {
    newNode.prev = d.tail
    d.tail.next = newNode
    d.tail = newNode
  }

  d.size++
}

func (d *Deque[T]) RemoveFront() (T, error) {
  var result T

  if d.head == nil {
    return result, errors.New("empty deque")
  }

  result = d.head.value
  d.head = d.head.next

  if d.head == nil {
    d.tail = nil
  } else {
    d.head.prev = nil
  }

  d.size--
  return result, nil
}

func (d *Deque[T]) RemoveTail() (T, error) {
  var result T

  if d.tail == nil {
    return result, errors.New("empty deque")
  }

  result = d.tail.value
  d.tail = d.tail.prev

  if d.tail == nil {
    d.head = nil
  } else {
    d.tail.next = nil
  }

  d.size--
  return result, nil
}
