package binary_tree

type TreeNode struct {
    Val   int
    Left  *TreeNode
    Right *TreeNode
}

func maxDepth(root *TreeNode) int {
    return maxDepthRec(root, 0)
}

func maxDepthRec(root *TreeNode, currentDepth int) int {
    if root == nil {
        return currentDepth
    }
    return max(maxDepthRec(root.Left, currentDepth+1), maxDepthRec(root.Right, currentDepth+1))
}