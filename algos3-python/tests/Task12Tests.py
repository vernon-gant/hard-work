import pytest

from Task12 import BinarySearch


class CountingList(list):
    """A list that records how many times a single element was read."""

    def __init__(self, values):
        super().__init__(values)
        self.reads = 0

    def __getitem__(self, key):
        if isinstance(key, int):
            self.reads += 1
        return super().__getitem__(key)


@pytest.fixture
def searcher():
    return BinarySearch([])


@pytest.mark.parametrize("target", [1, 3, 5, 7, 9, 11, 13, 15, 17, 19, 21])
def test_GIVEN_present_element_WHEN_galloping_THEN_returns_true(searcher, target):
    array = [1, 3, 5, 7, 9, 11, 13, 15, 17, 19, 21]

    assert searcher.GallopingSearch(array, target) is True


@pytest.mark.parametrize("target", [0, 2, 8, 20, 22, 1000, -7])
def test_GIVEN_absent_element_WHEN_galloping_THEN_returns_false(searcher, target):
    array = [1, 3, 5, 7, 9, 11, 13, 15, 17, 19, 21]

    assert searcher.GallopingSearch(array, target) is False


def test_GIVEN_empty_array_WHEN_galloping_THEN_returns_false(searcher):
    assert searcher.GallopingSearch([], 1) is False


@pytest.mark.parametrize("target,expected", [(42, True), (41, False), (43, False)])
def test_GIVEN_single_element_array_WHEN_galloping_THEN_matches_only_that_element(
    searcher, target, expected
):
    assert searcher.GallopingSearch([42], target) is expected


@pytest.mark.parametrize("size", [1, 2, 3, 4, 5, 6, 7, 8, 15, 16, 17, 31, 32, 33, 64])
def test_GIVEN_array_of_any_size_WHEN_every_element_searched_THEN_all_are_found(
    searcher, size
):
    array = [value * 2 for value in range(size)]

    for target in array:
        assert searcher.GallopingSearch(array, target) is True, f"missed {target}"


@pytest.mark.parametrize("size", [1, 2, 3, 4, 5, 6, 7, 8, 15, 16, 17, 31, 32, 33, 64])
def test_GIVEN_array_of_any_size_WHEN_gaps_searched_THEN_none_are_found(searcher, size):
    array = [value * 2 for value in range(size)]

    for target in [value * 2 + 1 for value in range(-1, size)]:
        assert searcher.GallopingSearch(array, target) is False, f"false hit {target}"


def test_GIVEN_first_element_WHEN_galloping_THEN_found_on_the_very_first_probe(searcher):
    array = CountingList(range(1_000_000))

    assert searcher.GallopingSearch(array, 0) is True
    assert array.reads == 1


def test_GIVEN_early_element_in_huge_array_WHEN_galloping_THEN_cost_depends_on_position(
    searcher,
):
    early_array = CountingList(range(1_000_000))
    late_array = CountingList(range(1_000_000))

    assert searcher.GallopingSearch(early_array, 100) is True
    assert searcher.GallopingSearch(late_array, 999_999) is True

    # cost scales with the element's position, not with the array length
    assert early_array.reads < late_array.reads / 2


def test_GIVEN_last_element_WHEN_galloping_THEN_still_found(searcher):
    array = list(range(1000))

    assert searcher.GallopingSearch(array, 999) is True


def test_GIVEN_target_beyond_array_WHEN_galloping_THEN_returns_false(searcher):
    array = list(range(1000))

    assert searcher.GallopingSearch(array, 5000) is False


def test_GIVEN_negative_values_WHEN_galloping_THEN_works_the_same(searcher):
    array = [-50, -30, -10, -1, 0, 4, 8]

    assert searcher.GallopingSearch(array, -30) is True
    assert searcher.GallopingSearch(array, -31) is False


def test_GIVEN_galloping_search_WHEN_called_THEN_binary_search_class_is_untouched():
    searcher = BinarySearch([1, 2, 3, 4, 5])
    searcher.GallopingSearch([10, 20, 30], 20)

    assert searcher.Left == 0
    assert searcher.Right == 4
    assert searcher.GetResult() == 0