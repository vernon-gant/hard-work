using System;
using System.Collections.Generic;

namespace AlgorithmsDataStructures2
{
    public class aBST
    {
        public int?[] Tree;

        public aBST(int depth)
        {
            int tree_size = 0;
            for (int i = 0; i < depth; i++) tree_size = 2 * tree_size + 2;
            Tree = new int?[tree_size + 1];
            for (int i = 0; i < tree_size; i++) Tree[i] = null;
        }

        public int? FindKeyIndex(int key)
        {
            int currentIdx = 0;

            while (currentIdx < Tree.Length)
            {
                if (Tree[currentIdx] == key) return currentIdx;
                if (Tree[currentIdx] == null) return -currentIdx;

                currentIdx = 2 * currentIdx + (Tree[currentIdx] > key ? 1 : 2);
            }

            return null;
        }

        public int AddKey(int key)
        {
            int? foundIdx = FindKeyIndex(key);
            if (foundIdx == null) return -1;

            foundIdx = Math.Abs(foundIdx.Value);
            Tree[foundIdx.Value] = key;

            return foundIdx.Value;
        }
    }
}