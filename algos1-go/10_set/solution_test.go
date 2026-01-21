package set

import (
	"testing"

	"github.com/stretchr/testify/assert"
)

// PUT TESTS

func Test_GivenEmptySet_WhenPuttingSingleValue_ThenCanBeFound(t *testing.T) {
	// Given
	set := Init[int]()

	// When
	set.Put(100)

	// Then
	assert.Equal(t, 1, set.Size())
	assert.True(t, set.Get(100))
}