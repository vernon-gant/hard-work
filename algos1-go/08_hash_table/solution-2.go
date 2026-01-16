package hashtable

import (
	"errors"
	"math/rand"
	"time"
)

/*
* 8. Hash Table - task number 3 + 4 - dynamic table + double hashing
*
* I decided to take a deeper look at the C# implementation of HashTable. I deliberately skipped some implementaiton details, although
* they seem to be quite important. I mean, adding multiple readers and prohibiting multiple editions is a good thing, but not
* when you practice algos. Then they also have the _version variable - interesting one. I did not know how enumeration works in C#,
* now I know it :) If someone modifies the collection during some iteration, this is the way we know about it, by comparing the
* version. Another important point which I skipped was prime selection of the new size. But i did not want to copy the whole code.
* As far as I understood, using prime size and coprime step visits all the indexes. And this is somehow related to generators from
* cyclic groups :) I just doubled the size here, but C# devs of course did it properly.
*
* But the rest seems fine! We have our slots aka buckets, they store the hash code with leading bit storing whether this slot
* already collided during inserion. This will help us during search, insertion and deletion. Then we store the key itself and
* whether it was deleted. OF course I could do it as in C# with nullalble 'object' key and store there either key, or null or
* reference to the slots indicating that element was deleted. But casting in go is horrible from the syntax point so yeah...
*
* The rest is implementation. I liked the idea about leading bit, that we ahve tombstones and also save space by tracking first
* deleted element. The two hasing approach with prime numbers will reduce clustering, dev thouroughly described why above the class.
* And reduced clustering will improve performance!
*/

const (
	CollisionBit = 0x80000000
	HashMask     = 0x7FFFFFFF
	LoadFactor   = 0.72
	InitialSize  = 17
)

type Hashable interface {
	comparable
	HashCode() int
}

type Slot[K Hashable] struct {
	key      K
	hashColl int
	deleted  bool
}

type DynamicHashSet[K Hashable] struct {
	slots []Slot[K]
	count int
	loadSize int
}

func NewDynamicHashSet[K Hashable]() *DynamicHashSet[K] {
	return &DynamicHashSet[K]{
		slots:  make([]Slot[K], InitialSize),
		count:    0,
		loadSize: int(LoadFactor * float64(InitialSize)),
	}
}

func (hs *DynamicHashSet[K]) Insert(key K) error {
	if hs.count >= hs.loadSize {
		hs.resize(len(hs.slots) * 2)
	}

	hash, seed, step := hs.hashKey(key)
	idx := seed
	firstDeletedSlot := -1

	for i := 0; i < len(hs.slots); i++ {
		currentSlot := &hs.slots[idx]

		if firstDeletedSlot == -1 && currentSlot.isTombstone() {
			firstDeletedSlot = idx
		}

		hasFreeSlotBehind := currentSlot.isEmpty() && firstDeletedSlot != -1
		if hasFreeSlotBehind {
			hs.insertAt(firstDeletedSlot, key, hash)
			return nil
		} else if currentSlot.isEmpty() {
			hs.insertAt(idx, key, hash)
			return nil
		}

		if currentSlot.matchesKey(hash, key) {
			return errors.New("duplicate key")
		}

		if !hasCollisionBit(currentSlot.hashColl) {
			hs.slots[idx].hashColl |= CollisionBit
		}

		idx = (idx + step) % len(hs.slots)
	}

	if firstDeletedSlot != -1 {
		hs.insertAt(firstDeletedSlot, key, hash)
		return nil
	}

	return errors.New("table full")
}

func (hs *DynamicHashSet[K]) Find(key K) bool {
	hash, seed, step := hs.hashKey(key)
	idx := seed

	for i := 0; i < len(hs.slots); i++ {
		e := &hs.slots[idx]

		if e.isEmpty() {
			return false
		}

		if e.matchesKey(hash, key) {
			return true
		}

		if !hasCollisionBit(e.hashColl) {
			return false
		}

		idx = (idx + step) % len(hs.slots)
	}

	return false
}

func (hs *DynamicHashSet[K]) Delete(key K) error {
	hash, seed, step := hs.hashKey(key)
	idx := seed

	for i := 0; i < len(hs.slots); i++ {
		e := &hs.slots[idx]

		if e.isEmpty() {
			return errors.New("no element found")
		}

		if e.matchesKey(hash, key) {
			hs.slots[idx].deleted = true
			hs.count--
			return nil
		}

		if !hasCollisionBit(e.hashColl) {
			return errors.New("no element found")
		}

		idx = (idx + step) % len(hs.slots)
	}

	return errors.New("no element found")
}

func (hs *DynamicHashSet[K]) Count() int {
	return hs.count
}

func (hs *DynamicHashSet[K]) hashKey(key K) (hash int, seed int, step int) {
	h := key.HashCode()
	hash = extractHash(h)
	seed = hash % len(hs.slots)
	step = 1 + (hash>>5)%(len(hs.slots)-1)
	return
}

func (hs *DynamicHashSet[K]) insertAt(idx int, key K, hash int) {
	hs.slots[idx].key = key
	hs.slots[idx].hashColl = hash
	hs.slots[idx].deleted = false
	hs.count++
}

func (hs *DynamicHashSet[K]) resize(newSize int) {
	oldEntries := hs.slots
	hs.slots = make([]Slot[K], newSize)
	hs.count = 0
	hs.loadSize = int(LoadFactor * float64(newSize))

	for i := 0; i < len(oldEntries); i++ {
		e := &oldEntries[i]
		shouldReinsert := !e.isEmpty() && !e.isTombstone()
		if shouldReinsert {
			hs.Insert(e.key)
		}
	}
}

func extractHash(hashColl int) int {
	return hashColl & HashMask
}

func hasCollisionBit(hashColl int) bool {
	return hashColl < 0
}

func (e *Slot[K]) isEmpty() bool {
	var zero K
	return e.key == zero && !e.deleted
}

func (e *Slot[K]) isTombstone() bool {
	return e.deleted
}

func (e *Slot[K]) matchesKey(hash int, key K) bool {
	return extractHash(e.hashColl) == hash && e.key == key && !e.deleted
}

/*
* 8. Hash Table - task number 5 - DoS protection with salt
*
* I generate salt dynamically in constructor using current time, though in real production you'd want crypto-secure random.
* The salt stays constant for table lifetime, so lookups still work, but external attacker can't predict where keys land because each table instance has different salt.
*
* Simple XOR for mixing has nice properties - reversible, fast, good bit diffusion. Real implementations
* like Go's runtime use more sophisticated mixing functions, but XOR demonstrates the concept well enough. Performance cost is
* basically zero (one XOR instruction), but security improvement is huge. Modern languages do this by default now.
*/

type DynamicHashSetSalt[K Hashable] struct {
	slots    []Slot[K]
	count    int
	loadSize int
	salt     uint32
}

func NewDynamicHashSetSalt[K Hashable]() *DynamicHashSetSalt[K] {
	return &DynamicHashSetSalt[K]{
		slots:    make([]Slot[K], InitialSize),
		count:    0,
		loadSize: int(LoadFactor * float64(InitialSize)),
		salt:     rand.New(rand.NewSource(time.Now().UnixNano())).Uint32(),
	}
}

func (hs *DynamicHashSetSalt[K]) hashKey(key K) (hash int, seed int, step int) {
	h := key.HashCode()
	h = h ^ int(hs.salt)
	hash = extractHash(h)
	seed = hash % len(hs.slots)
	step = 1 + (hash>>5)%(len(hs.slots)-1)
	return
}