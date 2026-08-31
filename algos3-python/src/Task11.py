SEARCH_IN_PROGRESS = 0
SEARCH_FOUND = 1
SEARCH_NOT_FOUND = -1


class BinarySearch:
    def __init__(self, array):
        self.array = array
        self.Left = 0
        self.Right = len(array) - 1
        self.Progress = SEARCH_IN_PROGRESS if len(array) > 0 else SEARCH_NOT_FOUND

    def Step(self, N):
        if self.Progress != SEARCH_IN_PROGRESS:
            return

        middle = (self.Right + self.Left) // 2

        if self.array[middle] == N:
            self.Progress = SEARCH_FOUND
            return
        elif self.array[middle] < N:
            self.Left = middle + 1
        else:
            self.Right = middle - 1

        if self.Left > self.Right:
            self.Progress = SEARCH_NOT_FOUND
        elif self.Right - self.Left <= 1:
            remaining = self.array[self.Left:self.Right + 1]
            self.Progress = SEARCH_FOUND if N in remaining else SEARCH_NOT_FOUND

    def GetResult(self):
        return self.Progress