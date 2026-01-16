package linkedlist

import (
	"testing"
	"github.com/stretchr/testify/assert"
)

// helpers

func makeList(values ...int) LinkedList {
	list := LinkedList{}
	for _, v := range values {
		list.AddInTail(Node{value: v})
	}
	return list
}

func toSlice(list *LinkedList) []int {
	if list.head == nil {
		return nil
	}
	var res []int
	for n := list.head; n != nil; n = n.next {
		res = append(res, n.value)
	}
	return res
}

// DELETE

func Test_GivenEmptyList_WhenDeletingValue_ThenSizeRemainsZero(t *testing.T) {
	// Given
	list := LinkedList{}

	// When
	list.Delete(42, false)

	// Then
	assert.Equal(t, 0, list.Count())
	assert.Nil(t, list.head)
	assert.Nil(t, list.tail)
}

func Test_GivenSingleNodeList_WhenDeletingDifferentValue_ThenListUnchanged(t *testing.T) {
	// Given
	list := makeList(7)
	oldHead := list.head
	oldTail := list.tail

	// When
	list.Delete(99, false)

	// Then
	assert.Equal(t, 1, list.Count())
	assert.Equal(t, []int{7}, toSlice(&list))
	assert.Equal(t, oldHead, list.head)
	assert.Equal(t, oldTail, list.tail)
	assert.Same(t, list.head, list.tail)
}

func Test_GivenSingleNodeList_WhenDeletingExistingValue_ThenListBecomesEmpty(t *testing.T) {
	// Given
	list := makeList(5)

	// When
	list.Delete(5, false)

	// Then
	assert.Equal(t, 0, list.Count())
	assert.Nil(t, list.head)
	assert.Nil(t, list.tail)
}

func Test_GivenMultiNodeList_WhenDeletingNonexistentValue_ThenAllRemainIntact(t *testing.T) {
	// Given
	list := makeList(1, 2, 3, 4)
	oldHead := list.head
	oldTail := list.tail

	// When
	list.Delete(99, false)

	// Then
	assert.Equal(t, 4, list.Count())
	assert.Equal(t, []int{1, 2, 3, 4}, toSlice(&list))
	assert.Equal(t, oldHead, list.head)
	assert.Equal(t, oldTail, list.tail)
	assert.Nil(t, list.tail.next)
}

func Test_GivenMultiNodeListWithDuplicates_WhenDeletingValue_ThenOnlyFirstOccurrenceRemoved(t *testing.T) {
	// Given
	list := makeList(1, 2, 3, 2, 4)
	oldTail := list.tail

	// When
	list.Delete(2, false)

	// Then
	assert.Equal(t, 4, list.Count())
	assert.Equal(t, []int{1, 3, 2, 4}, toSlice(&list))
	assert.Equal(t, oldTail, list.tail)
	assert.Equal(t, 4, list.tail.value)
	assert.Nil(t, list.tail.next)
}

func Test_GivenMultiNodeList_WhenDeletingHeadValue_ThenNextBecomesHead(t *testing.T) {
	// Given
	list := makeList(10, 20, 30)
	oldTail := list.tail

	// When
	list.Delete(10, false)

	// Then
	assert.Equal(t, 2, list.Count())
	assert.Equal(t, []int{20, 30}, toSlice(&list))
	assert.NotNil(t, list.head)
	assert.Equal(t, 20, list.head.value)
	assert.Equal(t, oldTail, list.tail)
	assert.Equal(t, 30, list.tail.value)
	assert.Nil(t, list.tail.next)
}

func Test_GivenMultiNodeList_WhenDeletingTailValue_ThenTailMovesBack(t *testing.T) {
	// Given
	list := makeList(1, 2, 3)

	// When
	list.Delete(3, false)

	// Then
	assert.Equal(t, 2, list.Count())
	assert.Equal(t, []int{1, 2}, toSlice(&list))
	assert.NotNil(t, list.tail)
	assert.Equal(t, 2, list.tail.value)
	assert.Nil(t, list.tail.next)
	assert.NotNil(t, list.head)
	assert.Equal(t, 1, list.head.value)
}

// DELETE ALL

func Test_GivenEmptyList_WhenDeletingAllValues_ThenListRemainsEmpty(t *testing.T) {
	// Given
	list := LinkedList{}

	// When
	list.Delete(5, true)

	// Then
	assert.Equal(t, 0, list.Count())
	assert.Nil(t, list.head)
	assert.Nil(t, list.tail)
}

func Test_GivenSingleNodeList_WhenDeletingAllMatchingValues_ThenListBecomesEmpty(t *testing.T) {
	// Given
	list := makeList(7)

	// When
	list.Delete(7, true)

	// Then
	assert.Equal(t, 0, list.Count())
	assert.Nil(t, list.head)
	assert.Nil(t, list.tail)
}

func Test_GivenSingleNodeList_WhenDeletingAllDifferentValues_ThenListUnchanged(t *testing.T) {
	// Given
	list := makeList(7)
	oldHead := list.head
	oldTail := list.tail

	// When
	list.Delete(99, true)

	// Then
	assert.Equal(t, 1, list.Count())
	assert.Equal(t, []int{7}, toSlice(&list))
	assert.Equal(t, oldHead, list.head)
	assert.Equal(t, oldTail, list.tail)
	assert.Same(t, list.head, list.tail)
}

func Test_GivenMultiNodeList_WhenDeletingAllMatchingValues_ThenListBecomesEmpty(t *testing.T) {
	// Given
	list := makeList(3, 3, 3, 3)

	// When
	list.Delete(3, true)

	// Then
	assert.Equal(t, 0, list.Count())
	assert.Nil(t, list.head)
	assert.Nil(t, list.tail)
}

func Test_GivenMultiNodeList_WhenDeletingAllMatchingValues_ThenOnlyThoseValuesRemoved(t *testing.T) {
	// Given
	list := makeList(1, 2, 3, 2, 4, 2)

	// When
	list.Delete(2, true)

	// Then
	assert.Equal(t, 3, list.Count())
	assert.Equal(t, []int{1, 3, 4}, toSlice(&list))
	assert.NotNil(t, list.head)
	assert.NotNil(t, list.tail)
	assert.Equal(t, 4, list.tail.value)
	assert.Nil(t, list.tail.next)
}

func Test_GivenMultiNodeList_WhenDeletingAllNonexistentValues_ThenListUnchanged(t *testing.T) {
	// Given
	list := makeList(10, 20, 30)
	oldHead := list.head
	oldTail := list.tail

	// When
	list.Delete(99, true)

	// Then
	assert.Equal(t, 3, list.Count())
	assert.Equal(t, []int{10, 20, 30}, toSlice(&list))
	assert.Equal(t, oldHead, list.head)
	assert.Equal(t, oldTail, list.tail)
	assert.Nil(t, list.tail.next)
}

// CLEAN

func Test_GivenEmptyList_WhenCleanCalled_ThenListRemainsEmpty(t *testing.T) {
	// Given
	list := LinkedList{}

	// When
	list.Clean()

	// Then
	assert.Equal(t, 0, list.Count())
	assert.Nil(t, list.head)
	assert.Nil(t, list.tail)
}

func Test_GivenSingleElementList_WhenCleanCalled_ThenListBecomesEmpty(t *testing.T) {
	// Given
	list := makeList(42)

	// When
	list.Clean()

	// Then
	assert.Equal(t, 0, list.Count())
	assert.Nil(t, list.head)
	assert.Nil(t, list.tail)
}

func Test_GivenMultipleElementsList_WhenCleanCalled_ThenListBecomesEmpty(t *testing.T) {
	// Given
	list := makeList(1, 2, 3, 4, 5)

	// When
	list.Clean()

	// Then
	assert.Equal(t, 0, list.Count())
	assert.Nil(t, list.head)
	assert.Nil(t, list.tail)
}

func Test_GivenMultipleElementsList_WhenCleanCalled_ThenSubsequentAddWorksCorrectly(t *testing.T) {
	// Given
	list := makeList(10, 20, 30)

	// When
	list.Clean()
	list.AddInTail(Node{value: 99})

	// Then
	assert.Equal(t, 1, list.Count())
	assert.Equal(t, []int{99}, toSlice(&list))
	assert.NotNil(t, list.head)
	assert.NotNil(t, list.tail)
	assert.Same(t, list.head, list.tail)
	assert.Nil(t, list.tail.next)
}

func Test_GivenMultipleElementsList_WhenCleanCalled_ThenFindAndDeleteReturnEmptyBehavior(t *testing.T) {
	// Given
	list := makeList(5, 10, 15)

	// When
	list.Clean()

	// Then
	_, err := list.Find(10)
	assert.Error(t, err)
	list.Delete(10, false)
	assert.Equal(t, 0, list.Count())
	assert.Nil(t, list.head)
	assert.Nil(t, list.tail)
}

// FIND

func Test_GivenEmptyList_WhenFindingValue_ThenErrorReturned(t *testing.T) {
	// Given
	list := LinkedList{}

	// When
	_, err := list.Find(42)

	// Then
	assert.Error(t, err)
	assert.Nil(t, list.head)
	assert.Nil(t, list.tail)
}

func Test_GivenSingleNodeList_WhenFindingExistingValue_ThenNodeReturned(t *testing.T) {
	// Given
	list := makeList(7)
	oldHead := list.head
	oldTail := list.tail

	// When
	node, err := list.Find(7)

	// Then
	assert.NoError(t, err)
	assert.Equal(t, 7, node.value)
	// no structural mutation
	assert.Equal(t, oldHead, list.head)
	assert.Equal(t, oldTail, list.tail)
	assert.Same(t, list.head, list.tail)
}

func Test_GivenSingleNodeList_WhenFindingDifferentValue_ThenErrorReturned(t *testing.T) {
	// Given
	list := makeList(7)

	// When
	_, err := list.Find(99)

	// Then
	assert.Error(t, err)
	assert.NotNil(t, list.head)
	assert.NotNil(t, list.tail)
	assert.Same(t, list.head, list.tail)
}

func Test_GivenMultiNodeList_WhenFindingExistingValue_ThenCorrectNodeReturned(t *testing.T) {
	// Given
	list := makeList(10, 20, 30)
	oldHead := list.head
	oldTail := list.tail

	// When
	node, err := list.Find(20)

	// Then
	assert.NoError(t, err)
	assert.Equal(t, 20, node.value)
	assert.Equal(t, oldHead, list.head)
	assert.Equal(t, oldTail, list.tail)
	assert.Nil(t, list.tail.next)
}

func Test_GivenMultiNodeList_WhenFindingNonexistentValue_ThenErrorReturned(t *testing.T) {
	// Given
	list := makeList(10, 20, 30)

	// When
	_, err := list.Find(99)

	// Then
	assert.Error(t, err)
	assert.NotNil(t, list.head)
	assert.NotNil(t, list.tail)
	assert.Nil(t, list.tail.next)
}

// FIND ALL

func Test_GivenEmptyList_WhenFindingAllValues_ThenReturnsEmptySlice(t *testing.T) {
	// Given
	list := LinkedList{}

	// When
	results := list.FindAll(42)

	// Then
	assert.Empty(t, results)
	assert.Nil(t, list.head)
	assert.Nil(t, list.tail)
}

func Test_GivenSingleNodeList_WhenFindingAllMatchingValues_ThenReturnsSingleNode(t *testing.T) {
	// Given
	list := makeList(5, 4, 3, 2, 1)

	// When
	results := list.FindAll(5)

	// Then
	assert.Len(t, results, 1)
	assert.Equal(t, 5, results[0].value)
	assert.NotNil(t, list.head)
	assert.NotNil(t, list.tail)
}

func Test_GivenSingleNodeList_WhenFindingAllNonMatchingValues_ThenReturnsEmptySlice(t *testing.T) {
	// Given
	list := makeList(5)

	// When
	results := list.FindAll(99)

	// Then
	assert.Empty(t, results)
	assert.NotNil(t, list.head)
	assert.NotNil(t, list.tail)
}

func Test_GivenMultiNodeList_WhenFindingAllMatchingValues_ThenReturnsAllOccurrences(t *testing.T) {
	// Given
	list := makeList(1, 2, 3, 2, 4, 2)

	// When
	results := list.FindAll(2)

	// Then
	assert.Len(t, results, 3)
	for _, n := range results {
		assert.Equal(t, 2, n.value)
	}
	assert.NotNil(t, list.head)
	assert.NotNil(t, list.tail)
	assert.Nil(t, list.tail.next)
}

func Test_GivenMultiNodeList_WhenFindingAllNonMatchingValues_ThenReturnsEmptySlice(t *testing.T) {
	// Given
	list := makeList(10, 20, 30)

	// When
	results := list.FindAll(99)

	// Then
	assert.Empty(t, results)
	assert.NotNil(t, list.head)
	assert.NotNil(t, list.tail)
	assert.Nil(t, list.tail.next)
}

// COUNT

func Test_GivenEmptyList_WhenObtainingCount_ThenReturnsZero(t *testing.T) {
	// Given
	list := LinkedList{}

	// When/Then
	assert.Equal(t, 0, list.Count())
	assert.Nil(t, list.head)
	assert.Nil(t, list.tail)
}

func Test_GivenSingleElementList_WhenObtainingCount_ThenReturnsOne(t *testing.T) {
	// Given
	list := makeList(5)

	// When/Then
	assert.Equal(t, 1, list.Count())
	assert.NotNil(t, list.head)
	assert.NotNil(t, list.tail)
	assert.Same(t, list.head, list.tail)
}

func Test_GivenMultipleElementsList_WhenObtainingCount_ThenReturnsCorrectValue(t *testing.T) {
	// Given
	list := makeList(1, 2, 3, 4, 5, 6, 7)

	// When/Then
	assert.Equal(t, 7, list.Count())
	assert.NotNil(t, list.head)
	assert.NotNil(t, list.tail)
	assert.Nil(t, list.tail.next)
}

// INSERT

func Test_GivenMiddleNode_WhenInsertingAfter_ThenNodeAppearsImmediatelyAfterIt(t *testing.T) {
	// Given
	list := makeList(1, 2, 3, 4)
	middle := list.head.next
	newNode := Node{value: 99}

	// When
	list.Insert(middle, newNode)

	// Then
	assert.Equal(t, []int{1, 2, 99, 3, 4}, toSlice(&list))
	assert.Equal(t, 5, list.Count())
	assert.NotNil(t, list.head)
	assert.NotNil(t, list.tail)
	assert.Equal(t, 4, list.tail.value)
	assert.Nil(t, list.tail.next)
}

func Test_GivenTailNode_WhenInsertingAfter_ThenNodeBecomesNewTail(t *testing.T) {
	// Given
	list := makeList(10, 20, 30)
	tail := list.tail
	newNode := Node{value: 99}

	// When
	list.Insert(tail, newNode)

	// Then
	assert.Equal(t, []int{10, 20, 30, 99}, toSlice(&list))
	assert.Equal(t, 4, list.Count())
	assert.NotNil(t, list.tail)
	assert.Equal(t, 99, list.tail.value)
	assert.Nil(t, list.tail.next)
}

func Test_GivenHeadNode_WhenInsertingAfter_ThenNodeInsertedAsSecond(t *testing.T) {
	// Given
	list := makeList(5, 10, 15)
	head := list.head
	newNode := Node{value: 7}

	// When
	list.Insert(head, newNode)

	// Then
	assert.Equal(t, []int{5, 7, 10, 15}, toSlice(&list))
	assert.Equal(t, 4, list.Count())
	assert.Equal(t, 15, list.tail.value)
	assert.Nil(t, list.tail.next)
}

func Test_GivenSingleNodeList_WhenInsertingAfterHead_ThenNodeAppended(t *testing.T) {
	// Given
	list := makeList(1)
	head := list.head
	newNode := Node{value: 2}

	// When
	list.Insert(head, newNode)

	// Then
	assert.Equal(t, []int{1, 2}, toSlice(&list))
	assert.Equal(t, 2, list.Count())
	assert.Equal(t, 2, list.tail.value)
	assert.Nil(t, list.tail.next)
}

// INSERT FIRST

func Test_GivenEmptyList_WhenInsertingFirst_ThenNodeBecomesHeadAndTail(t *testing.T) {
	// Given
	list := LinkedList{}
	newNode := Node{value: 10}

	// When
	list.InsertFirst(newNode)

	// Then
	assert.Equal(t, []int{10}, toSlice(&list))
	assert.Equal(t, 1, list.Count())
	assert.NotNil(t, list.head)
	assert.NotNil(t, list.tail)
	assert.Same(t, list.head, list.tail)
	assert.Equal(t, 10, list.tail.value)
	assert.Nil(t, list.tail.next)
}

func Test_GivenNonEmptyList_WhenInsertingFirst_ThenNodePrependedToFront(t *testing.T) {
	// Given
	list := makeList(2, 3, 4)
	newNode := Node{value: 1}

	// When
	list.InsertFirst(newNode)

	// Then
	assert.Equal(t, []int{1, 2, 3, 4}, toSlice(&list))
	assert.Equal(t, 4, list.Count())
	assert.Equal(t, 4, list.tail.value)
	assert.Nil(t, list.tail.next)
}

func Test_GivenMultipleInsertFirstCalls_WhenRepeatedlyAdding_ThenOrderReverses(t *testing.T) {
	// Given
	list := LinkedList{}

	// When
	list.InsertFirst(Node{value: 3})
	list.InsertFirst(Node{value: 2})
	list.InsertFirst(Node{value: 1})

	// Then
	assert.Equal(t, []int{1, 2, 3}, toSlice(&list))
	assert.Equal(t, 3, list.Count())
	assert.Equal(t, 3, list.tail.value)
	assert.Nil(t, list.tail.next)
}

// LISTS SUM

func Test_GivenTwoEmptyLists_WhenSummed_ThenResultIsEmptyList(t *testing.T) {
	// Given
	listA := LinkedList{}
	listB := LinkedList{}

	// When
	result, err := listsSum(&listA, &listB)

	// Then
	assert.NoError(t, err)
	assert.NotNil(t, result)
	assert.Equal(t, 0, result.Count())
	assert.Nil(t, result.head)
	assert.Nil(t, result.tail)
}

func Test_GivenTwoSingleElementLists_WhenSummed_ThenSingleSumReturned(t *testing.T) {
	// Given
	listA := makeList(3)
	listB := makeList(5)

	// When
	result, err := listsSum(&listA, &listB)

	// Then
	assert.NoError(t, err)
	assert.NotNil(t, result)
	assert.Equal(t, []int{8}, toSlice(result))
	assert.NotNil(t, result.head)
	assert.NotNil(t, result.tail)
	assert.Same(t, result.head, result.tail)
	assert.Nil(t, result.tail.next)
}

func Test_GivenTwoEqualLengthLists_WhenSummed_ThenElementwiseSumReturned(t *testing.T) {
	// Given
	listA := makeList(1, 2, 3)
	listB := makeList(4, 5, 6)

	// When
	result, err := listsSum(&listA, &listB)

	// Then
	assert.NoError(t, err)
	assert.Equal(t, []int{5, 7, 9}, toSlice(result))
	assert.Equal(t, 3, result.Count())
	assert.NotNil(t, result.tail)
	assert.Equal(t, 9, result.tail.value)
	assert.Nil(t, result.tail.next)
}

func Test_GivenListsOfDifferentLengths_WhenSummed_ThenErrorReturned(t *testing.T) {
	// Given
	listA := makeList(1, 2, 3)
	listB := makeList(4, 5)

	// When
	result, err := listsSum(&listA, &listB)

	// Then
	assert.Nil(t, result)
	assert.Error(t, err)
}