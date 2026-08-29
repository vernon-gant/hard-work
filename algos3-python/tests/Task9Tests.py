from Task9 import HeapSort


def test_GIVEN_shuffled_array_WHEN_get_next_max_called_repeatedly_THEN_values_returned_sorted_descending():
    sorter = HeapSort([7, 2, 9, 4, 5, 1, 3, 6, 8])

    result = [sorter.GetNextMax() for _ in range(9)]

    assert result == [9, 8, 7, 6, 5, 4, 3, 2, 1]


def test_GIVEN_empty_array_WHEN_get_next_max_called_THEN_minus_one_returned():
    sorter = HeapSort([])

    assert sorter.GetNextMax() == -1


def test_GIVEN_single_element_array_WHEN_get_next_max_called_THEN_that_element_then_minus_one_returned():
    sorter = HeapSort([42])

    assert sorter.GetNextMax() == 42
    assert sorter.GetNextMax() == -1


def test_GIVEN_already_sorted_array_WHEN_get_next_max_called_repeatedly_THEN_values_returned_descending():
    sorter = HeapSort([1, 2, 3, 4, 5])

    result = [sorter.GetNextMax() for _ in range(5)]

    assert result == [5, 4, 3, 2, 1]


def test_GIVEN_reverse_sorted_array_WHEN_get_next_max_called_repeatedly_THEN_values_returned_descending():
    sorter = HeapSort([5, 4, 3, 2, 1])

    result = [sorter.GetNextMax() for _ in range(5)]

    assert result == [5, 4, 3, 2, 1]


def test_GIVEN_array_with_duplicate_values_WHEN_get_next_max_called_repeatedly_THEN_duplicates_all_returned():
    sorter = HeapSort([3, 1, 3, 2, 3])

    result = [sorter.GetNextMax() for _ in range(5)]

    assert result == [3, 3, 3, 2, 1]


def test_GIVEN_all_values_drained_WHEN_get_next_max_called_again_THEN_minus_one_returned_repeatedly():
    sorter = HeapSort([2, 1])
    sorter.GetNextMax()
    sorter.GetNextMax()

    assert sorter.GetNextMax() == -1
    assert sorter.GetNextMax() == -1


def test_GIVEN_two_heap_sort_instances_WHEN_used_independently_THEN_each_has_its_own_heap():
    first = HeapSort([1, 2, 3])
    second = HeapSort([10, 20])

    assert first.GetNextMax() == 3
    assert second.GetNextMax() == 20
    assert first.GetNextMax() == 2
    assert second.GetNextMax() == 10
