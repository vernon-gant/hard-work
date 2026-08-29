def MergeSort(array):
    if len(array) <= 1:
        return array

    middle = len(array) // 2
    left = MergeSort(array[:middle])
    right = MergeSort(array[middle:])

    return Merge(left, right)


def Merge(left, right):
    result, idx = [None] * (len(left) + len(right)), 0
    left_pointer, rigth_pointer = 0, 0

    while left_pointer < len(left) and rigth_pointer < len(right):
        if left[left_pointer] <= right[rigth_pointer]:
            result[idx] = left[left_pointer]
            left_pointer += 1
        else:
            result[idx] = right[rigth_pointer]
            rigth_pointer += 1
        idx += 1

    while left_pointer < len(left):
        result[idx] = left[left_pointer]
        left_pointer += 1
        idx += 1

    while rigth_pointer < len(right):
        result[idx] = right[rigth_pointer]
        rigth_pointer += 1
        idx += 1

    return result
