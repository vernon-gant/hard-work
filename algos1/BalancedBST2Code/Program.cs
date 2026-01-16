using System;

using AlgorithmsDataStructures2;

namespace BalancedBST2
{
    class Program
    {
        static void Main(string[] args)
        {
            BalancedBST bst = new BalancedBST();
            bst.AddKeyMain(7);
            bst.AddKeyMain(8);
            bst.AddKeyMain(9);
            bst.AddKeyMain(2);
            bst.AddKeyMain(1);
            bst.AddKeyMain(4);
            bst.AddKeyMain(3);
            bst.AddKeyMain(5);
            Console.WriteLine(bst.IsBalanced(bst.Root));
        }
    }
}