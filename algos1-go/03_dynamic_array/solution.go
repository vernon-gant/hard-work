package dynamic_array

import (
	// "os"
	"fmt"
)

const (
	MIN_CAPACITY = 16
	GROW_FACTOR   = 2
	SHRINK_FACTOR = 1.5
	SHRINK_THRESHOLD = 0.5
)

type DynArray[T any] struct {
	count    int
	capacity int
	array    []T
}

func (da *DynArray[T]) Init() {
	da.count = 0
	da.MakeArray(MIN_CAPACITY)
}

func (da *DynArray[T]) MakeArray(sz int) {
	if sz < MIN_CAPACITY {
		sz = MIN_CAPACITY
	}
	newArr := make([]T, sz)
	if da.array != nil && da.count > 0 {
		copy(newArr, da.array[:da.count])
	}
	da.array = newArr
	da.capacity = sz
}

func (da *DynArray[T]) Append(itm T) {
	if da.count == da.capacity {
		da.MakeArray(da.capacity * GROW_FACTOR)
	}
	da.array[da.count] = itm
	da.count++
}

func (da *DynArray[T]) GetItem(index int) (T, error) {
	var zero T
	if index < 0 || index >= da.count {
		return zero, fmt.Errorf("invalid index '%d'", index)
	}
	return da.array[index], nil
}

// Time : O(N) because when we need to resize and copy the whole array in the worst case
// Space : O(1) if we do not consider the new buffer during resize
func (da *DynArray[T]) Insert(itm T, index int) error {
	if index < 0 || index > da.count {
		return fmt.Errorf("invalid index '%d'", index)
	}
	if da.count == da.capacity {
		da.MakeArray(da.capacity * GROW_FACTOR)
	}
	for i := da.count; i > index; i-- {
		da.array[i] = da.array[i-1]
	}
	da.array[index] = itm
	da.count++
	return nil
}

// Time : O(n) for the same reason as in the insert, because we might need to shift the whole array
// Sapce : O(1)
func (da *DynArray[T]) Remove(index int) error {
	if index < 0 || index >= da.count {
		return fmt.Errorf("invalid index '%d'", index)
	}

	copy(da.array[index:], da.array[index+1:da.count])
	da.count--

	if float64(da.count) / float64(da.capacity) >= SHRINK_THRESHOLD {
		return nil
	}

	newCap := int(float64(da.capacity) / SHRINK_FACTOR)

	if newCap < MIN_CAPACITY {
		newCap = MIN_CAPACITY
	}

	if newCap >= da.capacity {
		return nil
	}

	da.MakeArray(newCap)
	return nil
}
