"""
Tests for the "longest strictly increasing subsequence" exercise.

Contract you are implementing in Task13.py:

    def longest_increasing_subsequence(array): -> list

    Returns the longest strictly increasing SUBSEQUENCE of `array`
    (elements keep their relative order but need not be adjacent).
    If several subsequences share the maximum length, ANY of them is
    accepted -- these tests check validity + length, not identity.
"""

import time

import pytest

from Task13 import longest_increasing_subsequence


def is_subsequence(candidate, array):
    """True if `candidate` can be obtained from `array` by deleting elements."""
    position = 0
    for value in array:
        if position < len(candidate) and candidate[position] == value:
            position += 1
    return position == len(candidate)


def is_strictly_increasing(candidate):
    return all(
        earlier < later for earlier, later in zip(candidate, candidate[1:])
    )


def assert_valid_answer(array, result, expected_length):
    assert isinstance(result, list), f"expected a list, got {type(result).__name__}"
    assert is_strictly_increasing(result), f"{result} is not strictly increasing"
    assert is_subsequence(result, array), f"{result} is not a subsequence of {array}"
    assert len(result) == expected_length, (
        f"expected length {expected_length}, got {len(result)}: {result}"
    )


def test_GIVEN_the_example_from_the_task_WHEN_solved_THEN_returns_expected_sequence():
    array = [7, 1, 2, 3, 0, 4, 5, 6, 5]

    result = longest_increasing_subsequence(array)

    assert result == [1, 2, 3, 4, 5, 6]


@pytest.mark.parametrize(
    "array,expected_length",
    [
        ([], 0),
        ([42], 1),
        ([1, 2, 3, 4, 5], 5),
        ([5, 4, 3, 2, 1], 1),
        ([2, 2, 2, 2], 1),
        ([1, 1, 2, 2, 3, 3], 3),
        ([10, 9, 2, 5, 3, 7, 101, 18], 4),
        ([0, 1, 0, 3, 2, 3], 4),
        ([3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5], 4),
        ([-5, -3, -10, -1, 0, -2, 7], 5),
        ([1, 3, 2, 4, 3, 5, 4, 6], 5),
    ],
)
def test_GIVEN_various_arrays_WHEN_solved_THEN_answer_is_valid_and_maximal(
    array, expected_length
):
    assert_valid_answer(array, longest_increasing_subsequence(array), expected_length)


def test_GIVEN_strictly_monotonic_requirement_WHEN_plateau_present_THEN_no_equal_pair():
    array = [1, 5, 5, 5, 5, 5, 9]

    result = longest_increasing_subsequence(array)

    assert_valid_answer(array, result, 3)


def test_GIVEN_input_array_WHEN_solved_THEN_input_is_not_mutated():
    array = [7, 1, 2, 3, 0, 4, 5, 6, 5]
    original = list(array)

    longest_increasing_subsequence(array)

    assert array == original


def build_descending_blocks(block_count, block_size):
    """block_count descending runs, each run larger than the previous one.

    One element can be taken from each block and no more, so the longest
    increasing subsequence is exactly `block_count` elements long.
    """
    array = []
    for block_index in range(block_count):
        base = block_index * block_size
        array.extend(range(base + block_size, base, -1))
    return array


@pytest.mark.parametrize(
    "block_count,block_size", [(1, 1), (5, 1), (1, 5), (7, 4), (50, 3)]
)
def test_GIVEN_blocks_with_known_answer_WHEN_solved_THEN_length_matches(
    block_count, block_size
):
    array = build_descending_blocks(block_count, block_size)

    assert_valid_answer(
        array, longest_increasing_subsequence(array), block_count
    )


def test_GIVEN_50k_elements_WHEN_solved_THEN_finishes_quickly():
    """A quadratic solution will not finish this in time."""
    array = build_descending_blocks(5000, 10)

    started = time.perf_counter()
    result = longest_increasing_subsequence(array)
    elapsed = time.perf_counter() - started

    assert_valid_answer(array, result, 5000)
    assert elapsed < 5.0, f"took {elapsed:.1f}s -- looks worse than O(n log n)"