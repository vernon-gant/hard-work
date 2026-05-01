from pymonad.list import ListMonad
from pymonad.maybe import Just, Maybe, Nothing
from pymonad.tools import curry


@curry(2)
def add(x, y):
    return x + y


def add10(functor):
    return functor.map(add(10))


print(add10(Just(100)))
print(add10(ListMonad(1, 2, 3, 4, 5)))
