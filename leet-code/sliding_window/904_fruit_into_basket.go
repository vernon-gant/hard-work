package sliding_window

func totalFruit(fruits []int) int {
    start, windowFreq, result := 0, make(map[int]int), 0
    for i := 0; i < len(fruits); i++ {
        windowFreq[fruits[i]] += 1
        for ;len(windowFreq) > 2; start++ {
            windowFreq[fruits[start]]--
            if windowFreq[fruits[start]] == 0 {
                delete(windowFreq, fruits[start])
            }
        }
        result = max(result, i - start + 1)
    }
    return result
}