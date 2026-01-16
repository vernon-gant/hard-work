package sliding_window

func characterReplacement(s string, k int) int {
    occurences := make([]int, 26)
    start, maxFreq, result := 0, 0, 0
    for end := 0; end < len(s); end++ {
        occurences[s[end] - 'A'] = occurences[s[end] - 'A'] + 1
        maxFreq = max(maxFreq, occurences[s[end] - 'A'])
        for ;end - start + 1 > maxFreq + k; start++ {
            occurences[s[start] - 'A'] = occurences[s[start] - 'A'] - 1
        }
        result = max(result, end - start + 1)
    }
    return result
}