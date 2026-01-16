package subsets

func LetterCombinations(digits string) []string {
    if len(digits) == 0 {
        return []string{}
    }
    allLetters := digitsToLetters(digits)
    return letterCombinationsRec(allLetters, []byte{}, []string{})
}

func letterCombinationsRec(allLetters [][]byte, currentCombination []byte, result []string) []string {
    if len(allLetters) == 0 {
        return append(result, string(currentCombination))
    }
    for _, letter := range allLetters[0] {
        result = letterCombinationsRec(allLetters[1:], append(currentCombination, letter), result)
    }
    return result
}

func digitsToLetters(digits string) [][]byte {
    mappings := make([][]byte, 10)
    mappings[2] = []byte{'a', 'b', 'c'}
    mappings[3] = []byte{'d', 'e', 'f'}
    mappings[4] = []byte{'g', 'h', 'i'}
    mappings[5] = []byte{'j', 'k', 'l'}
    mappings[6] = []byte{'m', 'n', 'o'}
    mappings[7] = []byte{'p', 'q', 'r', 's'}
    mappings[8] = []byte{'t', 'u', 'v'}
    mappings[9] = []byte{'w', 'x', 'y', 'z'}
    result := make([][]byte, len(digits))
    for idx, letter := range digits {
        result[idx] = mappings[letter-'0']
    }
    return result
}
