package dynamic_array

import (
	"testing"
	"github.com/stretchr/testify/assert"
)

func makeDyn(values ...int) *DynArray[int] {
	var da DynArray[int]
	da.Init()
	for _, v := range values {
		da.Append(v)
	}
	return &da
}

func toSlice(da *DynArray[int]) []int {
	// return the logical content (count elements)
	out := make([]int, 0, da.count)
	for i := 0; i < da.count; i++ {
		out = append(out, da.array[i])
	}
	return out
}

// APPEND

func Test_GivenSpaceInBuffer_WhenAppending_ThenCountIncreasesAndCapacityUnchanged(t *testing.T) {
	// Given
	da := makeDyn()

	// When
	for i := 0; i < 10; i++ {
		da.Append(i)
	}

	// Then
	assert.Equal(t, 10, da.count)
	assert.Equal(t, 16, da.capacity)
	assert.Equal(t, []int{0,1,2,3,4,5,6,7,8,9}, toSlice(da))
}

func Test_GivenFullBuffer_WhenAppending_ThenCapacityDoubles(t *testing.T) {
	// Given
	da := makeDyn()
	for i := 0; i < 16; i++ {
		da.Append(i)
	}
	oldCap := da.capacity

	// When
	da.Append(999)

	// Then
	assert.Equal(t, 17, da.count)
	assert.Equal(t, oldCap*2, da.capacity) // must be 32
	assert.Equal(t, 999, da.array[16])
}

// GET ITEM

func Test_GivenValidIndex_WhenGetItem_ThenReturnsValue(t *testing.T) {
	// Given
	da := makeDyn(10, 20, 30)

	// When
	v, err := da.GetItem(1)

	// Then
	assert.NoError(t, err)
	assert.Equal(t, 20, v)
	assert.Equal(t, 3, da.count)
	assert.Equal(t, 16, da.capacity)
}

func Test_GivenInvalidIndex_WhenGetItem_ThenErrorReturned(t *testing.T) {
	// Given
	da := makeDyn(10, 20, 30)

	// When
	_, errNeg := da.GetItem(-1)
	_, errBig := da.GetItem(3)

	// Then
	assert.Error(t, errNeg)
	assert.Error(t, errBig)
	assert.Equal(t, 3, da.count)
	assert.Equal(t, 16, da.capacity)
}

// INSERT

func Test_GivenEnoughSpace_WhenInsertMiddle_ThenShiftsAndCapacityUnchanged(t *testing.T) {
	// Given
	da := makeDyn(1, 2, 4, 5)
	oldCap := da.capacity

	// When
	err := da.Insert(3, 2)

	// Then
	assert.NoError(t, err)
	assert.Equal(t, 5, da.count)
	assert.Equal(t, oldCap, da.capacity)
	assert.Equal(t, []int{1,2,3,4,5}, toSlice(da))
}

func Test_GivenEnoughSpace_WhenInsertHead_ThenPrepends(t *testing.T) {
	// Given
	da := makeDyn(2, 3, 4)

	// When
	err := da.Insert(1, 0)

	// Then
	assert.NoError(t, err)
	assert.Equal(t, []int{1,2,3,4}, toSlice(da))
	assert.Equal(t, 4, da.count)
	assert.Equal(t, 16, da.capacity)
}

func Test_GivenEnoughSpace_WhenInsertTail_ThenAppends(t *testing.T) {
	// Given
	da := makeDyn(1, 2, 3)

	// When
	err := da.Insert(4, 3)

	// Then
	assert.NoError(t, err)
	assert.Equal(t, []int{1,2,3,4}, toSlice(da))
	assert.Equal(t, 4, da.count)
	assert.Equal(t, 16, da.capacity)
}

func Test_GivenFullBuffer_WhenInsert_ThenCapacityDoublesAndDataShifted(t *testing.T) {
	// Given
	da := makeDyn()
	for i := 0; i < 16; i++ {
		da.Append(i)
	}
	// When
	err := da.Insert(777, 8)

	// Then
	assert.NoError(t, err)
	assert.Equal(t, 17, da.count)
	assert.Equal(t, 32, da.capacity)
	got := toSlice(da)
	assert.Equal(t, 777, got[8])
	assert.Equal(t, 7, got[7])
	assert.Equal(t, 8, got[9])
}

func Test_GivenInvalidIndex_WhenInsert_ThenErrorAndNoMutation(t *testing.T) {
	// Given
	da := makeDyn(1,2,3)
	old := toSlice(da)
	oldCap, oldCnt := da.capacity, da.count

	// When
	errNeg := da.Insert(99, -1)
	errBig := da.Insert(99, 4)

	// Then
	assert.Error(t, errNeg)
	assert.Error(t, errBig)
	assert.Equal(t, oldCnt, da.count)
	assert.Equal(t, oldCap, da.capacity)
	assert.Equal(t, old, toSlice(da))
}

// REMOVE

func Test_GivenSufficientLoad_WhenRemove_ThenCapacityUnchangedAndShiftLeft(t *testing.T) {
	// Given
	da := makeDyn(10, 20, 30, 40, 50)
	oldCap := da.capacity

	// When
	err := da.Remove(2)

	// Then
	assert.NoError(t, err)
	assert.Equal(t, 4, da.count)
	assert.Equal(t, oldCap, da.capacity)
	assert.Equal(t, []int{10, 20, 40, 50}, toSlice(da))
}

func Test_GivenDroppingBelowHalf_WhenRemove_ThenCapacityShrinksByOnePointFive(t *testing.T) {
	// Given
	da := makeDyn()
	for i := 0; i < 17; i++ { da.Append(i) }

	_ = da.Remove(da.count-1)
	err := da.Remove(da.count-1)

	// Then
	assert.NoError(t, err)
	assert.Equal(t, 15, da.count)
	assert.Equal(t, 21, da.capacity)
}

func Test_GivenInvalidIndex_WhenRemove_ThenErrorAndNoMutation(t *testing.T) {
	// Given
	da := makeDyn(1, 2, 3)
	before := toSlice(da)
	oldCap, oldCnt := da.capacity, da.count

	// When
	errNeg := da.Remove(-1)
	errBig := da.Remove(3)

	// Then
	assert.Error(t, errNeg)
	assert.Error(t, errBig)
	assert.Equal(t, oldCnt, da.count)
	assert.Equal(t, oldCap, da.capacity)
	assert.Equal(t, before, toSlice(da))
}