# [26. Remove Duplicates from Sorted Array](https://leetcode.com/problems/remove-duplicates-from-sorted-array/)

## Intuition

Since the array is sorted we always know that when we start moving right say from the first number we will meet either the same number or a next unique number.
That brings us to the point, that if we keep track of a position - n, where we should insert our next unique number(position is initially 1, because the first element is unique in any array)
and iteratively pass through all "slices" of same numbers or single unique numbers, inserting only the first of them to this position n, then we will be able to build at the beginning a sub array of size n containing only unique elements.

![1](https://github.com/vernon-gant/leet-code/assets/101332387/f9faf875-7ca8-4dd7-b7d7-7c1901210f96)
![2](https://github.com/vernon-gant/leet-code/assets/101332387/99fd5900-7a32-4a4a-8763-10419d7c3867)
![3](https://github.com/vernon-gant/leet-code/assets/101332387/08c9159d-1b75-44ee-b477-37d1fd19919f)


## Approach

1. Initialize a nextInsertIdx  variable with an initial value 1 - the first number in array of any size is unique, so we can easily skip it.

```
nextInsertIdx := 1
```

2. Start a loop from i = 1 to the length of the given array(exclusive).

```
for i := 1; i < len(nums); i++ {

}
```

3. Here comes the trick: we check inside of this loop for every number at index i if a number before it with index i - 1 is not the same.
If they are not the same, that means that we found the first unique number/the only unique number for our subarray of unique numbers after our current slice of numbers/the only unique number.
If they are same we do nothing and skip it, because our approach is based on putting the FIRST unique number(be it a sequence of the same numbers of just one unique number)

```
if nums[i - 1] != nums[i] {

}
```

4. And finally if the two numbers are different we put the nums[i] at nums[nextInsertIdx] or in other words we put the unique number on its right place and increase the nextInsertIdx.

```
nums[nextInsertIdx] = nums[i]
nextInsertIdx++
```

## Complexity
- Time complexity:

O(n) as we iterate over the whole array.

- Space complexity:

O(1), we use only one extra variable


## Code
```
func RemoveDuplicates(nums []int) int {
    nextInsertIdx := 1
    for i := 1; i < len(nums); i++ {
        if nums[i - 1] != nums[i] {
            nums[nextInsertIdx] = nums[i]
            nextInsertIdx++
        }
    }
    return nextInsertIdx
}
```
