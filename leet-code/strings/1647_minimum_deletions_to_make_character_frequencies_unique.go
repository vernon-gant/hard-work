package strings

import (
    "sort"
)

func MinDeletions(s string) int {
    allFreqs := [26]int{}
    for _, v := range s {
        allFreqs[v - 'a'] += 1
    }
    freqs := make([]int, 0)
    for _, v := range allFreqs {
        if v == 0 {
            continue
        }
        freqs = append(freqs, v)
    }
    sort.Slice(freqs, func(i, j int) bool {
        return freqs[i] > freqs[j]
    })
    result := 0
    for i := 0; i < len(freqs) - 1; i++ {
        if freqs[i] > freqs[i+1] || (freqs[i] == 0 && freqs[i+1] == 0) {
            continue
        }
        freqs[i+1]--
        result++
        i--
    }
    return result
}