package two_pointers

import "strings"

func ReverseWords(s string) string {
    s = trimSpace(s)
    result := make([]byte,0)
    for start := len(s) - 1; start >= 0;  {
        end := start
        for ; start > 0 && s[start - 1] != ' '; start-- {}
        result = append(result, s[start : end + 1]...)
        result = append(result, ' ')
        start--
        for ; start > 0 && s[start] == ' '; start--{}
    }
    return string(result[:len(result) - 1])
}

func trimSpace(s string) string {
    start := 0
    for ; s[start] == ' '; start++ {}
    end := len(s) - 1
    for ; s[end] == ' '; end-- {}
    return s[start : end + 1]
}

func ReverseWords1(s string) string {
    s = trimSpace(s)
    
    splitString := strings.Split(s, " ")

    var words []string
    for _, str := range splitString {
        if str == "" {
            continue
        }
        words = append(words,str)
    }
    
    for i, j := 0, len(words) - 1; i < j; i, j = i+1, j-1 {
        words[i], words[j] = words[j], words[i]
    }
    
    return strings.Join(words, " ")
}