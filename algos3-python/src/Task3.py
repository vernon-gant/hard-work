def KnuthSequence(array_size):
    def knuth_rec(result):
        next_step = 3 * result[0] + 1
        return result if next_step >= array_size else knuth_rec([next_step] + result)

    return knuth_rec([1]) if array_size > 0 else []