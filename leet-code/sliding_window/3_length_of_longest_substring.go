package sliding_window

func lengthOfLongestSubstring(s string) int {
    var occurences [128]int
    result, start, end, duplicates := 0, 0, 0, 0
    for ; end < len(s); end++ {
        occurences[s[end]] = occurences[s[end]] + 1
        if occurences[s[end]] == 2 {
            duplicates++
        }
        for ; duplicates > 0; start++ {
            occurences[s[start]] = occurences[s[start]] - 1
            if occurences[s[start]] == 1 {
                duplicates--
            }
        }
        result = max(result, end - start + 1)
    }
    return result
}