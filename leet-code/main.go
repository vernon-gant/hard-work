package main

import (
    "algos/recursion"
    "fmt"
)

func makeList(values []int) *recursion.ListNode {
    if len(values) == 0 {
        panic("No empty array")
    }

	list := &recursion.ListNode{}
    temp := list

	for _, v := range values {
        temp.Next = &recursion.ListNode{Val: v}
        temp = temp.Next
	}

	return list.Next
}

func main() {
    first := makeList([]int{9,9,9,9,9,9,9})
    second := makeList([]int{9,9,9,9})
    result := recursion.AddTwoNumbers(first, second)
    fmt.Println(result)
}
