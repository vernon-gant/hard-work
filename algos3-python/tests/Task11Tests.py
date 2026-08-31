import pytest

from Task11 import BinarySearch

MAX_STEPS = 100


def run_until_finished(searcher, target):
    """Drive Step() until the search reports a result. Returns the step count."""
    for step_count in range(1, MAX_STEPS + 1):
        searcher.Step(target)
        if searcher.GetResult() != 0:
            return step_count
    pytest.fail(f"search for {target} did not finish within {MAX_STEPS} steps")


def test_GIVEN_sorted_array_WHEN_constructed_THEN_boundaries_span_whole_array():
    searcher = BinarySearch([10, 20, 30, 40, 50])

    assert searcher.Left == 0
    assert searcher.Right == 4


def test_GIVEN_fresh_searcher_WHEN_no_step_taken_THEN_result_is_in_progress():
    assert BinarySearch([10, 20, 30]).GetResult() == 0


def test_GIVEN_99_elements_WHEN_target_below_middle_THEN_range_becomes_left_half():
    searcher = BinarySearch(list(range(99)))

    searcher.Step(10)

    assert searcher.Left == 0
    assert searcher.Right == 48


def test_GIVEN_99_elements_WHEN_target_above_middle_THEN_range_becomes_right_half():
    searcher = BinarySearch(list(range(99)))

    searcher.Step(90)

    assert searcher.Left == 50
    assert searcher.Right == 98


def test_GIVEN_middle_element_WHEN_first_step_THEN_found_immediately():
    searcher = BinarySearch(list(range(99)))

    searcher.Step(49)

    assert searcher.GetResult() == 1


@pytest.mark.parametrize("target", [1, 3, 5, 7, 9, 11, 13, 15, 17])
def test_GIVEN_present_element_WHEN_search_runs_THEN_result_is_found(target):
    searcher = BinarySearch([1, 3, 5, 7, 9, 11, 13, 15, 17])

    run_until_finished(searcher, target)

    assert searcher.GetResult() == 1


@pytest.mark.parametrize("target", [0, 2, 8, 16, 18, 100])
def test_GIVEN_absent_element_WHEN_search_runs_THEN_result_is_not_found(target):
    searcher = BinarySearch([1, 3, 5, 7, 9, 11, 13, 15, 17])

    run_until_finished(searcher, target)

    assert searcher.GetResult() == -1


@pytest.mark.parametrize("size", [1, 2, 3, 4, 5, 8, 9, 16, 17, 33, 64])
def test_GIVEN_array_of_any_size_WHEN_every_element_searched_THEN_all_are_found(size):
    array = [value * 2 for value in range(size)]

    for target in array:
        searcher = BinarySearch(array)
        run_until_finished(searcher, target)
        assert searcher.GetResult() == 1, f"missed {target} in size {size}"


@pytest.mark.parametrize("size", [1, 2, 3, 4, 5, 8, 9, 16, 17, 33, 64])
def test_GIVEN_array_of_any_size_WHEN_gaps_searched_THEN_none_are_found(size):
    array = [value * 2 for value in range(size)]

    for target in [value * 2 + 1 for value in range(-1, size)]:
        searcher = BinarySearch(array)
        run_until_finished(searcher, target)
        assert searcher.GetResult() == -1, f"false hit on {target} in size {size}"


def test_GIVEN_empty_array_WHEN_step_taken_THEN_result_is_not_found():
    searcher = BinarySearch([])

    run_until_finished(searcher, 42)

    assert searcher.GetResult() == -1


def test_GIVEN_finished_search_WHEN_more_steps_taken_THEN_nothing_changes():
    searcher = BinarySearch([1, 3, 5, 7, 9])
    run_until_finished(searcher, 7)
    boundaries_after_finish = (searcher.Left, searcher.Right)

    for _ in range(5):
        searcher.Step(7)

    assert (searcher.Left, searcher.Right) == boundaries_after_finish
    assert searcher.GetResult() == 1


def test_GIVEN_large_array_WHEN_searching_THEN_step_count_is_logarithmic():
    array = list(range(1024))
    searcher = BinarySearch(array)

    step_count = run_until_finished(searcher, 1023)

    assert step_count <= 12, f"took {step_count} steps -- not a halving search"