from functools import partial

from pymonad.tools import curry


@curry(2)
def concat(a: str, b: str) -> str:
    return f"{a}, {b}"


greet = concat("Hello")
print(greet("World"))


def configurable_greet(salutation: str, separator: str, name: str, end: str) -> str:
    return f"{salutation}{separator} {name}{end}"


def first_step(salutation: str):
    def second_step(separator: str):
        def third_step(end: str):
            def fourth_step(name: str):
                return configurable_greet(salutation, separator, name, end)

            return fourth_step

        return third_step

    return second_step


final_first = first_step("Hello")(",")("!")
print(final_first("Petya"))

final_second = partial(configurable_greet, salutation="Hello", separator=",", end="!")
print(final_second(name="Petya"))
