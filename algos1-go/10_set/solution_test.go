package set

import (
	"testing"

	"github.com/stretchr/testify/assert"
)

func Test_GivenEmptySet_WhenPuttingSingleValue_ThenCanBeFound(t *testing.T) {
	// Given
	set := Init[int]()

	// When
	set.Put(100)

	// Then
	assert.Equal(t, 1, set.Size())
	assert.True(t, set.Get(100))
}

func Test_GivenEmptySet_WhenPuttingMultipleValues_ThenTheyAllArePresent(t *testing.T) {
	// Given
	set := Init[int]()

	// When
	set.Put(100)
	set.Put(-100)
	set.Put(50)
	set.Put(25)

	// Then
	assert.Equal(t, 4, set.Size())
	assert.True(t, set.Get(100))
	assert.True(t, set.Get(-100))
	assert.True(t, set.Get(50))
	assert.True(t, set.Get(25))
}

func Test_GivenNonEmptySet_WhenPuttingDuplicateValues_ThenNothingChanges(t *testing.T) {
	// Given
	set := Init[int]()
	set.Put(100)
	set.Put(-100)
	set.Put(50)
	set.Put(25)

	// When
	set.Put(100)
	set.Put(-100)
	set.Put(50)
	set.Put(25)

	// Then
	assert.Equal(t, 4, set.Size())
	assert.True(t, set.Get(100))
	assert.True(t, set.Get(-100))
	assert.True(t, set.Get(50))
	assert.True(t, set.Get(25))
}

func Test_GivenFullSet_WhenPuttingValues_ThenSizeDoesNotChangeAndMethodReturns(t *testing.T) {
	// Given
	set := Init[int]()
	step, current := 7, 0

	for i := 0; i < SET_SIZE; i++ {
		set.Put(current)
		current = (current + step) % SET_SIZE
	}

	// When
	set.Put(100)
	set.Put(-100)
	set.Put(50)
	set.Put(25)

	// Then
	assert.Equal(t, SET_SIZE, set.Size())
}

func Test_GivenEmptySet_WhenRemovingValue_ThenReturnsFalse(t *testing.T) {
	// Given
	set := Init[int]()

	// When
	removed := set.Remove(100)

	// Then
	assert.False(t, removed)
	assert.Equal(t, 0, set.Size())
}

func Test_GivenSetWithOneElement_WhenRemovingIt_ThenSetBecomesEmpty(t *testing.T) {
	// Given
	set := Init[int]()
	set.Put(100)

	// When
	removed := set.Remove(100)

	// Then
	assert.True(t, removed)
	assert.Equal(t, 0, set.Size())
	assert.False(t, set.Get(100))
}

func Test_GivenSetWithFiveElements_WhenRemovingMiddleElement_ThenOnlyMiddleRemoved(t *testing.T) {
	// Given
	set := Init[int]()
	set.Put(1)
	set.Put(2)
	set.Put(3)
	set.Put(4)
	set.Put(5)

	// When
	removed := set.Remove(3)

	// Then
	assert.True(t, removed)
	assert.Equal(t, 4, set.Size())
	assert.False(t, set.Get(3))
}

func Test_GivenSetWithFiveElements_WhenRemovingLastElement_ThenOnlyLastRemoved(t *testing.T) {
	// Given
	set := Init[int]()
	set.Put(1)
	set.Put(2)
	set.Put(3)
	set.Put(4)
	set.Put(5)

	// When
	removed := set.Remove(5)

	// Then
	assert.True(t, removed)
	assert.Equal(t, 4, set.Size())
	assert.False(t, set.Get(5))
}

func Test_GivenSetWithFiveElements_WhenRemovingNonExistent_ThenNothingChanges(t *testing.T) {
	// Given
	set := Init[int]()
	set.Put(1)
	set.Put(2)
	set.Put(3)
	set.Put(4)
	set.Put(5)

	// When
	removed := set.Remove(999)

	// Then
	assert.False(t, removed)
	assert.Equal(t, 5, set.Size())
}

func Test_GivenSetWithFiveElements_WhenRemovingAll_ThenSetBecomesEmpty(t *testing.T) {
	// Given
	set := Init[int]()
	set.Put(1)
	set.Put(2)
	set.Put(3)
	set.Put(4)
	set.Put(5)

	// When
	set.Remove(1)
	set.Remove(2)
	set.Remove(3)
	set.Remove(4)
	set.Remove(5)

	// Then
	assert.Equal(t, 0, set.Size())
}

func Test_GivenTwoDisjointSets_WhenIntersecting_ThenResultIsEmpty(t *testing.T) {
	// Given
	set1 := Init[int]()
	set1.Put(1)
	set1.Put(2)
	set1.Put(3)

	set2 := Init[int]()
	set2.Put(4)
	set2.Put(5)
	set2.Put(6)

	// When
	result := set1.Intersection(set2)

	// Then
	assert.Equal(t, 0, result.Size())
}

func Test_GivenTwoOverlappingSets_WhenIntersecting_ThenResultContainsCommonElements(t *testing.T) {
	// Given
	set1 := Init[int]()
	set1.Put(1)
	set1.Put(2)
	set1.Put(3)
	set1.Put(4)

	set2 := Init[int]()
	set2.Put(3)
	set2.Put(4)
	set2.Put(5)

	// When
	result := set1.Intersection(set2)

	// Then
	assert.Equal(t, 2, result.Size())
	assert.True(t, result.Get(3))
	assert.True(t, result.Get(4))
}

func Test_GivenEmptySetAndNonEmptySet_WhenIntersecting_ThenResultIsEmpty(t *testing.T) {
	// Given
	set1 := Init[int]()

	set2 := Init[int]()
	set2.Put(1)
	set2.Put(2)

	// When
	result := set1.Intersection(set2)

	// Then
	assert.Equal(t, 0, result.Size())
}

func Test_GivenTwoNonEmptySets_WhenUnion_ThenResultContainsAllElements(t *testing.T) {
	// Given
	set1 := Init[int]()
	set1.Put(1)
	set1.Put(2)
	set1.Put(3)

	set2 := Init[int]()
	set2.Put(3)
	set2.Put(4)
	set2.Put(5)

	// When
	result := set1.Union(set2)

	// Then
	assert.Equal(t, 5, result.Size())
	assert.True(t, result.Get(1))
	assert.True(t, result.Get(2))
	assert.True(t, result.Get(3))
	assert.True(t, result.Get(4))
	assert.True(t, result.Get(5))
}

func Test_GivenEmptySetAndNonEmptySet_WhenUnion_ThenResultEqualsNonEmptySet(t *testing.T) {
	// Given
	set1 := Init[int]()

	set2 := Init[int]()
	set2.Put(1)
	set2.Put(2)
	set2.Put(3)

	// When
	result := set1.Union(set2)

	// Then
	assert.Equal(t, 3, result.Size())
	assert.True(t, result.Get(1))
	assert.True(t, result.Get(2))
	assert.True(t, result.Get(3))
}

func Test_GivenTwoIdenticalSets_WhenUnion_ThenResultEqualsOriginal(t *testing.T) {
	// Given
	set1 := Init[int]()
	set1.Put(1)
	set1.Put(2)

	set2 := Init[int]()
	set2.Put(1)
	set2.Put(2)

	// When
	result := set1.Union(set2)

	// Then
	assert.Equal(t, 2, result.Size())
	assert.True(t, result.Get(1))
	assert.True(t, result.Get(2))
}

func Test_GivenSubsetOfCurrentSet_WhenIsSubset_ThenReturnsTrue(t *testing.T) {
	// Given
	set1 := Init[int]()
	set1.Put(1)
	set1.Put(2)
	set1.Put(3)
	set1.Put(4)
	set1.Put(5)

	set2 := Init[int]()
	set2.Put(2)
	set2.Put(3)
	set2.Put(4)

	// When
	result := set1.IsSubset(set2)

	// Then
	assert.True(t, result)
}

func Test_GivenCurrentSetIsSubsetOfParameter_WhenIsSubset_ThenReturnsFalse(t *testing.T) {
	// Given
	set1 := Init[int]()
	set1.Put(2)
	set1.Put(3)

	set2 := Init[int]()
	set2.Put(1)
	set2.Put(2)
	set2.Put(3)
	set2.Put(4)

	// When
	result := set1.IsSubset(set2)

	// Then
	assert.False(t, result)
}

func Test_GivenPartialOverlap_WhenIsSubset_ThenReturnsFalse(t *testing.T) {
	// Given
	set1 := Init[int]()
	set1.Put(1)
	set1.Put(2)
	set1.Put(3)

	set2 := Init[int]()
	set2.Put(2)
	set2.Put(3)
	set2.Put(4)

	// When
	result := set1.IsSubset(set2)

	// Then
	assert.False(t, result)
}

func Test_GivenEmptySet_WhenIsSubset_ThenReturnsTrue(t *testing.T) {
	// Given
	set1 := Init[int]()
	set1.Put(1)
	set1.Put(2)

	set2 := Init[int]()

	// When
	result := set1.IsSubset(set2)

	// Then
	assert.True(t, result)
}

func Test_GivenIdenticalSets_WhenEquals_ThenReturnsTrue(t *testing.T) {
	// Given
	set1 := Init[int]()
	set1.Put(1)
	set1.Put(2)
	set1.Put(3)

	set2 := Init[int]()
	set2.Put(1)
	set2.Put(2)
	set2.Put(3)

	// When
	result := set1.Equals(set2)

	// Then
	assert.True(t, result)
}

func Test_GivenDifferentSizes_WhenEquals_ThenReturnsFalse(t *testing.T) {
	// Given
	set1 := Init[int]()
	set1.Put(1)
	set1.Put(2)

	set2 := Init[int]()
	set2.Put(1)
	set2.Put(2)
	set2.Put(3)

	// When
	result := set1.Equals(set2)

	// Then
	assert.False(t, result)
}

func Test_GivenDifferentElements_WhenEquals_ThenReturnsFalse(t *testing.T) {
	// Given
	set1 := Init[int]()
	set1.Put(1)
	set1.Put(2)
	set1.Put(3)

	set2 := Init[int]()
	set2.Put(1)
	set2.Put(2)
	set2.Put(4)

	// When
	result := set1.Equals(set2)

	// Then
	assert.False(t, result)
}

func Test_GivenTwoEmptySets_WhenEquals_ThenReturnsTrue(t *testing.T) {
	// Given
	set1 := Init[int]()
	set2 := Init[int]()

	// When
	result := set1.Equals(set2)

	// Then
	assert.True(t, result)
}