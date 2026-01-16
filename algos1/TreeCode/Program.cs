using System;

using AlgorithmsDataStructures2;

namespace TreeCode
{
    class Program
    {
        static void Main(string[] args)
        {
            var tree = new SimpleTree<int>(new SimpleTreeNode<int>(1, null));

            var root = tree.Root;

            var secondLevelOne = new SimpleTreeNode<int>(2, root);
            var secondLevelTwo = new SimpleTreeNode<int>(3, root);
            var secondLevelThree = new SimpleTreeNode<int>(4, root);
            var thirdLevelOne = new SimpleTreeNode<int>(5, secondLevelOne);
            var thirdLevelTwo = new SimpleTreeNode<int>(6, secondLevelOne);
            var thirdLevelThree = new SimpleTreeNode<int>(5, secondLevelOne);
            var thirdLevelFour = new SimpleTreeNode<int>(7, secondLevelTwo);
            var thirdLevelFive= new SimpleTreeNode<int>(8, secondLevelTwo);
            var thirdLevelSix = new SimpleTreeNode<int>(7, secondLevelTwo);
            var thirdLevelSeven = new SimpleTreeNode<int>(10, secondLevelThree);
            var thirdLevelEight = new SimpleTreeNode<int>(10, secondLevelThree);
            var thirdLevelNine = new SimpleTreeNode<int>(10, secondLevelThree);

            tree.AddChild(root, secondLevelOne);
            tree.AddChild(root, secondLevelTwo);
            tree.AddChild(root, secondLevelThree);

            tree.AddChild(secondLevelOne, thirdLevelOne);
            tree.AddChild(secondLevelOne, thirdLevelTwo);
            tree.AddChild(secondLevelOne, thirdLevelThree);

            tree.AddChild(secondLevelTwo, thirdLevelFour);
            tree.AddChild(secondLevelTwo, thirdLevelFive);
            tree.AddChild(secondLevelTwo, thirdLevelSix);

            tree.AddChild(secondLevelThree, thirdLevelSeven);
            tree.AddChild(secondLevelThree, thirdLevelEight);
            tree.AddChild(secondLevelThree, thirdLevelNine);


            tree.ShowNodesLevel();
        }
    }
}