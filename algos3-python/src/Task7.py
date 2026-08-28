def KthOrderStatisticsStep(Array, L, R, k):
    pivot_index = ArrayChunk(Array, L, R)

    if pivot_index < k:
        return [pivot_index + 1, R]
    if pivot_index > k:
        return [L, pivot_index - 1]
    
    return [pivot_index, pivot_index]    
    
def ArrayChunk(M, L, R):
    pivot_index = (L + R) // 2
    N = M[pivot_index]
    i1 = L
    i2 = R

    while True:
        while M[i1] < N:
            i1 += 1
        while M[i2] > N:
            i2 -= 1

        if i1 == i2 - 1 and M[i1] > M[i2]:
            M[i1], M[i2] = M[i2], M[i1]
            pivot_index = (L + R) // 2
            N = M[pivot_index]
            i1 = 0
            i2 = len(M) - 1
            continue

        if i1 == i2:
            return pivot_index

        if i1 == pivot_index:
            pivot_index = i2
        elif i2 == pivot_index:
            pivot_index = i1
        M[i1], M[i2] = M[i2], M[i1]