package cache

import (
	"testing"

	"github.com/stretchr/testify/assert"
)

func Test_GivenEmptyCache_WhenGetting_ThenReturnsError(t *testing.T) {
	// Given
	cache := Init[int](5)

	// When
	val, err := cache.Get("01234")

	// Then
	assert.Error(t, err)
	assert.Equal(t, 0, val)
}

func Test_GivenEmptyCache_WhenPuttingSingleKey_ThenCanBeRetrieved(t *testing.T) {
	// Given
	cache := Init[int](5)

	// When
	cache.Put("01234", 100)
	val, err := cache.Get("01234")

	// Then
	assert.NoError(t, err)
	assert.Equal(t, 100, val)
}

func Test_GivenCacheWithKey_WhenPuttingSameKeyAgain_ThenValueIsUpdated(t *testing.T) {
	// Given
	cache := Init[int](5)
	cache.Put("01234", 100)

	// When
	cache.Put("01234", 200)
	val, err := cache.Get("01234")

	// Then
	assert.NoError(t, err)
	assert.Equal(t, 200, val)
}

func Test_GivenCache_WhenPuttingEmptyKey_ThenKeyIsNotStored(t *testing.T) {
	// Given
	cache := Init[int](5)

	// When
	cache.Put("", 100)
	val, err := cache.Get("")

	// Then
	assert.Error(t, err)
	assert.Equal(t, 0, val)
}

func Test_GivenFullCache_WhenPuttingNewKey_ThenLeastFrequentlyUsedIsEvicted(t *testing.T) {
	// Given
	cache := Init[int](5)

	cache.Put("01234", 0)
	cache.Put("12345", 1)
	cache.Put("23456", 2)
	cache.Put("34567", 3)
	cache.Put("45678", 4)

	_, _ = cache.Get("01234")
	_, _ = cache.Get("01234")

	_, _ = cache.Get("12345")

	_, _ = cache.Get("23456")
	_, _ = cache.Get("23456")
	_, _ = cache.Get("23456")

	_, _ = cache.Get("34567")

	// When
	cache.Put("56789", 5)

	// Then
	_, evictedError := cache.Get("45678")
	assert.Error(t, evictedError)

	valNew, errNew := cache.Get("56789")
	assert.NoError(t, errNew)
	assert.Equal(t, 5, valNew)
}
