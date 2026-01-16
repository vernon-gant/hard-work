package hashtable

import (
	// "strconv"
	"math"
	// "os"
)

type HashTable struct {
	size  int
	step  int
	slots []string
	occupied []bool
}

func Init(sz int, stp int) HashTable {
	ht := HashTable{size: sz, step: stp, slots: nil}
	ht.slots = make([]string, sz)
	ht.occupied = make([]bool, sz)
	return ht
}

func (ht *HashTable) HashFun(value string) int {
	result := 0

	for i := 0; i < len(value); i++ {
		result += (int(value[i]) * int(math.Pow(26, float64(len(value)-i)))) % len(ht.slots)
	}
	
	return result % len(ht.slots)
}

func (ht *HashTable) SeekSlot(value string) int {
	idx := ht.HashFun(value)
	startIdx := idx

	for ht.occupied[idx] {
		if ht.slots[idx] == value {
			return -1
		}

		idx = (idx + ht.step) % len(ht.slots)

		if idx == startIdx {
			return -1
		}
	}
	return idx
}

func (ht *HashTable) Put(value string) int {
	idx := ht.SeekSlot(value)

	if idx == -1 {
		return idx
	}

	ht.slots[idx] = value
	ht.occupied[idx] = true

	return idx
}

func (ht *HashTable) Find(value string) int {
	slowIdx := ht.HashFun(value)
	fastIdx := (slowIdx + ht.step) % len(ht.slots)

	for (!ht.occupied[slowIdx] || ht.slots[slowIdx] != value) && slowIdx != fastIdx {
		slowIdx = (slowIdx + ht.step) % len(ht.slots)
		fastIdx = ((fastIdx+ht.step)%len(ht.slots) + ht.step) % len(ht.slots)
	}

	if ht.occupied[slowIdx] && ht.slots[slowIdx] == value {
		return slowIdx
	}

	return -1
}
