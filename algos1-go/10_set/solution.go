package set

import (
	"constraints"
	// "os"
	//"strconv"
)

const SET_SIZE = 20000

type PowerSet[T constraints.Ordered] struct {
	values   []T
	count int
}

// создание экземпляра множества
func Init[T constraints.Ordered]() PowerSet[T] {
	return InitSize[T](SET_SIZE)
}

func InitSize[T constraints.Ordered](size int) PowerSet[T] {
	return PowerSet[T]{
		values: make([]T, size),
		count: 0,
	}
}

func (s *PowerSet[T]) Size() int {
	return s.count
}

func (s *PowerSet[T]) Put(value T) {
	if s.count == len(s.values) {
		return
	}

	if s.count > 0 && value > s.values[s.count - 1] {
		s.values[s.count] = value
		s.count++
		return
	}

	insertIdx, found := s.getIdx(value)

	if found {
		return
	}

	copy(s.values[insertIdx + 1:s.count + 1], s.values[insertIdx:s.count])
	s.values[insertIdx] = value
	s.count++
}

func (s *PowerSet[T]) Get(value T) bool {
	_, found := s.getIdx(value)
	return found
}

func (s *PowerSet[T]) getIdx(value T) (int, bool) {
	start, end := 0, s.count - 1

	for start <= end {
		midIdx := (start + end) / 2

		if s.values[midIdx] == value {
			return midIdx, true
		}

		if s.values[midIdx] < value {
			start = midIdx + 1
		} else {
			end = midIdx - 1
		}
	}

	return start, false
}

func (s *PowerSet[T]) Remove(value T) bool {
	valueIdx, found := s.getIdx(value)

	if !found {
		return false
	}

	copy(s.values[valueIdx:], s.values[valueIdx+1:s.count])
	s.count--
	
	return true
}

func (s * PowerSet[T]) Intersection(set2 PowerSet[T]) PowerSet[T] {
	result := Init[T]()

	primaryPointer, newPointer := 0, 0

	for primaryPointer < s.count && newPointer < set2.count {
		primaryValue, newValue := s.values[primaryPointer], set2.values[newPointer]

		if primaryValue == newValue {
			result.Put(primaryValue)
			primaryPointer++
			newPointer++
		} else if primaryValue < newValue {
			primaryPointer++
		} else {
			newPointer++
		}
	}

	return result
}

func (s*PowerSet[T]) Union(set2 PowerSet[T]) PowerSet[T] {
	result := InitSize[T](s.count + set2.count)
	primaryPointer, newPointer := 0, 0

	for primaryPointer < s.count && newPointer < set2.count {
		primaryValue, newValue := s.values[primaryPointer], set2.values[newPointer]
		if primaryValue == newValue {
			result.Put(primaryValue)
			primaryPointer++
			newPointer++
		} else if primaryValue < newValue {
			result.Put(primaryValue)
			primaryPointer++
		} else {
			result.Put(newValue)
			newPointer++
		}
	}

	for primaryPointer < s.count {
		result.Put(s.values[primaryPointer])
		primaryPointer++
	}

	for newPointer < set2.count {
		result.Put(set2.values[newPointer])
		newPointer++
	}

	return result
}

func (s*PowerSet[T]) Difference(set2 PowerSet[T]) PowerSet[T] {
	result := Init[T]()
	primaryPointer, newPointer := 0, 0

	for primaryPointer < s.count {
		if newPointer >= set2.count {
			result.Put(s.values[primaryPointer])
			primaryPointer++
			continue
		}

		primaryValue, newValue := s.values[primaryPointer], set2.values[newPointer]

		if primaryValue == newValue {
			primaryPointer++
			newPointer++
		} else if primaryValue < newValue {
			result.Put(primaryValue)
			primaryPointer++
		} else {
			newPointer++
		}
	}

	return result
}

func (s*PowerSet[T]) IsSubset(set2 PowerSet[T]) bool {
	if set2.count > s.count {
		return false
	}

	primaryPointer, subsetPointer := 0, 0

	for subsetPointer < set2.count && primaryPointer < s.count {
		primaryValue, subsetValue := s.values[primaryPointer], set2.values[subsetPointer]

		if primaryValue == subsetValue {
			subsetPointer++
			primaryPointer++
		} else if primaryValue < subsetValue {
			primaryPointer++
		} else {
			return false
		}
	}

	return subsetPointer == set2.count
}

func (s*PowerSet[T]) Equals(set2 PowerSet[T]) bool {
	if s.count != set2.count {
		return false
	}

	for i := 0; i < s.count; i++ {
		if s.values[i] != set2.values[i] {
			return false
		}
	}

	return true
}
