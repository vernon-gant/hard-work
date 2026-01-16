from typing import List


class Solution:
    def fizzBuzz(self, n: int) -> List[str]:
        return list(map(lambda n: next(
            (handler[1]() for handler in [(lambda num: num % 3 == 0 and num % 5 == 0, lambda: "FizzBuzz"),
                                             (lambda num: num % 3 == 0, lambda: "Fizz"),
                                             (lambda num: num % 5 == 0, lambda: "Buzz")] if handler[0](n)), n),
                        range(1, n + 1)))