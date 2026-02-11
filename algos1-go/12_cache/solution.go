package cache

import (
	"errors"
)

// Still using non nullable string array => prohibit empty strings as keys and treat them as missing values in the slice
// The rest is just normal LFU cache.
type NativeCache[T any] struct {
	size   int
	slots  []string
	values []T
	hits   []int
}

func Init[T any](sz int) NativeCache[T] {
	return NativeCache[T]{
		size:   sz,
		slots:  make([]string, sz),
		values: make([]T, sz),
		hits:   make([]int, sz),
	}
}

func (c *NativeCache[T]) hash(key string) int {
	result := 0

	for i := 0; i < len(key); i++ {
		pow := 1

		for j := 0; j < len(key)-i; j++ {
			pow *= 26
		}

		result += (int(key[i]) * pow) % c.size
	}

	return result % c.size
}

func (c *NativeCache[T]) findSlot(key string) int {
	startIdx := c.hash(key)
	idx := startIdx

	for c.slots[idx] != "" && c.slots[idx] != key {
		idx = (idx + 1) % c.size

		if idx == startIdx {
			return -1
		}
	}

	return idx
}

func (c *NativeCache[T]) Get(key string) (T, error) {
	idx := c.findSlot(key)
	var empty T

	if idx == -1 || c.slots[idx] != key {
		return empty, errors.New("key not found")
	}

	c.hits[idx]++
	return c.values[idx], nil
}

func (c *NativeCache[T]) Put(key string, value T) {
	if key == "" {
		return
	}

	idx := c.findSlot(key)

	if idx == -1 {
		c.evict()
		idx = c.findSlot(key)
	}

	c.slots[idx] = key
	c.values[idx] = value
	c.hits[idx] = 0
}

func (c *NativeCache[T]) evict() {
	minIdx := 0

	for i := 1; i < c.size; i++ {
		if c.hits[i] < c.hits[minIdx] {
			minIdx = i
		}
	}

	c.slots[minIdx] = ""
	c.hits[minIdx] = 0

	var empty T
	c.values[minIdx] = empty
}