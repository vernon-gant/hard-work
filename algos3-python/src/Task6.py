def _partition_range(array, left, right):
    pivot_index = left + (right - left + 1) // 2
    N = array[pivot_index]
    i1 = left
    i2 = right
    while True:
        while array[i1] < N:
            i1 += 1
        while array[i2] > N:
            i2 -= 1
        if i1 == i2 - 1 and array[i1] > array[i2]:
            array[i1], array[i2] = array[i2], array[i1]
            pivot_index = left + (right - left + 1) // 2
            N = array[pivot_index]
            i1 = left
            i2 = right
            continue
        if i1 == i2 or (i1 == i2 - 1 and array[i1] < array[i2]):
            return pivot_index
        if i1 == pivot_index:
            pivot_index = i2
        elif i2 == pivot_index:
            pivot_index = i1
        array[i1], array[i2] = array[i2], array[i1]


def QuickSortTailOptimization(array, left, right):
    pending_ranges = []

    def sort_range(left, right):
        if left >= right:
            if not pending_ranges:
                return
            next_left, next_right = pending_ranges.pop()
            return sort_range(next_left, next_right)

        pivot_index = _partition_range(array, left, right)

        left_size = pivot_index - 1 - left
        right_size = right - (pivot_index + 1)

        if left_size < right_size:
            pending_ranges.append((left, pivot_index - 1))
            return sort_range(pivot_index + 1, right)

        pending_ranges.append((pivot_index + 1, right))
        return sort_range(left, pivot_index - 1)

    return sort_range(left, right)