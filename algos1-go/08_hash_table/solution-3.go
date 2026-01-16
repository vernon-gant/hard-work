package hashtable

import (
	"github.com/stretchr/testify/assert"
	"testing"
)

// HASH FUN

func Test_GivenString_WhenHashingTwice_ThenReturnsSameIndex(t *testing.T) {
	// Given
	ht := Init(17, 3)

	// When
	idx1 := ht.HashFun("test")
	idx2 := ht.HashFun("test")

	// Then
	assert.Equal(t, idx1, idx2)
}

func Test_GivenString_WhenHashing_ThenReturnsValidIndex(t *testing.T) {
	// Given
	ht := Init(17, 3)

	// When
	idx := ht.HashFun("hello")

	// Then
	assert.GreaterOrEqual(t, idx, 0)
	assert.Less(t, idx, 17)
}

// SEEK SLOT

func Test_GivenEmptyHashTable_WhenSeekingSlot_ThenReturnsValidIndex(t *testing.T) {
	// Given
	ht := Init(17, 3)

	// When
	idx := ht.SeekSlot("test")

	// Then
	assert.GreaterOrEqual(t, idx, 0)
	assert.Less(t, idx, 17)
}

func Test_GivenHashTableWithValue_WhenSeekingSameValue_ThenReturnsMinusOne(t *testing.T) {
	// Given
	ht := Init(17, 3)
	ht.Put("test")

	// When
	idx := ht.SeekSlot("test")

	// Then
	assert.Equal(t, -1, idx)
}

func Test_GivenHashTableWithValue_WhenSeekingDifferentValue_ThenReturnsValidIndex(t *testing.T) {
	// Given
	ht := Init(17, 3)
	ht.Put("first")

	// When
	idx := ht.SeekSlot("second")

	// Then
	assert.GreaterOrEqual(t, idx, 0)
}

func Test_GivenFullHashTable_WhenSeekingSlot_ThenReturnsMinusOne(t *testing.T) {
	// Given
	ht := Init(3, 1)
	ht.Put("a")
	ht.Put("b")
	ht.Put("c")

	// When
	idx := ht.SeekSlot("d")

	// Then
	assert.Equal(t, -1, idx)
}

// PUT

func Test_GivenEmptyHashTable_WhenPuttingValue_ThenReturnsValidIndex(t *testing.T) {
	// Given
	ht := Init(17, 3)

	// When
	idx := ht.Put("test")

	// Then
	assert.GreaterOrEqual(t, idx, 0)
	assert.Less(t, idx, 17)
}

func Test_GivenHashTable_WhenPuttingDuplicateValue_ThenReturnsMinusOne(t *testing.T) {
	// Given
	ht := Init(17, 3)
	ht.Put("test")

	// When
	idx := ht.Put("test")

	// Then
	assert.Equal(t, -1, idx)
}

func Test_GivenFullHashTable_WhenPuttingValue_ThenReturnsMinusOne(t *testing.T) {
	// Given
	ht := Init(3, 1)
	ht.Put("a")
	ht.Put("b")
	ht.Put("c")

	// When
	idx := ht.Put("d")

	// Then
	assert.Equal(t, -1, idx)
}

func Test_GivenHashTable_WhenPuttingMultipleValues_ThenAllSucceed(t *testing.T) {
	// Given
	ht := Init(17, 3)

	// When
	idx1 := ht.Put("apple")
	idx2 := ht.Put("banana")
	idx3 := ht.Put("cherry")

	// Then
	assert.GreaterOrEqual(t, idx1, 0)
	assert.GreaterOrEqual(t, idx2, 0)
	assert.GreaterOrEqual(t, idx3, 0)
	assert.NotEqual(t, idx1, idx2)
	assert.NotEqual(t, idx2, idx3)
}

func Test_GivenHashTable_WhenPuttingEmptyString_ThenSucceeds(t *testing.T) {
	// Given
	ht := Init(17, 3)

	// When
	idx := ht.Put("")

	// Then
	assert.GreaterOrEqual(t, idx, 0)
}

// FIND

func Test_GivenEmptyHashTable_WhenFindingValue_ThenReturnsMinusOne(t *testing.T) {
	// Given
	ht := Init(17, 3)

	// When
	idx := ht.Find("test")

	// Then
	assert.Equal(t, -1, idx)
}

func Test_GivenHashTableWithValue_WhenFindingThatValue_ThenReturnsItsIndex(t *testing.T) {
	// Given
	ht := Init(17, 3)
	putIdx := ht.Put("test")

	// When
	findIdx := ht.Find("test")

	// Then
	assert.Equal(t, putIdx, findIdx)
}

func Test_GivenHashTableWithValue_WhenFindingDifferentValue_ThenReturnsMinusOne(t *testing.T) {
	// Given
	ht := Init(17, 3)
	ht.Put("apple")

	// When
	idx := ht.Find("banana")

	// Then
	assert.Equal(t, -1, idx)
}

func Test_GivenHashTableWithMultipleValues_WhenFindingEach_ThenReturnsTheirIndices(t *testing.T) {
	// Given
	ht := Init(17, 3)
	idx1 := ht.Put("apple")
	idx2 := ht.Put("banana")
	idx3 := ht.Put("cherry")

	// When
	find1 := ht.Find("apple")
	find2 := ht.Find("banana")
	find3 := ht.Find("cherry")

	// Then
	assert.Equal(t, idx1, find1)
	assert.Equal(t, idx2, find2)
	assert.Equal(t, idx3, find3)
}

func Test_GivenHashTable_WhenFindingEmptyString_ThenFindsItIfStored(t *testing.T) {
	// Given
	ht := Init(17, 3)
	putIdx := ht.Put("")

	// When
	findIdx := ht.Find("")

	// Then
	assert.Equal(t, putIdx, findIdx)
}

// COMBINED BEHAVIORS - PUT AND FIND

func Test_GivenHashTable_WhenPuttingAndFindingMultipleValues_ThenAllAreFoundCorrectly(t *testing.T) {
	// Given
	ht := Init(19, 5)

	// When
	ht.Put("one")
	ht.Put("two")
	ht.Put("three")
	ht.Put("four")

	// Then
	assert.NotEqual(t, -1, ht.Find("one"))
	assert.NotEqual(t, -1, ht.Find("two"))
	assert.NotEqual(t, -1, ht.Find("three"))
	assert.NotEqual(t, -1, ht.Find("four"))
	assert.Equal(t, -1, ht.Find("five"))
}

func Test_GivenSingleSlotTable_WhenPuttingTwoValues_ThenSecondFails(t *testing.T) {
	// Given
	ht := Init(1, 1)

	// When
	idx1 := ht.Put("first")
	idx2 := ht.Put("second")

	// Then
	assert.Equal(t, 0, idx1)
	assert.Equal(t, -1, idx2)
	assert.NotEqual(t, -1, ht.Find("first"))
	assert.Equal(t, -1, ht.Find("second"))
}

func Test_GivenHashTable_WhenPuttingSameEmptyStringTwice_ThenSecondFails(t *testing.T) {
	// Given
	ht := Init(10, 3)

	// When
	idx1 := ht.Put("")
	idx2 := ht.Put("")

	// Then
	assert.GreaterOrEqual(t, idx1, 0)
	assert.Equal(t, -1, idx2)
}

func Test_GivenFullTable_WhenSeekingNewValue_ThenReturnsMinusOneWithoutHanging(t *testing.T) {
	// Given
	ht := Init(3, 1)
	ht.Put("a")
	ht.Put("b")
	ht.Put("c")

	// When
	idx := ht.SeekSlot("d")

	// Then
	assert.Equal(t, -1, idx)
}

func Test_GivenHashTable_WhenInsertingWithCollisions_ThenHandlesCollisionsCorrectly(t *testing.T) {
	// Given
	ht := Init(5, 1)

	// When - insert values that will collide
	idx1 := ht.Put("a")
	idx2 := ht.Put("f")
	idx3 := ht.Put("k")

	// Then - all succeed and are findable
	assert.GreaterOrEqual(t, idx1, 0)
	assert.GreaterOrEqual(t, idx2, 0)
	assert.GreaterOrEqual(t, idx3, 0)
	assert.NotEqual(t, -1, ht.Find("a"))
	assert.NotEqual(t, -1, ht.Find("f"))
	assert.NotEqual(t, -1, ht.Find("k"))
}