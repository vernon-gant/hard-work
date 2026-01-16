package sliding_window

func findRepeatedDnaSequences(s string) []string {
    if len(s) <= 10 {
        return []string{}
    }
	encoding := make(map[byte]int)
    encoding['A'] = 0
    encoding['C'] = 1
    encoding['G'] = 2
    encoding['T'] = 3
    occurences, result := make(map[int]int), make([]string, 0)
    current, remover := 0, ^(3 << 18)
    for i := 0; i < 10; i++ {
        current <<= 2;
        current |= encoding[s[i]]
    }
    occurences[current] = 1
    for i := 1; i <= len(s) - 10; i++ {
        current &= remover
        current <<= 2
        current |= encoding[s[i + 9]]
        value, _ := occurences[current]
        value += 1
        if value == 2 {
            result = append(result, s[i : i + 10])
        }
        occurences[current] = value
    }
    return result
}
