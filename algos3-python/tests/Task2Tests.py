from Task2 import InsertionSortStep


def test_GIVEN_teacher_worked_example_WHEN_insertion_sort_step_applied_sequentially_for_each_index_THEN_array_matches_each_traced_state():
    array = [7, 6, 5, 4, 3, 2, 1]

    InsertionSortStep(array, 3, 0)
    assert array == [1, 6, 5, 4, 3, 2, 7]

    InsertionSortStep(array, 3, 1)
    assert array == [1, 3, 5, 4, 6, 2, 7]

    InsertionSortStep(array, 3, 2)
    assert array == [1, 3, 2, 4, 6, 5, 7]

    InsertionSortStep(array, 3, 3)
    assert array == [1, 3, 2, 4, 6, 5, 7]


def test_GIVEN_task_prompt_example_array_WHEN_insertion_sort_step_called_with_step_three_at_index_one_THEN_gapped_tail_fully_sorted():
    array = [1, 6, 5, 4, 3, 2, 7]

    InsertionSortStep(array, 3, 1)

    assert array == [1, 3, 5, 4, 6, 2, 7]


def test_GIVEN_unit_step_with_multi_element_forward_tail_WHEN_insertion_sort_step_called_THEN_entire_tail_sorted_ascending():
    array = [1, 3, 5, 7, 2, 6]

    InsertionSortStep(array, 1, 3)

    assert array == [1, 3, 5, 2, 6, 7]


def test_GIVEN_reverse_sorted_forward_tail_WHEN_insertion_sort_step_called_THEN_tail_fully_sorted_and_elements_before_i_untouched():
    array = [5, 4, 3, 2, 1]

    InsertionSortStep(array, 1, 1)

    assert array == [5, 1, 2, 3, 4]


def test_GIVEN_already_sorted_tail_WHEN_insertion_sort_step_called_THEN_array_unchanged():
    array = [1, 2, 3, 4, 5]

    InsertionSortStep(array, 1, 2)

    assert array == [1, 2, 3, 4, 5]


def test_GIVEN_duplicate_values_in_forward_tail_WHEN_insertion_sort_step_called_THEN_minimum_moved_to_front_and_duplicates_left_equal():
    array = [5, 3, 3, 3, 1]

    InsertionSortStep(array, 1, 1)

    assert array == [5, 1, 3, 3, 3]


def test_GIVEN_minimal_two_element_array_WHEN_insertion_sort_step_called_at_index_zero_THEN_out_of_order_pair_swapped():
    array = [3, 1]

    InsertionSortStep(array, 1, 0)

    assert array == [1, 3]


def test_GIVEN_unit_step_WHEN_insertion_sort_step_called_at_last_valid_index_THEN_only_trailing_pair_sorted():
    array = [1, 2, 3, 0]

    InsertionSortStep(array, 1, 2)

    assert array == [1, 2, 0, 3]


def test_GIVEN_step_greater_than_one_at_index_zero_WHEN_insertion_sort_step_called_THEN_only_gapped_pair_is_affected():
    array = [9, 1, 5, 2]

    InsertionSortStep(array, 2, 0)

    assert array == [5, 1, 9, 2]
