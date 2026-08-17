from Task4 import ArrayChunk


def test_GIVEN_task_prompt_example_array_WHEN_array_chunk_called_THEN_array_partitioned_around_pivot_and_pivot_index_returned():
    array = [7, 5, 6, 4, 3, 1, 2]

    pivot_index = ArrayChunk(array)

    assert pivot_index == 3
    assert array == [2, 1, 3, 4, 6, 5, 7]


def test_GIVEN_already_sorted_array_WHEN_array_chunk_called_THEN_array_unchanged_and_middle_index_returned():
    array = [1, 2, 3, 4, 5]

    pivot_index = ArrayChunk(array)

    assert pivot_index == 2
    assert array == [1, 2, 3, 4, 5]


def test_GIVEN_reverse_sorted_array_WHEN_array_chunk_called_THEN_array_fully_partitioned_around_pivot():
    array = [5, 4, 3, 2, 1]

    pivot_index = ArrayChunk(array)

    assert pivot_index == 2
    assert array == [1, 2, 3, 4, 5]


def test_GIVEN_single_element_array_WHEN_array_chunk_called_THEN_array_unchanged_and_index_zero_returned():
    array = [42]

    pivot_index = ArrayChunk(array)

    assert pivot_index == 0
    assert array == [42]


def test_GIVEN_two_element_array_out_of_order_WHEN_array_chunk_called_THEN_elements_swapped_and_last_index_returned():
    array = [2, 1]

    pivot_index = ArrayChunk(array)

    assert pivot_index == 1
    assert array == [1, 2]


def test_GIVEN_two_element_array_already_ordered_WHEN_array_chunk_called_THEN_array_unchanged_and_last_index_returned():
    array = [1, 2]

    pivot_index = ArrayChunk(array)

    assert pivot_index == 1
    assert array == [1, 2]


def test_GIVEN_even_length_reverse_sorted_array_WHEN_array_chunk_called_THEN_pivot_index_uses_floor_division_midpoint():
    array = [6, 5, 4, 3, 2, 1]

    pivot_index = ArrayChunk(array)

    assert pivot_index == 3
    assert array == [1, 2, 3, 4, 5, 6]


def test_GIVEN_array_where_pivot_is_the_smallest_element_WHEN_array_chunk_called_THEN_pivot_ends_up_at_front_with_empty_left_group():
    array = [10, 20, 30, 40, 50, 5]

    pivot_index = ArrayChunk(array)

    assert pivot_index == 0
    assert array == [5, 20, 30, 10, 40, 50]
