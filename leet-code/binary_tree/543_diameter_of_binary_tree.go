package binary_tree

func diameterOfBinaryTree(root *TreeNode) int {
    deepestNodePath := getDeepestNodePath(root, []*TreeNode{})
    currentNodeDistance := len(deepestNodePath) - 1
    maxDiameter := currentNodeDistance
    for i := 0; i < len(deepestNodePath) - 1; i++ {
        var otherDirectionNode *TreeNode
        if deepestNodePath[i].Left != deepestNodePath[i + 1] {
            otherDirectionNode = deepestNodePath[i].Left
        } else {
            otherDirectionNode = deepestNodePath[i].Right
        }
        maxDiameter = max(maxDiameter, currentNodeDistance + getDeepestNodeDepth(otherDirectionNode, 0))
        currentNodeDistance--
    }
    return maxDiameter
}

func getDeepestNodePath(root *TreeNode, currentPath []*TreeNode) []*TreeNode {
    if root == nil {
        return currentPath
    }
    updatedPath := append(currentPath, root)
    if root.Left == nil && root.Right == nil {
        return updatedPath
    }
    return maxPath(getDeepestNodePath(root.Left, updatedPath), getDeepestNodePath(root.Right, updatedPath))
}

func getDeepestNodeDepth(root *TreeNode, currentDepth int) int {
    if root == nil {
        return currentDepth
    }
    currentDepth++
    if root.Left == nil && root.Right == nil {
        return currentDepth
    }
    return max(getDeepestNodeDepth(root.Left, currentDepth), getDeepestNodeDepth(root.Right, currentDepth))
}

func maxPath(path1, path2 []*TreeNode) []*TreeNode {
    if len(path1) >= len(path2) {
        return path1
    }
    return path2
}

// ------------------

// postree traversal with Right->left->root
func nodeRescursive(root *TreeNode, result *int) int {
	if root == nil {
		return 0
	}

    right := nodeRescursive(root.Right, result)
    left := nodeRescursive(root.Left, result)

    *result = max(*result, right+left)

    return max(left, right) + 1
}
func DiameterOfBinaryTree2(root *TreeNode) int {

    var result int
    nodeRescursive(root, &result)

    return result
}