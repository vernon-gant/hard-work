import pytest

from Task10 import ksort

def test_GIVEN_new_ksort_WHEN_constructed_THEN_items_has_800_null_slots():
    sorter = ksort()

    assert len(sorter.items) == 800
    assert all(slot is None for slot in sorter.items)


@pytest.mark.parametrize(
    "value,expected_index",
    [
        ("a00", 0),
        ("a01", 1),
        ("a99", 99),
        ("b00", 100),
        ("b64", 164),
        ("g99", 699),
        ("h00", 700),
        ("h99", 799),
    ],
)
def test_GIVEN_valid_string_WHEN_index_THEN_returns_position_in_lexicographic_order(
    value, expected_index
):
    assert ksort().index(value) == expected_index


@pytest.mark.parametrize(
    "invalid_value",
    [
        "",
        "a1",
        "a001",
        "i01",
        "A01",
        "z99",
        "a1b",
        "aa1",
        "0a1",
        " a1",
        "a 1",
        "a-1",
        "a\u00b21",
        None,
        123,
        ["a", "0", "1"],
    ],
)
def test_GIVEN_malformed_input_WHEN_index_THEN_returns_minus_one(invalid_value):
    assert ksort().index(invalid_value) == -1


def test_GIVEN_valid_string_WHEN_add_THEN_returns_true_and_stores_it_at_its_index():
    sorter = ksort()

    assert sorter.add("b64") is True
    assert sorter.items[164] == "b64"
    assert sum(slot is not None for slot in sorter.items) == 1


@pytest.mark.parametrize("invalid_value", ["", "i01", "A01", "a001", None, 7])
def test_GIVEN_malformed_input_WHEN_add_THEN_returns_false_and_items_unchanged(
    invalid_value,
):
    sorter = ksort()

    assert sorter.add(invalid_value) is False
    assert all(slot is None for slot in sorter.items)


def test_GIVEN_unordered_strings_WHEN_all_added_THEN_items_holds_them_in_ascending_order():
    unordered = ["g99", "a01", "h00", "b64", "a00", "c50"]
    sorter = ksort()

    for value in unordered:
        sorter.add(value)

    stored = [slot for slot in sorter.items if slot is not None]
    assert stored == sorted(unordered)


def test_GIVEN_every_possible_string_WHEN_all_added_THEN_items_is_completely_filled():
    all_strings = [
        f"{letter}{code:02d}" for letter in "abcdefgh" for code in range(100)
    ]
    sorter = ksort()

    for value in all_strings:
        assert sorter.add(value) is True

    assert sorter.items == sorted(all_strings)