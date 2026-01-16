package recursion

func IsBalancedParanth(expression string) bool {
	balance := 0
	return isBalancedParanth(expression,&balance)
}

func isBalancedParanth(expression string,balance * int) bool {
	if *balance < 0 {
		return false
	}
	if len(expression) == 0 {
		return *balance == 0
	}
	if expression[0] == ')' {
		*balance--
	} else if expression[0] == '(' {
		*balance++
	}
	return isBalancedParanth(expression[1:],balance)
}

func GenerateAllParanthesis(openNumber int) []string {
	result := new([]string)
	generateAllParanthesis(result,"",0,0,openNumber)
	return *result
}

func generateAllParanthesis(result * []string, expression string, open, close int, n int)  {
	if open == n && close == n {
		*result = append(*result,expression)
		return
	}
	if open < n {
		generateAllParanthesis(result,expression + "(", open + 1, close,n)
	}
	if close < open {
		generateAllParanthesis(result,expression + ")", open, close+1,n)
	}
}