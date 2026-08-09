def SelectionSortStep(array, i):
    if i >= len(array) - 1:
        return
    min_index = i
    for j in range(i + 1, len(array)):
        if array[min_index] > array[j]:
            min_index = j
    array[i], array[min_index] = array[min_index], array[i]


def BubbleSortStep(array):
    if len(array) < 2:
        return True
    no_swaps = True
    for i in range(1, len(array)):
        if array[i - 1] > array[i]:
            no_swaps = False
            array[i - 1], array[i] = array[i], array[i - 1]
    return no_swaps
