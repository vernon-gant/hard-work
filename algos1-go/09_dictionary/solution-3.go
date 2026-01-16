package dictionary

import (
	"testing"

	"github.com/stretchr/testify/assert"
)

// PUT TESTS

func Test_GivenEmptyDict_WhenPuttingNewKey_ThenKeyIsStored(t *testing.T) {
	// Given
	dict := Init[int](17)

	// When
	dict.Put("key1", 100)

	// Then
	assert.True(t, dict.IsKey("key1"))
	val, err := dict.Get("key1")
	assert.NoError(t, err)
	assert.Equal(t, 100, val)
}

func Test_GivenDictWithKey_WhenPuttingSameKey_ThenValueIsUpdated(t *testing.T) {
	// Given
	dict := Init[int](17)
	dict.Put("key1", 100)

	// When
	dict.Put("key1", 200)

	// Then
	val, err := dict.Get("key1")
	assert.NoError(t, err)
	assert.Equal(t, 200, val)
}

func Test_GivenDict_WhenPuttingMultipleKeys_ThenAllAreStored(t *testing.T) {
	// Given
	dict := Init[string](17)

	// When
	dict.Put("apple", "red")
	dict.Put("banana", "yellow")
	dict.Put("grape", "purple")

	// Then
	assert.True(t, dict.IsKey("apple"))
	assert.True(t, dict.IsKey("banana"))
	assert.True(t, dict.IsKey("grape"))
}

func Test_GivenDict_WhenPuttingEmptyStringKey_ThenItWorks(t *testing.T) {
	// Given
	dict := Init[int](17)

	// When
	dict.Put("", 999)

	// Then
	assert.True(t, dict.IsKey(""))
	val, err := dict.Get("")
	assert.NoError(t, err)
	assert.Equal(t, 999, val)
}

// IS_KEY TESTS

func Test_GivenEmptyDict_WhenCheckingKey_ThenReturnsFalse(t *testing.T) {
	// Given
	dict := Init[int](17)

	// When
	exists := dict.IsKey("nonexistent")

	// Then
	assert.False(t, exists)
}

func Test_GivenDictWithKey_WhenCheckingExistingKey_ThenReturnsTrue(t *testing.T) {
	// Given
	dict := Init[int](17)
	dict.Put("key1", 100)

	// When
	exists := dict.IsKey("key1")

	// Then
	assert.True(t, exists)
}

func Test_GivenDictWithKey_WhenCheckingDifferentKey_ThenReturnsFalse(t *testing.T) {
	// Given
	dict := Init[int](17)
	dict.Put("key1", 100)

	// When
	exists := dict.IsKey("key2")

	// Then
	assert.False(t, exists)
}

func Test_GivenDictWithMultipleKeys_WhenCheckingEach_ThenCorrect(t *testing.T) {
	// Given
	dict := Init[string](17)
	dict.Put("first", "1st")
	dict.Put("second", "2nd")

	// When/Then
	assert.True(t, dict.IsKey("first"))
	assert.True(t, dict.IsKey("second"))
	assert.False(t, dict.IsKey("third"))
}

// GET TESTS

func Test_GivenEmptyDict_WhenGetting_ThenReturnsError(t *testing.T) {
	// Given
	dict := Init[int](17)

	// When
	val, err := dict.Get("notexist")

	// Then
	assert.Error(t, err)
	assert.Equal(t, 0, val)
}

func Test_GivenDictWithKey_WhenGettingExistingKey_ThenReturnsValue(t *testing.T) {
	// Given
	dict := Init[string](17)
	dict.Put("key1", "value1")

	// When
	val, err := dict.Get("key1")

	// Then
	assert.NoError(t, err)
	assert.Equal(t, "value1", val)
}

func Test_GivenDictWithKey_WhenGettingDifferentKey_ThenReturnsError(t *testing.T) {
	// Given
	dict := Init[int](17)
	dict.Put("key1", 100)

	// When
	val, err := dict.Get("key2")

	// Then
	assert.Error(t, err)
	assert.Equal(t, 0, val)
}

func Test_GivenDictWithMultipleKeys_WhenGettingEach_ThenReturnsCorrectValues(t *testing.T) {
	// Given
	dict := Init[float64](17)
	dict.Put("pi", 3.14159)
	dict.Put("e", 2.71828)

	// When
	val1, err1 := dict.Get("pi")
	val2, err2 := dict.Get("e")
	_, err3 := dict.Get("tau")

	// Then
	assert.NoError(t, err1)
	assert.Equal(t, 3.14159, val1)

	assert.NoError(t, err2)
	assert.Equal(t, 2.71828, val2)

	assert.Error(t, err3)
}

// COMBINED TESTS

func Test_GivenDict_WhenPutThenGet_ThenValuesMatch(t *testing.T) {
	// Given
	dict := Init[int](17)

	// When
	dict.Put("test", 123)
	val, err := dict.Get("test")

	// Then
	assert.NoError(t, err)
	assert.Equal(t, 123, val)
}

func Test_GivenDict_WhenUpdatingKeySeveralTimes_ThenLastValueReturned(t *testing.T) {
	// Given
	dict := Init[string](17)

	// When
	dict.Put("status", "pending")
	dict.Put("status", "processing")
	dict.Put("status", "complete")

	val, err := dict.Get("status")

	// Then
	assert.NoError(t, err)
	assert.Equal(t, "complete", val)
}

func Test_GivenSmallDict_WhenFillingMostSlots_ThenAllAccessible(t *testing.T) {
	// Given
	dict := Init[int](5)

	// When
	dict.Put("a", 1)
	dict.Put("b", 2)
	dict.Put("c", 3)
	dict.Put("d", 4)

	// Then
	assert.True(t, dict.IsKey("a"))
	assert.True(t, dict.IsKey("b"))
	assert.True(t, dict.IsKey("c"))
	assert.True(t, dict.IsKey("d"))

	val1, _ := dict.Get("a")
	val2, _ := dict.Get("b")
	val3, _ := dict.Get("c")
	val4, _ := dict.Get("d")

	assert.Equal(t, 1, val1)
	assert.Equal(t, 2, val2)
	assert.Equal(t, 3, val3)
	assert.Equal(t, 4, val4)
}

func Test_GivenDict_WhenMixingOperations_ThenBehavesConsistently(t *testing.T) {
	// Given
	dict := Init[string](17)

	// When/Then
	assert.False(t, dict.IsKey("x"))

	dict.Put("x", "value-x")
	assert.True(t, dict.IsKey("x"))

	val, err := dict.Get("x")
	assert.NoError(t, err)
	assert.Equal(t, "value-x", val)

	dict.Put("x", "updated-x")
	val2, err2 := dict.Get("x")
	assert.NoError(t, err2)
	assert.Equal(t, "updated-x", val2)
}