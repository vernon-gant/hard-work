from Task1 import BubbleSortStep, SelectionSortStep


def test_GIVEN_typical_array_WHEN_selection_sort_step_called_THEN_minimum_from_remainder_swapped_into_position_i():
    array = [5, 3, 8, 1, 4]

    SelectionSortStep(array, 1)

    assert array == [5, 1, 8, 3, 4]


def test_GIVEN_empty_array_WHEN_selection_sort_step_called_THEN_array_remains_empty():
    array = []

    SelectionSortStep(array, 0)

    assert array == []


def test_GIVEN_single_element_array_WHEN_selection_sort_step_called_at_index_zero_THEN_array_unchanged():
    array = [42]

    SelectionSortStep(array, 0)

    assert array == [42]


def test_GIVEN_already_sorted_array_WHEN_selection_sort_step_called_THEN_array_unchanged():
    array = [1, 2, 3, 4, 5]

    SelectionSortStep(array, 0)

    assert array == [1, 2, 3, 4, 5]


def test_GIVEN_reverse_sorted_array_WHEN_selection_sort_step_called_at_index_zero_THEN_minimum_moved_to_front():
    array = [5, 4, 3, 2, 1]

    SelectionSortStep(array, 0)

    assert array == [1, 4, 3, 2, 5]


def test_GIVEN_array_with_duplicate_minimums_WHEN_selection_sort_step_called_THEN_first_occurrence_of_minimum_swapped():
    array = [3, 1, 2, 1, 5]

    SelectionSortStep(array, 0)

    assert array == [1, 3, 2, 1, 5]


def test_GIVEN_typical_array_WHEN_selection_sort_step_called_at_last_valid_index_THEN_array_unchanged():
    array = [3, 1, 2, 4, 0]

    SelectionSortStep(array, len(array) - 1)

    assert array == [3, 1, 2, 4, 0]


def test_GIVEN_unsorted_array_with_swaps_needed_WHEN_bubble_sort_step_called_THEN_adjacent_out_of_order_elements_swapped_and_false_returned():
    array = [3, 1, 2]

    result = BubbleSortStep(array)

    assert array == [1, 2, 3]
    assert result is False


def test_GIVEN_already_sorted_array_WHEN_bubble_sort_step_called_THEN_no_swaps_performed_and_true_returned():
    array = [1, 2, 3, 4, 5]

    result = BubbleSortStep(array)

    assert array == [1, 2, 3, 4, 5]
    assert result is True


def test_GIVEN_reverse_sorted_array_WHEN_bubble_sort_step_called_THEN_largest_element_bubbles_to_end_and_false_returned():
    array = [5, 4, 3, 2, 1]

    result = BubbleSortStep(array)

    assert array == [4, 3, 2, 1, 5]
    assert result is False


def test_GIVEN_empty_array_WHEN_bubble_sort_step_called_THEN_true_returned_and_array_remains_empty():
    array = []

    result = BubbleSortStep(array)

    assert array == []
    assert result is True


def test_GIVEN_single_element_array_WHEN_bubble_sort_step_called_THEN_true_returned_and_array_unchanged():
    array = [7]

    result = BubbleSortStep(array)

    assert array == [7]
    assert result is True


def test_GIVEN_array_with_duplicate_elements_WHEN_bubble_sort_step_called_THEN_equal_adjacent_elements_not_swapped():
    array = [2, 2, 2]

    result = BubbleSortStep(array)

    assert array == [2, 2, 2]
    assert result is True
