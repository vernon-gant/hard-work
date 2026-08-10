def KnuthSequence(array_size):
    def knuth_rec(result):
        next = 3 * result[0] + 1
        if next >= array_size:
            return result
        else:
            return knuth_rec([next] + result)
            
    return knuth_rec([1]) if array_size > 1 else []
