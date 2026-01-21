package set

import (
	"constraints"
	// "os"
	//"strconv"
)

const SET_SIZE = 20000

type PowerSet[T constraints.Ordered] struct {
	values   []T
	occupied []bool
	count int
}

// создание экземпляра множества
func Init[T constraints.Ordered]() PowerSet[T] {
	return PowerSet[T]{
		values: make([]T, SET_SIZE),
		occupied: make([]bool, SET_SIZE),
		count: 0,
	}
}

func (s *PowerSet[T]) Size() int {
	return s.count
}

func (s *PowerSet[T]) Put(value T) {
	if s.count == SET_SIZE {
		return
	}

	if s.count > 0 && value > s.values[s.count - 1] {
		s.occupied[s.count] = true
		s.values[s.count] = value
		s.count++
		return
	}

	insertIdx := 0

	for ; insertIdx < s.count && s.occupied[insertIdx]; insertIdx++ {
		if s.values[insertIdx] == value {
			return
		}
		if s.values[insertIdx] > value {
			break
		}
	}

	copy(s.values[insertIdx + 1:s.count + 1], s.values[insertIdx:s.count])
	s.values[insertIdx] = value
	s.count++
	s.occupied[s.count - 1] = true
}

func (s *PowerSet[T]) Get(value T) bool {
	return s.getIdx(value) >= 0
}

func (s *PowerSet[T]) getIdx(value T) int {
	start, end := 0, s.count - 1

	for start <= end {
		midIdx := (start + end) / 2

		if s.values[midIdx] == value {
			return midIdx
		}

		if s.values[midIdx] < value {
			start = midIdx + 1
		} else {
			end = midIdx - 1
		}
	}

	return -1
}

func (s*PowerSet[T]) Remove(value T) bool {
	valueIdx := s.getIdx(value)

	if valueIdx == -1 {
		return false
	}

	copy(s.occupied[valueIdx:s.count - 1], s.occupied[valueIdx + 1:s.count])
	s.occupied[s.count - 1] = false
	s.count--

	return false
}

func (*PowerSet[T]) Intersection(set2 PowerSet[T]) PowerSet[T] {
	// пересечение текущего множества и set2
	var result PowerSet[T]
	// ...
	return result
}

func (*PowerSet[T]) Union(set2 PowerSet[T]) PowerSet[T] {
	// объединение текущего множества и set2
	var result PowerSet[T]
	// ...
	return result
}

func (*PowerSet[T]) Difference(set2 PowerSet[T]) PowerSet[T] {
	// разница текущего множества и set2
	var result PowerSet[T]
	// ...
	return result
}

func (*PowerSet[T]) IsSubset(set2 PowerSet[T]) bool {
	// возвращает true, если set2 есть
	// подмножество текущего множества
	return false
}

func (*PowerSet[T]) Equals(set2 PowerSet[T]) bool {
	// возвращает true,
	// если set2 равно текущему множеству
	return false
}
