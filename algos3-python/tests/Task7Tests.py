import pytest

from Task7 import KthOrderStatisticsStep


def _run_to_completion(array, k):
    """Repeatedly apply the single step until the window collapses."""
    L, R = 0, len(array) - 1
    while L < R:
        L, R = KthOrderStatisticsStep(array, L, R, k)
    return array[L]


def test_GIVEN_lesson_example_WHEN_searching_zeroth_statistic_THEN_smallest_returned():
    array = [3, 5, 2, 4, 1]
    assert _run_to_completion(array, 0) == 1


def test_GIVEN_lesson_example_WHEN_searching_fourth_statistic_THEN_largest_returned():
    array = [3, 5, 2, 4, 1]
    assert _run_to_completion(array, 4) == 5


def test_GIVEN_lesson_example_WHEN_searching_middle_statistic_THEN_median_returned():
    array = [3, 5, 2, 4, 1]
    assert _run_to_completion(array, 2) == 3


def test_GIVEN_single_element_array_WHEN_searching_only_statistic_THEN_that_element_returned():
    array = [42]
    assert _run_to_completion(array, 0) == 42


def test_GIVEN_already_sorted_array_WHEN_searching_each_statistic_THEN_each_matches_sorted_position():
    for k in range(5):
        array = [1, 2, 3, 4, 5]
        assert _run_to_completion(array, k) == k + 1


def test_GIVEN_reverse_sorted_array_WHEN_searching_each_statistic_THEN_each_matches_sorted_position():
    for k in range(5):
        array = [5, 4, 3, 2, 1]
        assert _run_to_completion(array, k) == k + 1


def test_GIVEN_negative_and_positive_values_WHEN_searching_statistic_THEN_correct_value_returned():
    array = [10, -3, 7, 0, -8, 4]
    assert _run_to_completion(array, 1) == -3


def test_GIVEN_collapsed_window_WHEN_step_called_THEN_same_bounds_returned():
    array = [1, 2, 3]
    assert KthOrderStatisticsStep(array, 2, 2, 2) == [2, 2]


def test_GIVEN_pivot_lands_left_of_k_WHEN_step_called_THEN_left_bound_moves_past_pivot():
    array = [1, 2, 3, 4, 5]
    new_L, new_R = KthOrderStatisticsStep(array, 0, 4, 4)
    assert new_L > 0 and new_R == 4


def test_GIVEN_pivot_lands_right_of_k_WHEN_step_called_THEN_right_bound_moves_before_pivot():
    array = [1, 2, 3, 4, 5]
    new_L, new_R = KthOrderStatisticsStep(array, 0, 4, 0)
    assert new_L == 0 and new_R < 4


@pytest.mark.parametrize("k", range(9))
def test_GIVEN_shuffled_array_WHEN_searching_each_statistic_THEN_matches_python_sorted(k):
    original = [9, 3, 7, 1, 8, 2, 6, 4, 5]
    assert _run_to_completion(list(original), k) == sorted(original)[k]