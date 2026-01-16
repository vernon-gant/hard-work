package strings

func EqualFrequency(word string) bool {
    freqs := [26]int{}
    for _, v := range word {
        freqs[v - 'a'] += 1
    }
    freqsCount := make(map[int]int)
    keys := make([]int, 0)
    for _, v := range freqs {
        if v == 0 {
            continue
        }
        if _, ok := freqsCount[v]; !ok {
            keys = append(keys, v)
        }
        freqsCount[v] += 1
    }

    if len(keys) == 1 && (keys[0] == 1 || freqsCount[keys[0]] == 1) {
        return true
    }

    if len(keys) != 2 {
        return false
    }

    minKey, maxKey := min(keys[0], keys[1]), max(keys[0], keys[1])
    minFreq, maxFreq := freqsCount[minKey], freqsCount[maxKey]

    if minKey == 1 && minFreq == 1 {
        return true
    }

    if maxKey - 1 == minKey && maxFreq == 1 {
        return true
    }

    return false
}