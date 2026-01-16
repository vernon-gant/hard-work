package depth_first

type TreeNode struct {
	Val   int
	Left  *TreeNode
	Right *TreeNode
}

func Flatten(root *TreeNode) {
	flattenRec(root, []*TreeNode{})
}

func flattenRec(root *TreeNode, right []*TreeNode) {
	if root == nil {
		return
	}
	if root.Right != nil {
		right = append(right, root.Right)
	}
	root.Right = root.Left
	root.Left = nil
	if root.Right == nil && len(right) > 0 {
		root.Right = right[len(right)-1]
		flattenRec(root.Right, right[:len(right)-1])
		return
	}
	flattenRec(root.Right, right)
}
