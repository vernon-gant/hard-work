class HeapSort:
    def __init__(self, array):
        self.HeapObject = Heap()
        for x in array:
            self.HeapObject.Add(x)

    def GetNextMax(self):
        return self.HeapObject.GetMax()


class Heap:
    def __init__(self):
        self.HeapArray = []
        self._capacity = None

    def MakeHeap(self, a, depth):
        self._capacity = 2**depth - 1
        self.HeapArray = list(a[: self._capacity])
        for i in range(len(self.HeapArray) // 2 - 1, -1, -1):
            self._sift_down(i)

    def GetMax(self):
        if not self.HeapArray:
            return -1

        max_value = self.HeapArray[0]
        last = self.HeapArray.pop()
        if self.HeapArray:
            self.HeapArray[0] = last
            self._sift_down(0)
        return max_value

    def Add(self, key):
        if self._capacity is not None and len(self.HeapArray) >= self._capacity:
            return False

        self.HeapArray.append(key)
        self._sift_up(len(self.HeapArray) - 1)
        return True

    def _sift_up(self, index):
        while index > 0:
            parent = (index - 1) // 2
            if self.HeapArray[parent] >= self.HeapArray[index]:
                break
            self.HeapArray[parent], self.HeapArray[index] = (
                self.HeapArray[index],
                self.HeapArray[parent],
            )
            index = parent

    def _sift_down(self, index):
        size = len(self.HeapArray)
        while True:
            left = 2 * index + 1
            right = 2 * index + 2
            largest = index
            if left < size and self.HeapArray[left] > self.HeapArray[largest]:
                largest = left
            if right < size and self.HeapArray[right] > self.HeapArray[largest]:
                largest = right
            if largest == index:
                break
            self.HeapArray[index], self.HeapArray[largest] = (
                self.HeapArray[largest],
                self.HeapArray[index],
            )
            index = largest
