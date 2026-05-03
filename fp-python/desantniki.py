from typing import Tuple

from pymonad.list import ListMonad, _List
from pymonad.maybe import Just, Nothing
from pymonad.monoid import List
from pymonad.reader import Compose
from pymonad.tools import curry

# Was an interesting one. For me the challenge was more to use types which we studied and not the for loop absence - recursion is always there.
# I do not think that I used everything wisely, but at least most of the tools are covered. I thought about the state monad, but could not wire up
# it together with recursion. Myself :)


@curry(3)
def neighbors(rows, cols, cell):
    cell_row, cell_column = cell
    candidates = [
        cell,
        (cell_row - 1, cell_column),
        (cell_row + 1, cell_column),
        (cell_row, cell_column - 1),
        (cell_row, cell_column + 1),
    ]
    return ListMonad(
        *[(nr, nc) for nr, nc in candidates if 0 <= nr < rows and 0 <= nc < cols]
    )


def create_conquer_day(rows, columns):
    expander = neighbors(rows, columns)

    def expand_field(battle_field):
        return ListMonad(*set(battle_field.bind(expander).value))

    def check_completion(battle_field):
        if len(battle_field.value) == rows * columns:
            return Nothing
        return Just(battle_field)

    return Compose(expand_field).then(check_completion)


def conquer_rec(maybe_field, conquer_day, day):
    if maybe_field.is_nothing():
        return day
    next_maybe = maybe_field.bind(conquer_day)
    return conquer_rec(next_maybe, conquer_day, day + 1)


def ConquestCampaign(rows_count, columns_count, battle_field):
    initial = ListMonad(*map(lambda p: (p[0] - 1, p[1] - 1), battle_field))
    conquer_day = create_conquer_day(rows_count, columns_count)

    if len(initial.value) == rows_count * columns_count:
        return 1

    return conquer_rec(Just(initial), conquer_day, 1)
