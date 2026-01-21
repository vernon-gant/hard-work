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