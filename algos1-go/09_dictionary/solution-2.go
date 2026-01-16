package dictionary

import (
	"constraints"
	"errors"
	"github.com/vernon-gant/algos1-go/07_ordered_list"
)

/*
* 9. Dictionary - task number 5 - dictionary using Ordered List
*
* I decided to take existing implementation of the OrderedList(more precisely its interface aka spec) and use it as internal field
* and not to reimplement the list from scratch. We take the interface, although in the case of go the concrete struct from previous
* lesson - but this does not matter, because concrete implementations may differ in efficincy. Our previous implementation was O(n)
* for search and insertion, but using an array for this would result in O(log(n)) for search. Specification could also (probably) mention
* that these operations must result in O(log(n)). This could make sense.
*
* In our case we typically want from the spec level that an Ordered List has search in O(n) or even O(log(n)) and maybe also insertion.
* Both log(n) would be an implementation using a skip list, the second one would be just dynamic array. But the interface is still the same!
* So we just take the OrderedList and use its operations. For that I added another method FindPosition which could be implemented
* in any data structure aka skip list, doubly linked list, dyn array. It returns bool and int where bool indicates that element was found
* or not and int its position which the element needs to be inserted to or is at. Because we need to shift all the value elements, the Put
* complexity is O(n) - copy does not do that in O(1). Same applies to Delete. Seach, however, depends on the implementation of the
* FindPosition.
 */

type OrderedDict[K constraints.Ordered, V any] struct {
	keys   *ordered_list.OrderedList[K]
	values []V
}

func NewOrderedDict[K constraints.Ordered, V any]() *OrderedDict[K, V] {
	keys := &ordered_list.OrderedList[K]{}
	keys.Clear(true)
	return &OrderedDict[K, V]{
		keys:   keys,
		values: make([]V, 0),
	}
}

func (d *OrderedDict[K, V]) Put(key K, value V) {
	position, found := d.keys.FindPosition(key)

	if found {
		d.values[position] = value
		return
	}

	d.keys.Add(key)

	var zero V

	d.values = append(d.values, zero)
	copy(d.values[position+1:], d.values[position:])
	d.values[position] = value
}

func (d *OrderedDict[K, V]) Delete(key K) error {
	position, found := d.keys.FindPosition(key)

	if !found {
		return errors.New("key not found")
	}

	d.keys.Delete(key)

	copy(d.values[position:], d.values[position+1:])
	d.values = d.values[:len(d.values)-1]

	return nil
}

func (d *OrderedDict[K, V]) IsKey(key K) bool {
	_, found := d.keys.FindPosition(key)
	return found
}

/*
* 9. Dictionary - task number 6 - dictionary for fixed length bit strings
*
* The advantage we get in this case is that under assumption that we have enough memory and we are ready to sacrifice it to get
* look up time benefit is that we can convert the string version of the bit into an integer and as mentioned in the theory
* just put it on its place in the preallocated array. But of course, of the bit length we set in constructor is 32, then allocated
* array will be ~2bg. If we are ready to pay this price, then this is the way to go. What I wanted to try out in this task
* is how to remove this if check everywhere and the solution turned out to be a monad! :) Only for 2 methods, because go does not
* have method type parameters and type assertions in this case would look ugly.
*/

type BitStringDict[V any] struct {
	bitLength int
	values    []*V
}

func NewBitStringDict[V any](bitLength int) *BitStringDict[V] {
	size := 1 << bitLength
	return &BitStringDict[V]{
		bitLength: bitLength,
		values:    make([]*V, size),
	}
}

func (d *BitStringDict[V]) bitStringToInt(key string) (int, error) {
	if len(key) != d.bitLength {
		return -1, errors.New("invalid bit string length")
	}

	result := 0
	for i := 0; i < len(key); i++ {
		result <<= 1

		if key[i] == '1' {
			result |= 1
		} else if key[i] != '0' {
			return -1, errors.New("invalid bit string: only 0 and 1 allowed")
		}
	}

	return result, nil
}

func (d *BitStringDict[V]) incorrectBitOrContinue(key string, cont func(int) error) error {
	idx, err := d.bitStringToInt(key)

	if err != nil {
		return err
	}

	return cont(idx)
}

func (d *BitStringDict[V]) Put(key string, value V) error {
	return d.incorrectBitOrContinue(key, func(idx int) error {
		d.values[idx] = &value
		return nil
	})
}

func (d *BitStringDict[V]) Delete(key string) error {
	return d.incorrectBitOrContinue(key, func(idx int) error {
		if d.values[idx] == nil {
			return errors.New("key not found")
		}

		d.values[idx] = nil
		return nil
	})
}

func (d *BitStringDict[V]) IsKey(key string) (bool, error) {
	idx, err := d.bitStringToInt(key)

	if err != nil {
		return false, err
	}

	return d.values[idx] != nil, nil
}
