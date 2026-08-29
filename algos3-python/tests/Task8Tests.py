from Task8 import MergeSort


def test_GIVEN_typical_unsorted_array_WHEN_merge_sort_called_THEN_new_array_returned_sorted_ascending_and_input_untouched():
    array = [3, 5, 2, 4, 1]

    result = MergeSort(array)

    assert result == [1, 2, 3, 4, 5]
    assert array == [3, 5, 2, 4, 1]


def test_GIVEN_empty_array_WHEN_merge_sort_called_THEN_empty_array_returned():
    array = []

    result = MergeSort(array)

    assert result == []


def test_GIVEN_single_element_array_WHEN_merge_sort_called_THEN_same_single_element_returned():
    array = [7]

    result = MergeSort(array)

    assert result == [7]


def test_GIVEN_already_sorted_array_WHEN_merge_sort_called_THEN_same_order_returned():
    array = [1, 2, 3, 4, 5]

    result = MergeSort(array)

    assert result == [1, 2, 3, 4, 5]


def test_GIVEN_reverse_sorted_array_WHEN_merge_sort_called_THEN_array_sorted_ascending():
    array = [5, 4, 3, 2, 1]

    result = MergeSort(array)

    assert result == [1, 2, 3, 4, 5]


def test_GIVEN_array_with_duplicate_values_WHEN_merge_sort_called_THEN_duplicates_preserved_in_sorted_order():
    array = [3, 1, 2, 3, 1]

    result = MergeSort(array)

    assert result == [1, 1, 2, 3, 3]


def test_GIVEN_odd_length_array_WHEN_merge_sort_called_THEN_uneven_split_still_sorts_correctly():
    array = [7, 2, 9, 4, 5, 1, 3]

    result = MergeSort(array)

    assert result == [1, 2, 3, 4, 5, 7, 9]


def test_GIVEN_larger_shuffled_array_WHEN_merge_sort_called_THEN_array_fully_sorted_ascending():
    array = [9, 3, 7, 1, 8, 2, 6, 4, 5]

    result = MergeSort(array)

    assert result == [1, 2, 3, 4, 5, 6, 7, 8, 9]


def test_GIVEN_two_element_array_out_of_order_WHEN_merge_sort_called_THEN_elements_swapped():
    array = [2, 1]

    result = MergeSort(array)

    assert result == [1, 2]
