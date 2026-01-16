package deque

import (
	"cmp"
	"errors"
)

/*
* 6. Deque - task number 4 - is palindrome
*
* Progress is when you write such small things almost without thinking :)
*/

func IsPalindrome(input string) bool {
	deq := Deque[rune]{}

	for _, letter := range input {
		deq.AddTail(letter)
	}

	for deq.Size() > 1 {
		first, _ := deq.RemoveFront()
		last, _ := deq.RemoveTail()
		if first != last {
			return false
		}
	}

	return true
}

/*
* 6. Deque - task number 5 - min for O(1)
*
* The MinStack approach does not work directly int he case of MinDeque, because we can both add and remove from both side, so just pushing min values to one side does not work. But we can
* keep the same min list but from both sides and deque itself will be used for this behavior! We have aka 2 stacks growing from the center to different sides. Then when removing from the front, we pop from the "left stack" - if we imagine
* them parallel to x axis. Same with the "right stack" when removing from the tail. The min element is the smallest element from peeking the front and tail, because we need to consider both
* sides. So we add the "PeekFront" and "PeekTail" for deque and then use 2 deques in the implementation to achieve this effect. Main deque will serve like the base class, on every method call
* we call the base method of the 'parent'. And then apply the logic to the additional minDeque.
*/

func (d *Deque[T]) PeekFront() (T, error) {
	var result T

	if d.head == nil {
		return result, errors.New("empty deque")
	}

	return d.head.value, nil
}

func (d *Deque[T]) PeekTail() (T, error) {
	var result T

	if d.head == nil {
		return result, errors.New("empty deque")
	}

	return d.tail.value, nil
}

type MinDeque[T cmp.Ordered] struct {
	main Deque[T]
	min  Deque[T]
}

func (d *MinDeque[T]) Size() int {
	return d.main.Size()
}

func (d *MinDeque[T]) AddFront(itm T) {
	d.main.AddFront(itm)

	if d.min.Size() == 0 {
		d.min.AddFront(itm)
	} else {
		currentMinFront, _ := d.min.PeekFront()
		newMin := min(itm, currentMinFront)
		d.min.AddFront(newMin)
	}
}

func (d *MinDeque[T]) AddTail(itm T) {
	d.main.AddTail(itm)

	if d.min.Size() == 0 {
		d.min.AddTail(itm)
	} else {
		currentMinTail, _ := d.min.PeekTail()
		newMin := min(itm, currentMinTail)
		d.min.AddTail(newMin)
	}
}

func (d *MinDeque[T]) RemoveFront() (T, error) {
	val, err := d.main.RemoveFront()

	if err == nil {
		d.min.RemoveFront()
	}

	return val, err
}

func (d *MinDeque[T]) RemoveTail() (T, error) {
	val, err := d.main.RemoveTail()

	if err == nil {
		d.min.RemoveTail()
	}

	return val, err
}

func (d *MinDeque[T]) Min() (T, error) {
	var result T

	if d.main.Size() == 0 {
		return result, errors.New("empty deque")
	}

	minFront, _ := d.min.PeekFront()
	minTail, _ := d.min.PeekTail()

	return min(minFront, minTail), nil
}

/*
* 6. Deque - task number 6 - deque with dynamic array with amortized o(1) for everything
*
* We can't implement the Deque with o(1) for add/remove on both sides just relying on the dynamic array, as stated in the text depending on the head choice, one operation pair will be O(n).
* But! Combining circular buffer approch with dynamic array can make it to achieve o(1). In this case we benefit from O(1) for both side add/remove operation in circular manner, and then
* when we are running out of space the array is expanded and again becomes "fixed size". Growth/shrink behavior is taken from the DynArray section. When growing or shrinking we adjust the
* elements "order" and put the head to index 0 and all other elements directly after it. Otherwise the modulo logic will break.
*/

const (
	MIN_CAPACITY     = 16
	GROW_FACTOR      = 2
	SHRINK_FACTOR    = 1.5
	SHRINK_THRESHOLD = 0.5
)

type DequeArray[T any] struct {
	buffer   []T
	head     int
	count    int
	capacity int
}

func (d *DequeArray[T]) Size() int {
	return d.count
}

func (d *DequeArray[T]) AddFront(itm T) {
	if d.buffer == nil {
		d.buffer = make([]T, MIN_CAPACITY)
		d.capacity = MIN_CAPACITY
	}

	if d.count == d.capacity {
		d.resize(d.capacity * GROW_FACTOR)
	}

	d.head = (d.head - 1 + d.capacity) % d.capacity
	d.buffer[d.head] = itm
	d.count++
}

func (d *DequeArray[T]) AddTail(itm T) {
	if d.buffer == nil {
		d.buffer = make([]T, MIN_CAPACITY)
		d.capacity = MIN_CAPACITY
	}

	if d.count == d.capacity {
		d.resize(d.capacity * GROW_FACTOR)
	}

	tailIndex := (d.head + d.count) % d.capacity
	d.buffer[tailIndex] = itm
	d.count++
}

func (d *DequeArray[T]) RemoveFront() (T, error) {
	var result T

	if d.count == 0 {
		return result, errors.New("empty deque")
	}

	result = d.buffer[d.head]
	d.head = (d.head + 1) % d.capacity
	d.count--
	d.shrinkIfNeeded()

	return result, nil
}

func (d *DequeArray[T]) RemoveTail() (T, error) {
	var result T

	if d.count == 0 {
		return result, errors.New("empty deque")
	}

	tailIndex := (d.head + d.count - 1 + d.capacity) % d.capacity
	result = d.buffer[tailIndex]
	d.count--
	d.shrinkIfNeeded()

	return result, nil
}

func (d *DequeArray[T]) resize(newCapacity int) {
	newBuffer := make([]T, newCapacity)

	for i := 0; i < d.count; i++ {
		oldIndex := (d.head + i) % d.capacity
		newBuffer[i] = d.buffer[oldIndex]
	}

	d.buffer = newBuffer
	d.capacity = newCapacity
	d.head = 0
}

func (d *DequeArray[T]) shrinkIfNeeded() {
	if float64(d.count)/float64(d.capacity) >= SHRINK_THRESHOLD {
		return
	}

	newCap := int(float64(d.capacity) / SHRINK_FACTOR)

	if newCap < MIN_CAPACITY {
		newCap = MIN_CAPACITY
	}

	if newCap >= d.capacity {
		return
	}

	d.resize(newCap)
}
