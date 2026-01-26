package bloom_filter

import (
	"testing"

	"github.com/stretchr/testify/assert"	
)

func Test_GivenEmptyFilter_WhenAddingSingleString_ThenCanBeFound(t *testing.T) {
	// Given
	filter := Init()

	// When
	filter.Add("0123456789")

	// Then
	assert.True(t, filter.IsValue("0123456789"))
}

func Test_GivenEmptyFilter_WhenCheckingNonExistentString_ThenReturnsFalse(t *testing.T) {
	// Given
	filter := Init()

	// When
	exists := filter.IsValue("0123456789")

	// Then
	assert.False(t, exists)
}

func Test_GivenFilterWithMultipleStrings_WhenCheckingAll_ThenAllAreFound(t *testing.T) {
	// Given
	filter := Init()
	testStrings := []string{
		"0123456789", "1234567890", "2345678901", "3456789012",
		"4567890123", "5678901234", "6789012345", "7890123456",
		"8901234567", "9012345678",
	}

	// When
	for _, s := range testStrings {
		filter.Add(s)
	}

	// Then
	for _, s := range testStrings {
		assert.True(t, filter.IsValue(s), "String %s should be found", s)
	}
}

func Test_GivenFilterWithStrings_WhenAddingDuplicate_ThenStillFound(t *testing.T) {
	// Given
	filter := Init()
	filter.Add("0123456789")

	// When
	filter.Add("0123456789")

	// Then
	assert.True(t, filter.IsValue("0123456789"))
}

