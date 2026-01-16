package queue

import (
	"errors"
	stack "github.com/vernon-gant/algos1-go/04_stack"
)

/*
* 5. Queue - task number 3 - rotate queue by N elements
*
* Very simple solution, we just dequeue and enque n times. More precisely n % q.size times, because there is no need to rotate the queue more than q.size - 1 times. That's why we take modulo.
* And I also added validation for negative | 0 values, because rotating 0 times makes no sense and negative rotation makes the interface more complicated in usage.
*/

func (q *Queue[T]) Rotate(n int) (error) {
	if n <= 0 {
		return errors.New("n must be >= 1")
	}
	if q.size == 0 {
		return nil
	}
	for trimmed := n % q.size; trimmed > 0; trimmed-- {
		first, _ := q.Dequeue()
		q.Enqueue(first)
	}
	return nil
}

/*
* 5. Queue - task number 4 - queue with 2 stacks
*
* I could implement it from the first time uiiii! For some reason I remembered the algorithm really well, that one stack will be responsible for storing enqueued items, another will be
* used for dequeuing from the queue. When it gets emptye, we move everything from the toEnqueue stack, this brings the queue order back and reverses all the items so that we get FIFO.
* Size we get from combining sizes of both stacks and enqueueing is just pushing to the toEnqueue stack. That's it.
*/

type Queue2[T any] struct {
	toDequeue stack.Stack[T]
	toEnqueue stack.Stack[T]
}

func (q *Queue2[T]) Size() int {
	return q.toDequeue.Size() + q.toEnqueue.Size()
}

func (q *Queue2[T]) Dequeue() (T, error) {
	var result T

	if q.toDequeue.Size() > 0 {
		return q.toDequeue.Pop()
	}

	for q.toEnqueue.Size() > 0 {
		popped, _ := q.toEnqueue.Pop()
		q.toDequeue.Push(popped)
	}

	if q.toDequeue.Size() == 0 {
		return result, errors.New("queue is empty")
	}

	result, _ = q.toDequeue.Pop()

	return result, nil
}

func (q *Queue2[T]) Enqueue(itm T) {
	q.toEnqueue.Push(itm)
}

/*
* 5. Queue - task number 5 - reverse queue
*
* There are 2 options here, either in my implementaiton I just reverse the linked list. But we already practiced this. Or we take a stack just for practicing it again and do it with O(N) space.
* The idea is the same as in the code above, we reverse the order using a stack and get the last element of the queue on top of the stack and then we keep popping till the end and enqueueing.
* That's it!
*/

func (q *Queue[T]) Reverse() {
	if q.size == 0 {
		return
	}
	st := stack.Stack[T]{}
	for q.Size() > 0 {
		first, _ := q.Dequeue()
		st.Push(first)
	}
	for st.Size() > 0 {
		top, _ := st.Pop()
		q.Enqueue(top)
	}
}

/*
* 5. Queue - task number 6 - circular queue
*
* As suggested we use the head index and count to derive the tail position = (head + count) % cap when enqueuing new items and dequeuing using just head. We do not need to set anything to 0,
* because we will just override it later in enqueue when needed. The whole algorithm works just by shifting pointers and overriding data in the buffer, that's it. God, save the modulo! It saves
* us from boundary crossing!
*/

type CircularQueue[T any] struct {
	buffer []T
	head   int
	count  int
	cap    int
}

func NewCircularQueue[T any](capacity int) *CircularQueue[T] {
	return &CircularQueue[T]{
		buffer: make([]T, capacity),
		head:   0,
		count:  0,
		cap:    capacity,
	}
}

func (q *CircularQueue[T]) Size() int {
	return q.count
}

func (q *CircularQueue[T]) IsFull() bool {
	return q.count == q.cap
}

func (q *CircularQueue[T]) Enqueue(item T) error {
	if q.IsFull() {
		return errors.New("queue is full")
	}
	tail := (q.head + q.count) % q.cap
	q.buffer[tail] = item
	q.count++
	return nil
}

func (q *CircularQueue[T]) Dequeue() (T, error) {
	var result T
	if q.count == 0 {
		return result, errors.New("queue is empty")
	}
	result = q.buffer[q.head]
	q.head = (q.head + 1) % q.cap
	q.count--
	return result, nil
}