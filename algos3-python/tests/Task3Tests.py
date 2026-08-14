from Task3 import KnuthSequence


def test_GIVEN_task_prompt_example_size_WHEN_knuth_sequence_called_THEN_descending_terms_below_size_returned():
    result = KnuthSequence(15)

    assert result == [13, 4, 1]


def test_GIVEN_empty_array_size_WHEN_knuth_sequence_called_THEN_empty_sequence_returned():
    result = KnuthSequence(0)

    assert result == []


def test_GIVEN_smallest_array_size_with_a_valid_step_WHEN_knuth_sequence_called_THEN_only_step_one_returned():
    result = KnuthSequence(2)

    assert result == [1]


def test_GIVEN_array_size_exactly_equal_to_a_sequence_term_WHEN_knuth_sequence_called_THEN_that_term_is_excluded():
    result = KnuthSequence(4)

    assert result == [1]


def test_GIVEN_array_size_one_more_than_a_sequence_term_WHEN_knuth_sequence_called_THEN_that_term_is_included():
    result = KnuthSequence(5)

    assert result == [4, 1]


def test_GIVEN_larger_array_size_spanning_multiple_terms_WHEN_knuth_sequence_called_THEN_all_terms_below_size_returned_descending():
    result = KnuthSequence(100)

    assert result == [40, 13, 4, 1]
