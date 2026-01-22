package set

import "constraints"

/*
* 10. Set - task number 4 - cartesian product
* Space - O(n * m) because this is a cartesian product :)
* Time - O(n * m) because we go through each element of the primary set n times where n is number of elements in the given set
*
* P.S Actually, I messed up the reflection a bit, I thought I need to write it for each completed special task. I reread the rules again and see that I need to write reflection
* by comparing solution for problems with my approach. My bad! However, it is not that bad, because surprisingly 90+ of my solutions corresponded to the correct one 1 to 1.
* He who controls the spice controls the universe......
*/

type Tuple[T any] struct {
	first  T
	second T
}

// This will not compile, because ordered constraint is only defined for integers/strings. But in C# this would work :)
func (s *PowerSet[T]) CartesianProduct(set2 PowerSet[T]) PowerSet[Tuple[T]] {
	result := InitSize[Tuple[T]](s.count * set2.count)

	for i := 0; i < s.count; i++ {
		for j := 0; j < set2.count; j++ {
			result.Put(Tuple[T]{First: s.values[i], Second: set2.values[j]})
		}
	}

	return result
}

/*
* 10. Set - task number 5 - intersection of many sets
* Space - O(x) for x pointers if we do not count result itself
* Time - O(n1 + n2 + n3) because we go through each element of all sets in the worst case
*/

func IntersectMany[T constraints.Ordered](sets ...PowerSet[T]) PowerSet[T] {
	result := Init[T]()
	pointers := make([]int, len(sets))

	for {
		for i := 0; i < len(sets); i++ {
			if pointers[i] >= sets[i].count {
				return result
			}
		}

		maxVal := sets[0].values[pointers[0]]
		for i := 1; i < len(sets); i++ {
			if sets[i].values[pointers[i]] > maxVal {
				maxVal = sets[i].values[pointers[i]]
			}
		}

		allMatch := true
		for i := 0; i < len(sets); i++ {
			if sets[i].values[pointers[i]] < maxVal {
				pointers[i]++
				allMatch = false
			}
		}

		if allMatch {
			result.Put(maxVal)
			for i := 0; i < len(sets); i++ {
				pointers[i]++
			}
		}
	}
}

type BagEntry[T constraints.Ordered] struct {
	value T
	count int
}

type Bag[T constraints.Ordered] struct {
	entries []BagEntry[T]
	count   int
}

func NewBag[T constraints.Ordered]() Bag[T] {
	return Bag[T]{
		entries: make([]BagEntry[T], 0),
		count:   0,
	}
}

func (b *Bag[T]) Add(value T) {
	idx, found := b.getIdx(value)

	if found {
		b.entries[idx].count++
		return
	}

	newEntry := BagEntry[T]{value: value, count: 1}

	b.entries = append(b.entries, newEntry)
	copy(b.entries[idx+1:], b.entries[idx:b.count])
	b.entries[idx] = newEntry
	b.count++
}

func (b *Bag[T]) Remove(value T) bool {
	idx, found := b.getIdx(value)

	if !found {
		return false
	}

	b.entries[idx].count--

	if b.entries[idx].count == 0 {
		copy(b.entries[idx:], b.entries[idx+1:b.count])
		b.count--
	}

	return true
}

func (b *Bag[T]) GetFrequencies() []BagEntry[T] {
	result := make([]BagEntry[T], b.count)
	copy(result, b.entries[:b.count])
	return result
}

func (b *Bag[T]) getIdx(value T) (int, bool) {
	start, end := 0, b.count-1

	for start <= end {
		midIdx := (start + end) / 2

		if b.entries[midIdx].value == value {
			return midIdx, true
		}

		if b.entries[midIdx].value < value {
			start = midIdx + 1
		} else {
			end = midIdx - 1
		}
	}

	return start, false
}

/*
* For now I do not own that much spice... My solution for dynamic the hash table was correct, i tried to repeat what the guys did for C# library. Of course, they did it better, but I caught the idea :)
* Extension with load factor and then rehashing is base. But I was not prepared foy dynamic salt. I though that generating the salt in constructor IS the dynamic salt because we will get different hashes
* in different instances. But it makes more sense from the security point to add to on each add and a different salt to the given value so that we always get different hashes. BUT!
* This will actually break the O(1) lookup because we will only have the linear probing. Maybe I miss something... "В пятнадцать лет он уже научился молчать."
*/