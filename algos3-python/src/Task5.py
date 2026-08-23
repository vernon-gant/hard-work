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


def QuickSort(array, left, right):
    if left >= right:
        return
    n = _partition_range(array, left, right)
    QuickSort(array, left, n - 1)
    QuickSort(array, n + 1, right)