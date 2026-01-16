package binary_tree

func isBalanced(root *TreeNode) bool {
    return isBalancedRec(root, 0) != -1
}

func isBalancedRec(root *TreeNode, currentDepth int) int {
    if root == nil {
        return currentDepth
    }
    leftDepth := isBalancedRec(root.Left, currentDepth + 1)
    rightDepth := isBalancedRec(root.Right, currentDepth + 1)
    if abs(leftDepth, rightDepth) < 2 {
        return -1
    }
    return rightDepth
}

func abs(num1, num2 int) int {
    result := num1 - num2
    if result < 0 {
        return -result
    }
    return result
}