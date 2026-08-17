def ArrayChunk(M):
    pivot_index = len(M) // 2
    N = M[pivot_index]
    i1 = 0
    i2 = len(M) - 1

    while True:
        while M[i1] < N:
            i1 += 1
        while M[i2] > N:
            i2 -= 1

        if i1 == i2 - 1 and M[i1] > M[i2]:
            M[i1], M[i2] = M[i2], M[i1]
            pivot_index = len(M) // 2
            N = M[pivot_index]
            i1 = 0
            i2 = len(M) - 1
            continue

        if i1 == i2 or (i1 == i2 - 1 and M[i1] < M[i2]):
            return pivot_index

        if i1 == pivot_index:
            pivot_index = i2
        elif i2 == pivot_index:
            pivot_index = i1
        M[i1], M[i2] = M[i2], M[i1]
