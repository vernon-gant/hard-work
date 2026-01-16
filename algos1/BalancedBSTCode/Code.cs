using System;
using System.Collections.Generic;

using Microsoft.VisualBasic;

namespace AlgorithmsDataStructures2
{
    public static class BalancedBST
    {
        public static int[] GenerateBBSTArray(int[] a)
        {
            Array.Sort(a);
            int[] balancedTree = new int[a.Length];

            return GenerateSubTree(a, balancedTree, 0);
        }

        private static int[] GenerateSubTree(int[] source, int[] dest, int nextIdx)
        {
            int midIdx = source.Length / 2;
            dest[nextIdx] = source[midIdx];

            if (source.Length == 1) return dest;

            int[] leftSubTree = new int[source.Length / 2];
            Array.Copy(source, leftSubTree, leftSubTree.Length);
            int[] rightSubTree = new int[source.Length / 2];
            Array.Copy(source, (source.Length / 2) + 1, rightSubTree, 0, rightSubTree.Length);
            dest = GenerateSubTree(leftSubTree, dest, nextIdx * 2 + 1);
            dest = GenerateSubTree(rightSubTree, dest, nextIdx * 2 + 2);

            return dest;
        }
    }
}