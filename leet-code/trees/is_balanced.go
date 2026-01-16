package trees

type TreeNode struct {
	Val   int
	Left  *TreeNode
	Right *TreeNode
}

func IsBalanced(root *TreeNode) bool {
	return getBalanceFactor(root) != -1
}

func getBalanceFactor(currentNode *TreeNode) int {
	if currentNode == nil {
		return 0
	}

	leftTreeDepth := getBalanceFactor(currentNode.Left)
	if leftTreeDepth == -1 {
		return -1
	}

	rightTreeDepth := getBalanceFactor(currentNode.Right)
	if rightTreeDepth == -1 {
		return -1
	}

	if abs(leftTreeDepth-rightTreeDepth) > 1 {
		return -1
	}

	return 1 + maxNum(leftTreeDepth, rightTreeDepth)
}

func maxNum(num1, num2 int) int {
	if num1 > num2 {
		return num1
	}
	return num2
}

func abs(num int) int {
	if num < 0 {
		return -num
	}
	return num
}
