package stack

import (
	"cmp"
	"errors"
	"strconv"
	"strings"
)

/*
* 4. Stack - task number 4 - determine whether brackets sequence is balanced or not.
*
* For some magical reason I could do it from the first time, but okay it was an easy task on leetcode, we even do not need to create an array of brackets, just filling it with 0 is also okay,
* because we only care about the matching amount of opening|closing and that no closing comes before having opening.
 */

func IsBalanced(input string) bool {
	st := Stack[byte]{}
	for _, v := range input {
		if v == '(' {
			st.Push(0)
		} else if st.Size() == 0 {
			return false
		} else {
			st.Pop()
		}
	}
	return st.Size() == 0
}

/*
* 4. Stack - task number 5 - determine whether brackets sequence is balanced or not.
*
* Same ideas as in the example above, but for convenienve we store the closing to opening brackets mapping so that when we come to the closing bracket, we check that the peeked from the stack
* bracket is the needed one. Same rule for no closing before opening appplies.
 */

func IsBalancedMultiple(input string) bool {
	st := Stack[byte]{}

	mappings := make(map[byte]byte)
	mappings[')'] = '('
	mappings[']'] = '['
	mappings['}'] = '{'

	for _, v := range input {
		if v == '(' || v == '[' || v == '{' {
			st.Push(byte(v))
		} else if st.Size() == 0 {
			return false
		} else if top, _ := st.Peek(); top != mappings[byte(v)] {
			return false
		} else {
			st.Pop()
		}
	}

	return st.Size() == 0
}

/*
* 4. Stack - task number 6 + 7 - min for O(1) and average of the stack for O(1)
*
* Very interesting task with the GetMinimum which I could not understand how to solve, but the got the idea - at the beginning I though that we need to somehow do it in a minHeap fashion
* that after each removal we need to restructure something or whatever. But simply storing the minimum is the solution. We do not care about the ordering of all items in the stack, only
* about the minimal elements. And if we have 10 elements in the stack, but the minimal element was added in the second push, then we will just have [_, min, min, 6 more `min` ] in the stack
* of minimal elements. Then when we do POP - we do not need to restructure anything, just pop together from the min stack. And we always know that it is minimum, because if there would be an
* element less then current peeked min, we would change it!
*
* 7th task was to add an average method, in go there is no constraint for addable, there is just nothing. After static abstract methods and generic math was added, I do not know how these
* guys program with go. Because I would just constraint the T to summable and dividable or just number and that's it. And then keep the sum and at every PUSH add to it and then in the query
* divide by count. But it is golang... Of course we can do here the same, but the idea of the example here is clear and I was dispapointed...
 */

type MinStack[T cmp.Ordered] struct {
	head  *Node[T]
	min   *Node[T]
	count int
	sum   int
}

func (st *MinStack[T]) Size() int {
	return st.count
}

func (st *MinStack[T]) Pop() (T, error) {
	var result T
	if st.Size() == 0 {
		return result, errors.New("empty stack")
	}
	result = st.head.value
	st.head = st.head.next
	st.min = st.min.next
	st.count--
	return result, nil
}

func (st *MinStack[T]) Push(itm T) {
	newTop := Node[T]{value: itm, next: st.head}
	st.head = &newTop

	if st.min == nil {
		st.min = &Node[T]{value: itm}
	} else if itm < st.min.value {
		newMin := Node[T]{value: itm, next: st.min}
		st.min = &newMin
	} else {
		newMin := Node[T]{value: st.min.value, next: st.min}
		st.min = &newMin
	}

	st.count++
}

func (st *MinStack[T]) GetMin() (T, error) {
	var result T
	if st.Size() == 0 {
		return result, errors.New("empty stack")
	}
	return st.min.value, nil
}

/*
* 4. Stack - task number 8 - postfix expression solution
*
* The solution was prettye straightforward, because it was in the task description :) I just assumed that = must be at the end and then just brought words from the description into code.
* We do not need an explicit if for "=" because we can simply skip it in the first loop.
*/

func Postfix(expression string) int {
	tokens, exprStack, valueStack := strings.Fields(expression), Stack[string]{}, Stack[int]{}

	for i := len(tokens) - 2; i >= 0; i-- {
		exprStack.Push(tokens[i])
	}

	for exprStack.Size() > 0 {
		token, _ := exprStack.Pop()

		if num, err := strconv.Atoi(token); err == nil {
			valueStack.Push(num)
		} else {
			right, _ := valueStack.Pop()
			left, _ := valueStack.Pop()
			valueStack.Push(applyOp(left, right, token))
		}
	}

	result, _ := valueStack.Pop()
	return result
}

func applyOp(left, right int, op string) int {
	switch op {
	case "+":
		return left + right
	case "-":
		return left - right
	case "*":
		return left * right
	case "/":
		return left / right
	}
	return 0
}