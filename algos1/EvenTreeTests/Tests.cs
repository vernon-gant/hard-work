using System.Collections.Generic;

using AlgorithmsDataStructures2;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EvenTreesTests
{
    [TestClass]
    public class EvenTrees
    {
        [TestMethod]
        public void ThreeEvenTreesFromTenNodes()
        {
            SimpleTree<int> root = new SimpleTree<int>(new SimpleTreeNode<int>(1, null));
            SimpleTreeNode<int> twoValueNode = new SimpleTreeNode<int>(2, root.Root);
            SimpleTreeNode<int> fiveValueNode = new SimpleTreeNode<int>(5, twoValueNode);
            SimpleTreeNode<int> sevenValueNode = new SimpleTreeNode<int>(7, twoValueNode);
            SimpleTreeNode<int> threeValueNode = new SimpleTreeNode<int>(3, root.Root);
            SimpleTreeNode<int> fourValueNode = new SimpleTreeNode<int>(4, threeValueNode);
            SimpleTreeNode<int> sixValueNode = new SimpleTreeNode<int>(6, root.Root);
            SimpleTreeNode<int> eightValueNode = new SimpleTreeNode<int>(8, sixValueNode);
            SimpleTreeNode<int> nineValueNode = new SimpleTreeNode<int>(9, eightValueNode);
            SimpleTreeNode<int> tenValueNode = new SimpleTreeNode<int>(10, eightValueNode);
            root.AddChild(root.Root, twoValueNode);
            root.AddChild(root.Root, threeValueNode);
            root.AddChild(root.Root, sixValueNode);
            root.AddChild(twoValueNode, fiveValueNode);
            root.AddChild(twoValueNode, sevenValueNode);
            root.AddChild(threeValueNode, fourValueNode);
            root.AddChild(sixValueNode, eightValueNode);
            root.AddChild(eightValueNode, nineValueNode);
            root.AddChild(eightValueNode, tenValueNode);
            List<int> evenTrees = root.EvenTrees();
            Assert.AreEqual(4, evenTrees.Count);
            Assert.AreEqual(1, evenTrees[0]);
            Assert.AreEqual(3, evenTrees[1]);
            Assert.AreEqual(1, evenTrees[2]);
            Assert.AreEqual(6, evenTrees[3]);
        }

        [TestMethod]
        public void FourEvenTreesFromTenNodes()
        {
            SimpleTree<int> root = new SimpleTree<int>(new SimpleTreeNode<int>(1, null));
            SimpleTreeNode<int> node2 = new SimpleTreeNode<int>(2, root.Root);
            SimpleTreeNode<int> node3 = new SimpleTreeNode<int>(3, node2);
            SimpleTreeNode<int> node4 = new SimpleTreeNode<int>(4, root.Root);
            SimpleTreeNode<int> node5 = new SimpleTreeNode<int>(5, node4);
            SimpleTreeNode<int> node6 = new SimpleTreeNode<int>(6, root.Root);
            SimpleTreeNode<int> node7 = new SimpleTreeNode<int>(7, node6);
            SimpleTreeNode<int> node8 = new SimpleTreeNode<int>(8, root.Root);
            SimpleTreeNode<int> node9 = new SimpleTreeNode<int>(9, node8);
            SimpleTreeNode<int> node10 = new SimpleTreeNode<int>(10, node9);

            root.AddChild(root.Root, node2);
            root.AddChild(node2, node3);
            root.AddChild(root.Root, node4);
            root.AddChild(node4, node5);
            root.AddChild(root.Root, node6);
            root.AddChild(node6, node7);
            root.AddChild(root.Root, node8);
            root.AddChild(node8, node9);
            root.AddChild(node9, node10);
            List<int> evenTrees = root.EvenTrees();
            Assert.AreEqual(8, evenTrees.Count);
            Assert.AreEqual(1, evenTrees[0]);
            Assert.AreEqual(2, evenTrees[1]);
            Assert.AreEqual(1, evenTrees[2]);
            Assert.AreEqual(4, evenTrees[3]);
            Assert.AreEqual(1, evenTrees[4]);
            Assert.AreEqual(6, evenTrees[5]);
            Assert.AreEqual(8, evenTrees[6]);
            Assert.AreEqual(9, evenTrees[7]);
        }

        [TestMethod]
        public void OneTree()
        {
            SimpleTree<int> root = new SimpleTree<int>(new SimpleTreeNode<int>(1, null));
            SimpleTreeNode<int> node2 = new SimpleTreeNode<int>(2, root.Root);
            SimpleTreeNode<int> node3 = new SimpleTreeNode<int>(3, node2);
            SimpleTreeNode<int> node4 = new SimpleTreeNode<int>(4, node2);
            SimpleTreeNode<int> node5 = new SimpleTreeNode<int>(5, root.Root);
            SimpleTreeNode<int> node6 = new SimpleTreeNode<int>(6, node5);
            SimpleTreeNode<int> node7 = new SimpleTreeNode<int>(7, node5);
            SimpleTreeNode<int> node8 = new SimpleTreeNode<int>(8, root.Root);
            SimpleTreeNode<int> node9 = new SimpleTreeNode<int>(9, node8);
            SimpleTreeNode<int> node10 = new SimpleTreeNode<int>(10, node8);

            root.AddChild(root.Root, node2);
            root.AddChild(node2, node3);
            root.AddChild(node2, node4);
            root.AddChild(root.Root, node5);
            root.AddChild(node5, node6);
            root.AddChild(node5, node7);
            root.AddChild(root.Root, node8);
            root.AddChild(node8, node9);
            root.AddChild(node8, node10);
            List<int> evenTrees = root.EvenTrees();
            Assert.AreEqual(0, evenTrees.Count);
        }
    }
}