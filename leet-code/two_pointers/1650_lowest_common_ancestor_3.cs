using System;
using System.Collections.Generic;

// Definiton of a binary tree node class
// public class TreeNode
// {
//     public int data;
//     public TreeNode left;
//     public TreeNode right;
//     public TreeNode parent;

//     public TreeNode(int value)
//     {
//         this.data = value;
//         this.left = null;
//         this.right = null;
//         this.parent = null;
//     }
// }

public class Solution
{
    public TreeNode LowestCommonAncestor_HashSet(TreeNode p, TreeNode q)
    {
      var visited = new HashSet<TreeNode>();

      while (true)
      {
        if (p != null && !visited.Add(p)) return p;

        if (q != null && !visited.Add(q)) return q;

        p = p is null ? null : p.parent;
        q = q is null ? null : q.parent;
      }
    }

    public TreeNode LowestCommonAncestor_TwoPointers(TreeNode p, TreeNode q)
    {
        TreeNode first = p, second = q;

        while(first != second)
        {
            first = first.parent == null ? q : first.parent;
            second = second.parent == null ? p : second.parent;
        }

        return first;
    }
}