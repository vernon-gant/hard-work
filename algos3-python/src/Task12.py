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

    def GallopingSearch(self, array, N):
        if len(array) == 0:
            return False
        
        last_index, power = len(array) - 1, 1
        current_index, previous_index = 2 ** power - 2, -1

        while True:
            current_value = array[current_index]
            
            if current_value == N:
                return True
                
            if current_value > N:
                break

            previous_index = current_index
            power += 1
            next_index = 2 ** power - 2
            
            if next_index < last_index:
                current_index = next_index
                continue
            
            current_index = last_index
            break

        lower_bound = previous_index + 1
        upper_bound = current_index
        
        if lower_bound > upper_bound:
            return False

        searcher = BinarySearch(array)
        searcher.Left = lower_bound
        searcher.Right = upper_bound
        
        while searcher.GetResult() == SEARCH_IN_PROGRESS:
            searcher.Step(N)

        return searcher.GetResult() == SEARCH_FOUND