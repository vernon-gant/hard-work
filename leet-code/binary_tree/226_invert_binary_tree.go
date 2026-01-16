package binary_tree

func invertTree(root *TreeNode) *TreeNode {
    invertTreeRec(root)
    return root
}

func invertTreeRec(root * TreeNode) {
    if root == nil {
        return
    }
    root.Left, root.Right = root.Right, root.Left
    invertTreeRec(root.Left)
    invertTreeRec(root.Right)
}