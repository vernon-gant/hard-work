from typing import List


class Solution:
    def subsets(self, nums: List[int]) -> List[List[int]]:
        n = len(nums)
        result = []
        for i in range(1 << n):
            subset = []
            for j in range(n):
                if i & (1 << j):
                    subset.append(nums[j])
            result.append(subset)
        return result


if __name__ == '__main__':
    obj = Solution()
    obj.subsets([9, 0, 1, 2, 3, 4])
