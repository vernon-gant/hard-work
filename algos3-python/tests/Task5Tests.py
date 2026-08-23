from Task5 import QuickSort


def test_GIVEN_task4_example_array_WHEN_quick_sort_called_THEN_array_fully_sorted_ascending():
    array = [7, 5, 6, 4, 3, 1, 2]

    QuickSort(array, 0, len(array) - 1)

    assert array == [1, 2, 3, 4, 5, 6, 7]


def test_GIVEN_already_sorted_array_WHEN_quick_sort_called_THEN_array_unchanged():
    array = [1, 2, 3, 4, 5]

    QuickSort(array, 0, len(array) - 1)

    assert array == [1, 2, 3, 4, 5]


def test_GIVEN_reverse_sorted_array_WHEN_quick_sort_called_THEN_array_fully_sorted_ascending():
    array = [5, 4, 3, 2, 1]

    QuickSort(array, 0, len(array) - 1)

    assert array == [1, 2, 3, 4, 5]


def test_GIVEN_empty_array_WHEN_quick_sort_called_THEN_array_remains_empty():
    array = []

    QuickSort(array, 0, len(array) - 1)

    assert array == []


def test_GIVEN_single_element_array_WHEN_quick_sort_called_THEN_array_unchanged():
    array = [42]

    QuickSort(array, 0, len(array) - 1)

    assert array == [42]


def test_GIVEN_two_element_array_out_of_order_WHEN_quick_sort_called_THEN_elements_swapped():
    array = [2, 1]

    QuickSort(array, 0, len(array) - 1)

    assert array == [1, 2]


def test_GIVEN_larger_shuffled_array_WHEN_quick_sort_called_THEN_array_fully_sorted_ascending():
    array = [9, 3, 7, 1, 8, 2, 6, 4, 5]

    QuickSort(array, 0, len(array) - 1)

    assert array == [1, 2, 3, 4, 5, 6, 7, 8, 9]


def test_GIVEN_array_where_pivot_repeatedly_lands_at_a_boundary_WHEN_quick_sort_called_THEN_array_fully_sorted_ascending():
    array = [10, 20, 30, 40, 50, 5]

    QuickSort(array, 0, len(array) - 1)

    assert array == [5, 10, 20, 30, 40, 50]
