from pymonad.maybe import *
from pymonad.tools import curry


def begin():
    return Just((0, 0))


def banana():
    return Nothing


@curry(2)
def to_left(num: int, balance: tuple[int, int]):
    return (
        Nothing
        if abs((balance[0] + num) - balance[1]) > 4
        else Just((balance[0] + num, balance[1]))
    )


@curry(2)
def to_right(num: int, balance: tuple[int, int]):
    return (
        Nothing
        if abs(balance[0] - (balance[1] + num)) > 4
        else Just((balance[0], (balance[1] + num)))
    )


def show(maybe: Maybe):
    if maybe.is_nothing():
        print("Fell down")
    else:
        print(maybe.value)


show(begin().bind(to_left(2)).bind(to_right(5)).bind(to_left(-2)))
show(begin().bind(to_left(2)).bind(to_right(5)).bind(to_left(-1)))
show(
    begin()
    .bind(to_left(2))
    .bind(lambda balance: banana())
    .bind(to_right(5))
    .bind(to_left(-1))
)
