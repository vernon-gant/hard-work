def InsertionSortStep(array, step, i):
    if i >= len(array) - 1:
        return

    for j in range(i + step, len(array), step):
        n = j - step
        while n >= i and array[n] > array[n + step]:
            array[n], array[n + step] = array[n + step], array[n]
            n -= step
