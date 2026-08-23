import pytest

from Task5 import QuickSort
from Task6 import QuickSortTailOptimization

IMPLEMENTATIONS = [QuickSort, QuickSortTailOptimization]


@pytest.mark.parametrize("quick_sort", IMPLEMENTATIONS)
def test_GIVEN_task4_example_array_WHEN_quick_sort_called_THEN_array_fully_sorted_ascending(quick_sort):
    array = [7, 5, 6, 4, 3, 1, 2]
    quick_sort(array, 0, len(array) - 1)
    assert array == [1, 2, 3, 4, 5, 6, 7]


@pytest.mark.parametrize("quick_sort", IMPLEMENTATIONS)
def test_GIVEN_already_sorted_array_WHEN_quick_sort_called_THEN_array_unchanged(quick_sort):
    array = [1, 2, 3, 4, 5]
    quick_sort(array, 0, len(array) - 1)
    assert array == [1, 2, 3, 4, 5]


@pytest.mark.parametrize("quick_sort", IMPLEMENTATIONS)
def test_GIVEN_reverse_sorted_array_WHEN_quick_sort_called_THEN_array_fully_sorted_ascending(quick_sort):
    array = [5, 4, 3, 2, 1]
    quick_sort(array, 0, len(array) - 1)
    assert array == [1, 2, 3, 4, 5]


@pytest.mark.parametrize("quick_sort", IMPLEMENTATIONS)
def test_GIVEN_empty_array_WHEN_quick_sort_called_THEN_array_remains_empty(quick_sort):
    array = []
    quick_sort(array, 0, len(array) - 1)
    assert array == []


@pytest.mark.parametrize("quick_sort", IMPLEMENTATIONS)
def test_GIVEN_single_element_array_WHEN_quick_sort_called_THEN_array_unchanged(quick_sort):
    array = [42]
    quick_sort(array, 0, len(array) - 1)
    assert array == [42]


@pytest.mark.parametrize("quick_sort", IMPLEMENTATIONS)
def test_GIVEN_two_element_array_out_of_order_WHEN_quick_sort_called_THEN_elements_swapped(quick_sort):
    array = [2, 1]
    quick_sort(array, 0, len(array) - 1)
    assert array == [1, 2]


@pytest.mark.parametrize("quick_sort", IMPLEMENTATIONS)
def test_GIVEN_larger_shuffled_array_WHEN_quick_sort_called_THEN_array_fully_sorted_ascending(quick_sort):
    array = [9, 3, 7, 1, 8, 2, 6, 4, 5]
    quick_sort(array, 0, len(array) - 1)
    assert array == [1, 2, 3, 4, 5, 6, 7, 8, 9]


@pytest.mark.parametrize("quick_sort", IMPLEMENTATIONS)
def test_GIVEN_array_where_pivot_repeatedly_lands_at_a_boundary_WHEN_quick_sort_called_THEN_array_fully_sorted_ascending(quick_sort):
    array = [10, 20, 30, 40, 50, 5]
    quick_sort(array, 0, len(array) - 1)
    assert array == [5, 10, 20, 30, 40, 50]