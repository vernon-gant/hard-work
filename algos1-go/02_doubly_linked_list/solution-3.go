package doubly_linked_list

import (
	"testing"

	"github.com/stretchr/testify/assert"
)

// ------------------------
// helpers
// ------------------------

func makeList2(values ...int) LinkedList2 {
	var list LinkedList2
	for _, v := range values {
		list.AddInTail(Node{value: v})
	}
	return list
}

func toSlice(list *LinkedList2) []int {
	res := make([]int, 0, list.Count())
	for n := list.head; n != nil; n = n.next {
		res = append(res, n.value)
	}
	return res
}

func toReverseSlice(list *LinkedList2) []int {
	res := make([]int, 0, list.Count())
	for n := list.tail; n != nil; n = n.prev {
		res = append(res, n.value)
	}
	return res
}

func reverseInts(in []int) []int {
	out := make([]int, len(in))
	for i := range in {
		out[i] = in[len(in)-1-i]
	}
	return out
}

// DELETE (single)

func Test_GivenEmptyList_WhenDeletingValue_ThenSizeRemainsZero_Doubly(t *testing.T) {
	list := LinkedList2{}

	list.Delete(42, false)

	assert.Equal(t, 0, list.Count())
	assert.Nil(t, list.head)
	assert.Nil(t, list.tail)
}

func Test_GivenSingleNodeList_WhenDeletingDifferentValue_ThenListUnchanged_Doubly(t *testing.T) {
	list := makeList2(7)
	oldHead, oldTail := list.head, list.tail

	list.Delete(99, false)

	assert.Equal(t, 1, list.Count())
	assert.Equal(t, []int{7}, toSlice(&list))
	assert.Equal(t, []int{7}, toReverseSlice(&list))
	assert.Equal(t, oldHead, list.head)
	assert.Equal(t, oldTail, list.tail)
	assert.Same(t, list.head, list.tail)
	assert.Nil(t, list.head.prev)
	assert.Nil(t, list.tail.next)
}

func Test_GivenSingleNodeList_WhenDeletingExistingValue_ThenListBecomesEmpty_Doubly(t *testing.T) {
	list := makeList2(5)

	list.Delete(5, false)

	assert.Equal(t, 0, list.Count())
	assert.Nil(t, list.head)
	assert.Nil(t, list.tail)
	assert.Empty(t, toSlice(&list))
	assert.Empty(t, toReverseSlice(&list))
}

func Test_GivenMultiNodeList_WhenDeletingNonexistentValue_ThenAllRemainIntact_Doubly(t *testing.T) {
	list := makeList2(1, 2, 3, 4)
	oldHead, oldTail := list.head, list.tail

	list.Delete(99, false)

	assert.Equal(t, 4, list.Count())
	fw := toSlice(&list)
	bw := toReverseSlice(&list)
	assert.Equal(t, []int{1, 2, 3, 4}, fw)
	assert.Equal(t, reverseInts(fw), bw)
	assert.Equal(t, oldHead, list.head)
	assert.Equal(t, oldTail, list.tail)
	assert.Nil(t, list.head.prev)
	assert.Nil(t, list.tail.next)
	assert.Equal(t, list.head.next.prev, list.head)
}

func Test_GivenMultiNodeListWithDuplicates_WhenDeletingValue_ThenOnlyFirstOccurrenceRemoved_Doubly(t *testing.T) {
	list := makeList2(1, 2, 3, 2, 4)
	oldTail := list.tail

	list.Delete(2, false)

	assert.Equal(t, 4, list.Count())
	fw := toSlice(&list)
	bw := toReverseSlice(&list)
	assert.Equal(t, []int{1, 3, 2, 4}, fw)
	assert.Equal(t, reverseInts(fw), bw)
	assert.Equal(t, oldTail, list.tail)
	assert.Nil(t, list.tail.next)
}

func Test_GivenMultiNodeList_WhenDeletingHeadValue_ThenNextBecomesHead_Doubly(t *testing.T) {
	list := makeList2(10, 20, 30)
	oldTail := list.tail

	list.Delete(10, false)

	assert.Equal(t, 2, list.Count())
	fw := toSlice(&list)
	assert.Equal(t, []int{20, 30}, fw)
	assert.Equal(t, reverseInts(fw), toReverseSlice(&list))
	assert.NotNil(t, list.head)
	assert.Equal(t, 20, list.head.value)
	assert.Nil(t, list.head.prev)
	assert.Equal(t, oldTail, list.tail)
	assert.Equal(t, 30, list.tail.value)
	assert.Nil(t, list.tail.next)
	assert.Equal(t, list.head.next.prev, list.head)
}

func Test_GivenMultiNodeList_WhenDeletingTailValue_ThenTailMovesBack_Doubly(t *testing.T) {
	list := makeList2(1, 2, 3)

	list.Delete(3, false)

	assert.Equal(t, 2, list.Count())
	fw := toSlice(&list)
	assert.Equal(t, []int{1, 2}, fw)
	assert.Equal(t, reverseInts(fw), toReverseSlice(&list))
	assert.NotNil(t, list.tail)
	assert.Equal(t, 2, list.tail.value)
	assert.Nil(t, list.tail.next)
	assert.NotNil(t, list.head)
	assert.Equal(t, 1, list.head.value)
	assert.Nil(t, list.head.prev)
	assert.Equal(t, list.tail.prev.value, 1)
}

// DELETE ALL

func Test_GivenEmptyList_WhenDeletingAllValues_ThenListRemainsEmpty_Doubly(t *testing.T) {
	list := LinkedList2{}

	list.Delete(5, true)

	assert.Equal(t, 0, list.Count())
	assert.Nil(t, list.head)
	assert.Nil(t, list.tail)
	assert.Equal(t, []int{}, toSlice(&list))
	assert.Equal(t, []int{}, toReverseSlice(&list))
}

func Test_GivenSingleNodeList_WhenDeletingAllMatchingValues_ThenListBecomesEmpty_Doubly(t *testing.T) {
	list := makeList2(7)

	list.Delete(7, true)

	assert.Equal(t, 0, list.Count())
	assert.Nil(t, list.head)
	assert.Nil(t, list.tail)
	assert.Equal(t, []int{}, toSlice(&list))
	assert.Equal(t, []int{}, toReverseSlice(&list))
}

func Test_GivenSingleNodeList_WhenDeletingAllDifferentValues_ThenListUnchanged_Doubly(t *testing.T) {
	list := makeList2(7)
	oldHead, oldTail := list.head, list.tail

	list.Delete(99, true)

	assert.Equal(t, 1, list.Count())
	fw := toSlice(&list)
	assert.Equal(t, []int{7}, fw)
	assert.Equal(t, reverseInts(fw), toReverseSlice(&list))
	assert.Equal(t, oldHead, list.head)
	assert.Equal(t, oldTail, list.tail)
	assert.Same(t, list.head, list.tail)
	assert.Nil(t, list.head.prev)
	assert.Nil(t, list.tail.next)
}

func Test_GivenMultiNodeList_WhenDeletingAllMatchingValues_ThenListBecomesEmpty_Doubly(t *testing.T) {
	list := makeList2(3, 3, 3, 3)

	list.Delete(3, true)

	assert.Equal(t, 0, list.Count())
	assert.Nil(t, list.head)
	assert.Nil(t, list.tail)
	assert.Equal(t, []int{}, toSlice(&list))
	assert.Equal(t, []int{}, toReverseSlice(&list))
}

func Test_GivenMultiNodeList_WhenDeletingAllMatchingValues_ThenOnlyThoseValuesRemoved_Doubly(t *testing.T) {
	list := makeList2(1, 2, 3, 2, 4, 2)

	list.Delete(2, true)

	assert.Equal(t, 3, list.Count())
	fw := toSlice(&list)
	assert.Equal(t, []int{1, 3, 4}, fw)
	assert.Equal(t, reverseInts(fw), toReverseSlice(&list))
	assert.NotNil(t, list.head)
	assert.NotNil(t, list.tail)
	assert.Equal(t, 4, list.tail.value)
	assert.Nil(t, list.tail.next)
	assert.Nil(t, list.head.prev)
}

func Test_GivenMultiNodeList_WhenDeletingAllNonexistentValues_ThenListUnchanged_Doubly(t *testing.T) {
	list := makeList2(10, 20, 30)
	oldHead, oldTail := list.head, list.tail

	list.Delete(99, true)

	assert.Equal(t, 3, list.Count())
	fw := toSlice(&list)
	assert.Equal(t, []int{10, 20, 30}, fw)
	assert.Equal(t, reverseInts(fw), toReverseSlice(&list))
	assert.Equal(t, oldHead, list.head)
	assert.Equal(t, oldTail, list.tail)
	assert.Nil(t, list.tail.next)
	assert.Nil(t, list.head.prev)
}

// CLEAN

func Test_GivenEmptyList_WhenCleanCalled_ThenListRemainsEmpty_Doubly(t *testing.T) {
	list := LinkedList2{}

	list.Clean()

	assert.Equal(t, 0, list.Count())
	assert.Nil(t, list.head)
	assert.Nil(t, list.tail)
	assert.Equal(t, []int{}, toSlice(&list))
	assert.Equal(t, []int{}, toReverseSlice(&list))
}

func Test_GivenSingleElementList_WhenCleanCalled_ThenListBecomesEmpty_Doubly(t *testing.T) {
	list := makeList2(42)

	list.Clean()

	assert.Equal(t, 0, list.Count())
	assert.Nil(t, list.head)
	assert.Nil(t, list.tail)
	assert.Equal(t, []int{}, toSlice(&list))
	assert.Equal(t, []int{}, toReverseSlice(&list))
}

func Test_GivenMultipleElementsList_WhenCleanCalled_ThenListBecomesEmpty_Doubly(t *testing.T) {
	list := makeList2(1, 2, 3, 4, 5)

	list.Clean()

	assert.Equal(t, 0, list.Count())
	assert.Nil(t, list.head)
	assert.Nil(t, list.tail)
	assert.Equal(t, []int{}, toSlice(&list))
	assert.Equal(t, []int{}, toReverseSlice(&list))
}

func Test_GivenMultipleElementsList_WhenCleanCalled_ThenSubsequentAddWorksCorrectly_Doubly(t *testing.T) {
	list := makeList2(10, 20, 30)

	list.Clean()
	list.AddInTail(Node{value: 99})

	assert.Equal(t, 1, list.Count())
	assert.Equal(t, []int{99}, toSlice(&list))
	assert.Equal(t, []int{99}, toReverseSlice(&list))
	assert.NotNil(t, list.head)
	assert.NotNil(t, list.tail)
	assert.Same(t, list.head, list.tail)
	assert.Nil(t, list.tail.next)
	assert.Nil(t, list.head.prev)
}

// FIND

func Test_GivenEmptyList_WhenFindingValue_ThenErrorReturned_Doubly(t *testing.T) {
	list := LinkedList2{}

	_, err := list.Find(42)

	assert.Error(t, err)
	assert.Nil(t, list.head)
	assert.Nil(t, list.tail)
	assert.Equal(t, []int{}, toSlice(&list))
	assert.Equal(t, []int{}, toReverseSlice(&list))
}

func Test_GivenSingleNodeList_WhenFindingExistingValue_ThenNodeReturned_Doubly(t *testing.T) {
	list := makeList2(7)
	oldHead, oldTail := list.head, list.tail

	node, err := list.Find(7)

	assert.NoError(t, err)
	assert.Equal(t, 7, node.value)
	assert.Equal(t, oldHead, list.head)
	assert.Equal(t, oldTail, list.tail)
	assert.Same(t, list.head, list.tail)
	assert.Nil(t, list.head.prev)
	assert.Nil(t, list.tail.next)
}

func Test_GivenSingleNodeList_WhenFindingDifferentValue_ThenErrorReturned_Doubly(t *testing.T) {
	list := makeList2(7)

	_, err := list.Find(99)

	assert.Error(t, err)
	assert.NotNil(t, list.head)
	assert.NotNil(t, list.tail)
	assert.Same(t, list.head, list.tail)
	assert.Nil(t, list.head.prev)
	assert.Nil(t, list.tail.next)
}

func Test_GivenMultiNodeList_WhenFindingExistingValue_ThenCorrectNodeReturned_Doubly(t *testing.T) {
	list := makeList2(10, 20, 30)
	oldHead, oldTail := list.head, list.tail

	node, err := list.Find(20)

	assert.NoError(t, err)
	assert.Equal(t, 20, node.value)
	assert.Equal(t, oldHead, list.head)
	assert.Equal(t, oldTail, list.tail)
	assert.Nil(t, list.tail.next)
	assert.Nil(t, list.head.prev)
}

func Test_GivenMultiNodeList_WhenFindingNonexistentValue_ThenErrorReturned_Doubly(t *testing.T) {
	list := makeList2(10, 20, 30)

	_, err := list.Find(99)

	assert.Error(t, err)
	assert.NotNil(t, list.head)
	assert.NotNil(t, list.tail)
	assert.Nil(t, list.tail.next)
	assert.Nil(t, list.head.prev)
}

// FIND ALL

func Test_GivenEmptyList_WhenFindingAllValues_ThenReturnsEmptySlice_Doubly(t *testing.T) {
	list := LinkedList2{}

	results := list.FindAll(42)

	assert.Empty(t, results)
	assert.Equal(t, 0, list.Count())
	assert.Equal(t, []int{}, toSlice(&list))
	assert.Equal(t, []int{}, toReverseSlice(&list))
}

func Test_GivenSingleNodeList_WhenFindingAllMatchingValues_ThenReturnsSingleNode_Doubly(t *testing.T) {
	list := makeList2(5, 4, 3, 2, 1)

	results := list.FindAll(5)

	assert.Len(t, results, 1)
	assert.Equal(t, 5, results[0].value)
	assert.NotNil(t, list.head)
	assert.NotNil(t, list.tail)
	assert.Nil(t, list.head.prev)
	assert.Nil(t, list.tail.next)
}

func Test_GivenSingleNodeList_WhenFindingAllNonMatchingValues_ThenReturnsEmptySlice_Doubly(t *testing.T) {
	list := makeList2(5)

	results := list.FindAll(99)

	assert.Empty(t, results)
	assert.NotNil(t, list.head)
	assert.NotNil(t, list.tail)
	assert.Same(t, list.head, list.tail)
	assert.Nil(t, list.head.prev)
	assert.Nil(t, list.tail.next)
}

func Test_GivenMultiNodeList_WhenFindingAllMatchingValues_ThenReturnsAllOccurrences_Doubly(t *testing.T) {
	list := makeList2(1, 2, 3, 2, 4, 2)

	results := list.FindAll(2)

	assert.Len(t, results, 3)
	for _, n := range results {
		assert.Equal(t, 2, n.value)
	}
	assert.NotNil(t, list.head)
	assert.NotNil(t, list.tail)
	assert.Nil(t, list.tail.next)
	assert.Nil(t, list.head.prev)
}

func Test_GivenMultiNodeList_WhenFindingAllNonMatchingValues_ThenReturnsEmptySlice_Doubly(t *testing.T) {
	list := makeList2(10, 20, 30)

	results := list.FindAll(99)

	assert.Empty(t, results)
	assert.NotNil(t, list.head)
	assert.NotNil(t, list.tail)
	assert.Nil(t, list.tail.next)
	assert.Nil(t, list.head.prev)
}

// COUNT

func Test_GivenEmptyList_WhenObtainingCount_ThenReturnsZero_Doubly(t *testing.T) {
	list := LinkedList2{}

	assert.Equal(t, 0, list.Count())
	assert.Nil(t, list.head)
	assert.Nil(t, list.tail)
	assert.Equal(t, []int{}, toSlice(&list))
	assert.Equal(t, []int{}, toReverseSlice(&list))
}

func Test_GivenSingleElementList_WhenObtainingCount_ThenReturnsOne_Doubly(t *testing.T) {
	list := makeList2(5)

	assert.Equal(t, 1, list.Count())
	assert.NotNil(t, list.head)
	assert.NotNil(t, list.tail)
	assert.Same(t, list.head, list.tail)
	assert.Nil(t, list.head.prev)
	assert.Nil(t, list.tail.next)
}

func Test_GivenMultipleElementsList_WhenObtainingCount_ThenReturnsCorrectValue_Doubly(t *testing.T) {
	list := makeList2(1, 2, 3, 4, 5, 6, 7)

	assert.Equal(t, 7, list.Count())
	assert.NotNil(t, list.head)
	assert.NotNil(t, list.tail)
	assert.Nil(t, list.tail.next)
	assert.Nil(t, list.head.prev)
}

// INSERT (after)

func Test_GivenMiddleNode_WhenInsertingAfter_ThenNodeAppearsImmediatelyAfterIt_Doubly(t *testing.T) {
	list := makeList2(1, 2, 3, 4)
	middle := list.head.next
	newNode := Node{value: 99}

	list.Insert(middle, newNode)

	fw := toSlice(&list)
	assert.Equal(t, []int{1, 2, 99, 3, 4}, fw)
	assert.Equal(t, reverseInts(fw), toReverseSlice(&list))
	assert.Equal(t, 5, list.Count())
	assert.Equal(t, middle, list.head.next)           // still 2
	assert.Equal(t, 99, middle.next.value)            // 99 after 2
	assert.Equal(t, middle, middle.next.prev)         // back-link fixed
	assert.Equal(t, 3, middle.next.next.value)        // 3 follows 99
	assert.Equal(t, 99, middle.next.next.prev.value)  // 3.prev == 99
	assert.Nil(t, list.tail.next)
	assert.Nil(t, list.head.prev)
}

func Test_GivenTailNode_WhenInsertingAfter_ThenNodeBecomesNewTail_Doubly(t *testing.T) {
	list := makeList2(10, 20, 30)
	tail := list.tail
	newNode := Node{value: 99}

	list.Insert(tail, newNode)

	fw := toSlice(&list)
	assert.Equal(t, []int{10, 20, 30, 99}, fw)
	assert.Equal(t, reverseInts(fw), toReverseSlice(&list))
	assert.Equal(t, 4, list.Count())
	assert.Equal(t, 99, list.tail.value)
	assert.Equal(t, tail, list.tail.prev)
	assert.Nil(t, list.tail.next)
	assert.Nil(t, list.head.prev)
}

func Test_GivenHeadNode_WhenInsertingAfter_ThenNodeInsertedAsSecond_Doubly(t *testing.T) {
	list := makeList2(5, 10, 15)
	head := list.head
	newNode := Node{value: 7}

	list.Insert(head, newNode)

	fw := toSlice(&list)
	assert.Equal(t, []int{5, 7, 10, 15}, fw)
	assert.Equal(t, reverseInts(fw), toReverseSlice(&list))
	assert.Equal(t, 4, list.Count())
	assert.Equal(t, head, list.head)          // still head
	assert.Equal(t, 7, head.next.value)       // 7 after head
	assert.Equal(t, head, head.next.prev)     // 7.prev == head
	assert.Nil(t, list.tail.next)
	assert.Nil(t, list.head.prev)
}

func Test_GivenSingleNodeList_WhenInsertingAfterHead_ThenNodeAppended_Doubly(t *testing.T) {
	list := makeList2(1)
	head := list.head
	newNode := Node{value: 2}

	list.Insert(head, newNode)

	fw := toSlice(&list)
	assert.Equal(t, []int{1, 2}, fw)
	assert.Equal(t, reverseInts(fw), toReverseSlice(&list))
	assert.Equal(t, 2, list.Count())
	assert.Equal(t, 2, list.tail.value)
	assert.Equal(t, head, list.tail.prev)
	assert.Nil(t, list.tail.next)
	assert.Nil(t, list.head.prev)
}

// INSERT FIRST

func Test_GivenEmptyList_WhenInsertingFirst_ThenNodeBecomesHeadAndTail_Doubly(t *testing.T) {
	list := LinkedList2{}
	newNode := Node{value: 10}

	list.InsertFirst(newNode)

	fw := toSlice(&list)
	assert.Equal(t, []int{10}, fw)
	assert.Equal(t, reverseInts(fw), toReverseSlice(&list))
	assert.Equal(t, 1, list.Count())
	assert.Same(t, list.head, list.tail)
	assert.Nil(t, list.head.prev)
	assert.Nil(t, list.tail.next)
}

func Test_GivenNonEmptyList_WhenInsertingFirst_ThenNodePrependedToFront_Doubly(t *testing.T) {
	list := makeList2(2, 3, 4)
	newNode := Node{value: 1}

	list.InsertFirst(newNode)

	fw := toSlice(&list)
	assert.Equal(t, []int{1, 2, 3, 4}, fw)
	assert.Equal(t, reverseInts(fw), toReverseSlice(&list))
	assert.Equal(t, 4, list.Count())
	assert.Equal(t, 2, list.head.next.value)
	assert.Nil(t, list.head.prev)
	assert.Equal(t, list.head, list.head.next.prev)
	assert.Nil(t, list.tail.next)
}

func Test_GivenMultipleInsertFirstCalls_WhenRepeatedlyAdding_ThenOrderReverses_Doubly(t *testing.T) {
	list := LinkedList2{}

	list.InsertFirst(Node{value: 3})
	list.InsertFirst(Node{value: 2})
	list.InsertFirst(Node{value: 1})

	fw := toSlice(&list)
	assert.Equal(t, []int{1, 2, 3}, fw)
	assert.Equal(t, reverseInts(fw), toReverseSlice(&list))
	assert.Equal(t, 3, list.Count())
	assert.Equal(t, 3, list.tail.value)
	assert.Nil(t, list.tail.next)
	assert.Nil(t, list.head.prev)
}

// SORT

func isNonDecreasing(a []int) bool {
	for i := 1; i < len(a); i++ {
		if a[i] < a[i-1] {
			return false
		}
	}
	return true
}

func Test_GivenEmptyList_WhenSortCalled_ThenListRemainsEmpty(t *testing.T) {
	list := LinkedList2{}

	list.Sort()

	assert.Equal(t, 0, list.Count())
	assert.Equal(t, []int{}, toSlice(&list))
	assert.Nil(t, list.head)
	assert.Nil(t, list.tail)
}

func Test_GivenSingleElementList_WhenSortCalled_ThenListUnchanged(t *testing.T) {
	list := makeList2(7)

	beforeCount := list.Count()
	list.Sort()

	assert.Equal(t, beforeCount, list.Count())
	assert.Equal(t, []int{7}, toSlice(&list))
	assert.NotNil(t, list.head)
	assert.NotNil(t, list.tail)
	assert.Same(t, list.head, list.tail)
}

func Test_GivenAlreadySortedList_WhenSortCalled_ThenListUnchanged(t *testing.T) {
	list := makeList2(1, 2, 3, 4, 5)

	before := toSlice(&list)
	beforeCount := list.Count()

	list.Sort()

	after := toSlice(&list)
	assert.Equal(t, beforeCount, list.Count())
	assert.Equal(t, before, after)
	assert.True(t, isNonDecreasing(after))
}

func Test_GivenReverseSortedList_WhenSortCalled_ThenBecomesAscending(t *testing.T) {
	list := makeList2(5, 4, 3, 2, 1)

	list.Sort()

	fw := toSlice(&list)
	assert.Equal(t, []int{1, 2, 3, 4, 5}, fw)
	assert.True(t, isNonDecreasing(fw))
	assert.Equal(t, list.Count(), len(fw))
}

func Test_GivenListWithDuplicates_WhenSortCalled_ThenIsNonDecreasing(t *testing.T) {
	list := makeList2(4, 2, 3, 2, 4, 1, 1, 3)

	list.Sort()

	fw := toSlice(&list)
	assert.True(t, isNonDecreasing(fw))
	assert.Equal(t, []int{1, 1, 2, 2, 3, 3, 4, 4}, fw)
	assert.Equal(t, list.Count(), len(fw))
}

func Test_GivenListWithNegatives_WhenSortCalled_ThenAscendingAcrossSign(t *testing.T) {
	list := makeList2(0, -1, 5, -3, 2, -2)

	list.Sort()

	fw := toSlice(&list)
	assert.True(t, isNonDecreasing(fw))
	assert.Equal(t, []int{-3, -2, -1, 0, 2, 5}, fw)
	assert.Equal(t, list.Count(), len(fw))
}

func Test_GivenTwoElementsSwapped_WhenSortCalled_ThenFixed(t *testing.T) {
	list := makeList2(2, 1)

	list.Sort()

	fw := toSlice(&list)
	assert.Equal(t, []int{1, 2}, fw)
	assert.Equal(t, 2, list.Count())
}

func Test_GivenAlternatingValues_WhenSortCalled_Twice_ThenIdempotent(t *testing.T) {
	list := makeList2(3, 1, 4, 1, 5, 9, 2, 6)

	list.Sort()
	once := toSlice(&list)

	list.Sort()
	twice := toSlice(&list)

	assert.Equal(t, once, twice, "Sort should be idempotent")
	assert.True(t, isNonDecreasing(twice))
	assert.Equal(t, list.Count(), len(twice))
}

func Test_GivenAllEqualValues_WhenSortCalled_ThenStructureConsistent(t *testing.T) {
	list := makeList2(7, 7, 7, 7)

	list.Sort()

	fw := toSlice(&list)
	assert.Equal(t, []int{7, 7, 7, 7}, fw)
	assert.Equal(t, 4, list.Count())
}

// REVERSE

func Test_GivenEmptyList_WhenReverseCalled_ThenListRemainsEmpty(t *testing.T) {
	list := LinkedList2{}

	list.Reverse()

	assert.Equal(t, 0, list.Count())
	assert.Equal(t, []int{}, toSlice(&list))
	assert.Nil(t, list.head)
	assert.Nil(t, list.tail)
}

func Test_GivenSingleElementList_WhenReverseCalled_ThenListUnchanged(t *testing.T) {
	list := makeList2(42)

	beforeHead := list.head
	beforeTail := list.tail
	beforeCount := list.Count()

	list.Reverse()

	assert.Equal(t, beforeCount, list.Count())
	assert.Equal(t, []int{42}, toSlice(&list))
	assert.Same(t, beforeHead, list.head)
	assert.Same(t, beforeTail, list.tail)
	assert.Same(t, list.head, list.tail)
	assert.NotNil(t, list.tail)
	assert.Nil(t, list.tail.next)
}

func Test_GivenTwoElementsList_WhenReverseCalled_ThenOrderSwapped(t *testing.T) {
	list := makeList2(1, 2)

	list.Reverse()

	assert.Equal(t, 2, list.Count())
	assert.Equal(t, []int{2, 1}, toSlice(&list))
	assert.NotNil(t, list.head)
	assert.Equal(t, 2, list.head.value)
	assert.NotNil(t, list.tail)
	assert.Equal(t, 1, list.tail.value)
	assert.Nil(t, list.tail.next)
}

func Test_GivenOddLengthList_WhenReverseCalled_ThenOrderFullyReversed(t *testing.T) {
	list := makeList2(1, 2, 3, 4, 5)

	list.Reverse()

	assert.Equal(t, 5, list.Count())
	assert.Equal(t, []int{5, 4, 3, 2, 1}, toSlice(&list))
	assert.Equal(t, 5, list.head.value)
	assert.Equal(t, 1, list.tail.value)
	assert.Nil(t, list.tail.next)
}

func Test_GivenEvenLengthList_WhenReverseCalled_ThenOrderFullyReversed(t *testing.T) {
	list := makeList2(10, 20, 30, 40)

	list.Reverse()

	assert.Equal(t, 4, list.Count())
	assert.Equal(t, []int{40, 30, 20, 10}, toSlice(&list))
	assert.Equal(t, 40, list.head.value)
	assert.Equal(t, 10, list.tail.value)
	assert.Nil(t, list.tail.next)
}

func Test_GivenListWithDuplicates_WhenReverseCalled_ThenDuplicatesPreservedInReversedOrder(t *testing.T) {
	list := makeList2(1, 2, 2, 3, 3, 3)

	list.Reverse()

	assert.Equal(t, 6, list.Count())
	assert.Equal(t, []int{3, 3, 3, 2, 2, 1}, toSlice(&list))
	assert.Equal(t, 3, list.head.value)
	assert.Equal(t, 1, list.tail.value)
	assert.Nil(t, list.tail.next)
}

func Test_GivenListWithNegatives_WhenReverseCalled_ThenOrderFullyReversed(t *testing.T) {
	list := makeList2(-2, -1, 0, 1, 2)

	list.Reverse()

	assert.Equal(t, 5, list.Count())
	assert.Equal(t, []int{2, 1, 0, -1, -2}, toSlice(&list))
	assert.Equal(t, 2, list.head.value)
	assert.Equal(t, -2, list.tail.value)
	assert.Nil(t, list.tail.next)
}

func Test_GivenAnyList_WhenReverseCalledTwice_ThenOriginalRestored(t *testing.T) {
	original := []int{5, 1, 4, 1, 5, 9}
	list := makeList2(original...)

	list.Reverse()
	list.Reverse()

	assert.Equal(t, len(original), list.Count())
	assert.Equal(t, original, toSlice(&list))
	assert.NotNil(t, list.head)
	assert.NotNil(t, list.tail)
	assert.Nil(t, list.tail.next)
}