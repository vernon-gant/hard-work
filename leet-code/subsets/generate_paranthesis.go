package subsets

func GenerateParenthesis(n int) []string {
    return generateParenthesisRec([]string{}, []byte{'('}, 1, 0, n * 2)    
}

func generateParenthesisRec(result []string, currentCombination []byte, leftCounter, rightCounter, maxLength int) []string {
    if leftCounter + rightCounter == maxLength {
        return append(result, string(currentCombination))
    }
    if leftCounter < maxLength / 2 {
        result = generateParenthesisRec(result, append(currentCombination, '('), leftCounter + 1, rightCounter, maxLength)
    }
    if leftCounter > rightCounter {
        result = generateParenthesisRec(result, append(currentCombination, ')'), leftCounter, rightCounter + 1, maxLength)
    }
    return result
}