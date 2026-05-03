from functools import reduce


def second_greatest(nums):
    if len(nums) < 2:
        raise Exception()

    def compute(acc, num):
        second, first = acc
        if num >= first:
            return (first, num)
        if num > second:
            return (num, first)
        return acc

    return reduce(
        compute,
        nums[2:],
        (min(nums[0], nums[1]), max(nums[0], nums[1])),
    )[0]
