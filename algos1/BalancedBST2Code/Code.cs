using System;
using System.Collections.Generic;

namespace AlgorithmsDataStructures2
{
    public class BSTNode
    {
        public int NodeKey; // ключ узла
        public BSTNode Parent; // родитель или null для корня
        public BSTNode LeftChild; // левый потомок
        public BSTNode RightChild; // правый потомок
        public int Level; // глубина узла

        public BSTNode(int key, BSTNode parent)
        {
            NodeKey = key;
            Parent = parent;
            LeftChild = null;
            RightChild = null;
        }
    }


    public class BalancedBST
    {
        public BSTNode Root; // корень дерева

        public BalancedBST()
        {
            Root = null;
        }

        public void AddKeyMain(int key)
        {
            if (Root == null)
            {
                Root = new BSTNode(key, null) { Level = 0 };

                return;
            }

            AddKey(Root, Root, key);
        }

        private void AddKey(BSTNode currentNode, BSTNode parentNode, int key)
        {
            if (currentNode == null)
            {
                BSTNode newNode = new BSTNode(key, parentNode) { Level = parentNode.Level + 1 };

                if (key < parentNode.NodeKey) parentNode.LeftChild = newNode;
                else parentNode.RightChild = newNode;

                return;
            }

            AddKey(key < currentNode.NodeKey ? currentNode.LeftChild : currentNode.RightChild, currentNode, key);
        }

        public void GenerateTree(int[] a)
        {
            Array.Sort(a);
            Root = GenerateTree(null, a);
        }

        private BSTNode GenerateTree(BSTNode parent, int[] a)
        {
            int midIdx = a.Length / 2;

            BSTNode currentRoot = new BSTNode(a[midIdx], parent)
            {
                Level = parent == null ? 0 : parent.Level + 1
            };

            if (a.Length == 1) return currentRoot;

            int[] leftSubTree = new int[midIdx];
            Array.Copy(a, leftSubTree, leftSubTree.Length);
            currentRoot.LeftChild = GenerateTree(currentRoot, leftSubTree);

            // If there are only two elements in the subarray, the midIdx will be the last element
            // and incrementing it will cause an IndexOutOfRangeException
            if (midIdx != a.Length - 1)
            {
                int[] rightSubTree = new int[a.Length - midIdx - 1];
                Array.Copy(a, midIdx + 1, rightSubTree, 0, rightSubTree.Length);
                currentRoot.RightChild = GenerateTree(currentRoot, rightSubTree);
            }
            return currentRoot;
        }

        public bool IsBalanced(BSTNode root_node)
        {
            return Math.Abs(GetNodeBalanceFactor(root_node)) <= 1;
        }

        private int GetNodeBalanceFactor(BSTNode currentNode)
        {
            if (currentNode == null) return 0;

            int leftTreeDepth = GetNodeBalanceFactor(currentNode.LeftChild);
            int rightTreeDepth = GetNodeBalanceFactor(currentNode.RightChild);
            int balanceFactor = leftTreeDepth - rightTreeDepth;

            return currentNode == Root ? balanceFactor : 1 + Math.Max(rightTreeDepth, leftTreeDepth);
        }

        private int GetNodeBalanceFactorWithLevel(BSTNode currentNode)
        {
            if (currentNode == null) return -1;

            int leftTreeMaxDepth = Math.Max(currentNode.Level, GetNodeBalanceFactorWithLevel(currentNode.LeftChild));
            int rightTreeMaxDepth = Math.Max(currentNode.Level, GetNodeBalanceFactorWithLevel(currentNode.RightChild));
            int balanceFactor = leftTreeMaxDepth - rightTreeMaxDepth;

            return currentNode == Root ? balanceFactor : Math.Max(leftTreeMaxDepth, rightTreeMaxDepth);
        }
    }
}