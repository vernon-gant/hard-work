package dynamic_array

import "fmt"

/*
* 1. Dynamic Array - task number 6 - dynamic array using a bank method
*
* As proposed in the course we keep track on our deposit where each cheap operation like insertion or simple element copying deposits a fixed cost into the bank. When the accumulated sum
* becomes enough to pay for the next reallocation || we run out of space => reallocate && deduct the price. For the price of current reallocation we take the power of 2 as stated.
*/

type DynArrayBank[T any] struct {
	count    int
	capacity int
	bank     int
	array    []T
}

const (
	INSERT_DEPOSIT = 3
	COPY_DEPOSIT   = 3
)

func (da *DynArrayBank[T]) Init() {
	da.count = 0
	da.bank = 0
	da.MakeArray(MIN_CAPACITY)
}

func (da *DynArrayBank[T]) MakeArray(sz int) {
	if sz < MIN_CAPACITY {
		sz = MIN_CAPACITY
	}
	newArr := make([]T, sz)
	copy(newArr, da.array[:da.count])
	da.array = newArr
	da.capacity = sz
}

func (da *DynArrayBank[T]) GetItem(index int) (T, error) {
	var zero T

	if index < 0 || index >= da.count {
		return zero, fmt.Errorf("invalid index '%d'", index)
	}

	return da.array[index], nil
}

func (da *DynArrayBank[T]) Append(itm T) {
	da.bank += INSERT_DEPOSIT
	da.tryReallocation()
	da.array[da.count] = itm
	da.count++
}

func (da *DynArrayBank[T]) Insert(itm T, index int) error {
	if index < 0 || index > da.count {
		return fmt.Errorf("invalid index '%d'", index)
	}

	da.bank += INSERT_DEPOSIT
	da.tryReallocation()

	for i := da.count; i > index; i-- {
		da.array[i] = da.array[i-1]
		da.bank += COPY_DEPOSIT
	}

	da.array[index] = itm
	da.count++
	return nil
}

func (da *DynArrayBank[T]) Remove(index int) error {
	if index < 0 || index >= da.count {
		return fmt.Errorf("invalid index '%d'", index)
	}

	for i := index; i < da.count-1; i++ {
		da.array[i] = da.array[i+1]
		da.bank += COPY_DEPOSIT
	}

	da.count--

	if da.count*2 < da.capacity && da.capacity > MIN_CAPACITY {
		da.shrink()
	}

	return nil
}

func (da *DynArrayBank[T]) tryReallocation() {
	nextCap := da.capacity * GROW_FACTOR
	price := da.relocationCost(nextCap)

	if da.count < da.capacity && da.bank < price {
		return
	}

	da.MakeArray(nextCap)

	if da.bank >= price {
		da.bank -= price
	}
}

func (da *DynArrayBank[T]) shrink() {
	newCap := int(float64(da.capacity) / SHRINK_FACTOR)

	if newCap < MIN_CAPACITY {
		newCap = MIN_CAPACITY
	}

	price := da.relocationCost(newCap)

	if da.bank < price {
		return
	}

	da.bank -= price
	da.MakeArray(newCap)
}

func (da *DynArrayBank[T]) relocationCost(newSize int) int {
	cost := 1

	for cost*2 <= newSize {
		cost *= 2
	}

	return cost
}