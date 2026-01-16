using System.Collections.Generic;

using AlgorithmsDataStructures2;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinaryTreeTests
{
    [TestClass]
    public class TestFind
    {
        private BST<int> _tree = new (null);

        [TestMethod]
        public void Empty()
        {
            BSTFind<int> found = _tree.FindNodeByKey(1);
            Assert.IsFalse(found.NodeHasKey);
            Assert.IsNull(found.Node);
            Assert.IsFalse(found.ToLeft);
        }

        [TestMethod]
        public void One()
        {
            _tree.AddKeyValue(1, 1);
            BSTFind<int> found = _tree.FindNodeByKey(1);
            Assert.IsTrue(found.NodeHasKey);
            Assert.IsNotNull(found.Node);
            Assert.IsFalse(found.ToLeft);
        }

        [TestMethod]
        public void Many()
        {
            _tree = TreeSeeder.SeedFullTreeGeneric();
            BSTFind<int> found = _tree.FindNodeByKey(15);
            Assert.IsTrue(found.NodeHasKey);
            Assert.IsNotNull(found.Node);
            Assert.AreEqual(15, found.Node.NodeKey);
            Assert.IsFalse(found.ToLeft);
        }

        [TestMethod]
        public void AddLeft()
        {
            _tree = TreeSeeder.SeedFullTreeGeneric();
            BSTFind<int> found = _tree.FindNodeByKey(-1);
            Assert.IsFalse(found.NodeHasKey);
            Assert.IsNotNull(found.Node);
            Assert.AreEqual(1, found.Node.NodeKey);
            Assert.IsTrue(found.ToLeft);
        }

        [TestMethod]
        public void AddRight()
        {
            _tree = TreeSeeder.SeedFullTreeGeneric();
            BSTFind<int> found = _tree.FindNodeByKey(16);
            Assert.IsFalse(found.NodeHasKey);
            Assert.IsNotNull(found.Node);
            Assert.AreEqual(15, found.Node.NodeKey);
            Assert.IsFalse(found.ToLeft);
        }
    }

    [TestClass]
    public class TestAdd
    {
        private BST<int> _tree = new (null);

        [TestMethod]
        public void Empty()
        {
            _tree.AddKeyValue(1, 1);
            Assert.AreEqual(1, _tree.Count());
            BSTFind<int> found = _tree.FindNodeByKey(1);
            Assert.IsTrue(found.NodeHasKey);
            Assert.IsNull(found.Node.Parent);
            Assert.IsNull(found.Node.LeftChild);
            Assert.IsNull(found.Node.RightChild);
            Assert.AreEqual(1, found.Node.NodeKey);
            Assert.AreEqual(1, found.Node.NodeValue);
            Assert.IsFalse(_tree.AddKeyValue(1, 1));
            Assert.AreEqual(1, _tree.Count());
        }

        [TestMethod]
        public void AddTwo()
        {
            _tree.AddKeyValue(1, 1);
            _tree.AddKeyValue(2, 2);
            Assert.AreEqual(2, _tree.Count());
            BSTFind<int> found = _tree.FindNodeByKey(1);
            Assert.IsTrue(found.NodeHasKey);
            Assert.IsNull(found.Node.Parent);
            Assert.IsNull(found.Node.LeftChild);
            Assert.IsNotNull(found.Node.RightChild);
            Assert.AreEqual(1, found.Node.NodeKey);
            Assert.AreEqual(1, found.Node.NodeValue);
            found = _tree.FindNodeByKey(2);
            Assert.IsTrue(found.NodeHasKey);
            Assert.IsNotNull(found.Node.Parent);
            Assert.AreEqual(1, found.Node.Parent.NodeKey);
            Assert.IsNull(found.Node.RightChild);
            Assert.IsNull(found.Node.LeftChild);
            Assert.AreEqual(2, found.Node.NodeKey);
            Assert.AreEqual(2, found.Node.NodeValue);
            Assert.IsFalse(_tree.AddKeyValue(2, 2));
            Assert.AreEqual(2, _tree.Count());
        }

        [TestMethod]
        public void AddThree()
        {
            _tree = TreeSeeder.SeedThreeNodesGeneric();
            Assert.AreEqual(3, _tree.Count());
            BSTFind<int> found = _tree.FindNodeByKey(1);
            Assert.IsTrue(found.NodeHasKey);
            Assert.IsNotNull(found.Node.Parent);
            Assert.AreEqual(2, found.Node.Parent.NodeKey);
            Assert.IsNull(found.Node.LeftChild);
            Assert.IsNull(found.Node.RightChild);
            Assert.AreEqual(1, found.Node.NodeKey);
            Assert.AreEqual(1, found.Node.NodeValue);
            found = _tree.FindNodeByKey(2);
            Assert.IsTrue(found.NodeHasKey);
            Assert.IsNull(found.Node.Parent);
            Assert.IsNotNull(found.Node.RightChild);
            Assert.IsNotNull(found.Node.LeftChild);
            Assert.AreEqual(2, found.Node.NodeKey);
            Assert.AreEqual(2, found.Node.NodeValue);
            found = _tree.FindNodeByKey(3);
            Assert.IsTrue(found.NodeHasKey);
            Assert.IsNotNull(found.Node.Parent);
            Assert.AreEqual(2, found.Node.Parent.NodeKey);
            Assert.IsNull(found.Node.RightChild);
            Assert.IsNull(found.Node.LeftChild);
            Assert.AreEqual(3, found.Node.NodeKey);
            Assert.AreEqual(3, found.Node.NodeValue);
            Assert.IsFalse(_tree.AddKeyValue(3, 3));
            Assert.AreEqual(3, _tree.Count());
        }
    }

    [TestClass]
    public class TestFindMinMax
    {
        private BST<int> _tree = new (null);

        [TestMethod]
        public void Empty()
        {
            BSTNode<int> found = _tree.FinMinMax(null, true);
            Assert.IsNull(found);
            found = _tree.FinMinMax(null, false);
            Assert.IsNull(found);
        }

        [TestMethod]
        public void FromRootMin()
        {
            _tree = TreeSeeder.SeedFullTreeGeneric();
            BSTNode<int> root = _tree.FindNodeByKey(8).Node;
            BSTNode<int> found = _tree.FinMinMax(root, false);
            Assert.IsNotNull(found);
            Assert.AreEqual(1, found.NodeKey);
        }

        [TestMethod]
        public void FromRootMax()
        {
            _tree = TreeSeeder.SeedFullTreeGeneric();
            BSTNode<int> root = _tree.FindNodeByKey(8).Node;
            BSTNode<int> found = _tree.FinMinMax(root, true);
            Assert.IsNotNull(found);
            Assert.AreEqual(15, found.NodeKey);
        }

        [TestMethod]
        public void FromNonRootMin()
        {
            _tree = TreeSeeder.SeedFullTreeGeneric();
            BSTNode<int> root = _tree.FindNodeByKey(12).Node;
            BSTNode<int> found = _tree.FinMinMax(root, false);
            Assert.IsNotNull(found);
            Assert.AreEqual(9, found.NodeKey);
        }

        [TestMethod]
        public void FromNonRootMax()
        {
            _tree = TreeSeeder.SeedFullTreeGeneric();
            BSTNode<int> root = _tree.FindNodeByKey(4).Node;
            BSTNode<int> found = _tree.FinMinMax(root, true);
            Assert.IsNotNull(found);
            Assert.AreEqual(7, found.NodeKey);
        }
    }

    [TestClass]
    public class TestRemove
    {
        private BST<int> _tree = new (null);

        [TestMethod]
        public void Empty()
        {
            Assert.AreEqual(0, _tree.Count());
            Assert.IsFalse(_tree.DeleteNodeByKey(1));
        }

        [TestMethod]
        public void Unexisting()
        {
            _tree = TreeSeeder.SeedFullTreeGeneric();
            Assert.IsFalse(_tree.DeleteNodeByKey(0));
            Assert.IsFalse(_tree.DeleteNodeByKey(16));
            Assert.AreEqual(15, _tree.Count());
        }

        [TestMethod]
        public void RootWhenTreeOfOnlyRoot()
        {
            _tree.AddKeyValue(1, 1);
            Assert.IsTrue(_tree.DeleteNodeByKey(1));
            Assert.AreEqual(0, _tree.Count());
            BSTFind<int> found = _tree.FindNodeByKey(1);
            Assert.IsFalse(found.NodeHasKey);
            Assert.IsNull(found.Node);
            Assert.IsFalse(found.ToLeft);
        }

        [TestMethod]
        public void RootWhenTreeOfThreeNodes()
        {
            _tree = TreeSeeder.SeedThreeNodesGeneric();
            Assert.IsTrue(_tree.DeleteNodeByKey(2));
            Assert.AreEqual(2, _tree.Count());
            BSTFind<int> found = _tree.FindNodeByKey(2);
            Assert.IsFalse(found.NodeHasKey);
            Assert.AreEqual(1, found.Node.NodeKey);
            Assert.IsFalse(found.ToLeft);
            BSTNode<int> root = _tree.FindNodeByKey(3).Node;
            Assert.IsNotNull(root);
            Assert.AreEqual(3, root.NodeKey);
            Assert.AreEqual(3, root.NodeValue);
            Assert.IsNull(root.Parent);
        }

        [TestMethod]
        public void RootWhenFullTree()
        {
            _tree = TreeSeeder.SeedFullTreeGeneric();
            Assert.IsTrue(_tree.DeleteNodeByKey(8));
            Assert.AreEqual(14, _tree.Count());
            BSTFind<int> found = _tree.FindNodeByKey(8);
            Assert.IsFalse(found.NodeHasKey);
            Assert.AreEqual(7, found.Node.NodeKey);
            Assert.IsFalse(found.ToLeft);
            BSTNode<int> root = _tree.FindNodeByKey(9).Node;
            Assert.IsNotNull(root);
            Assert.AreEqual(9, root.NodeKey);
            Assert.AreEqual(9, root.NodeValue);
            Assert.IsNull(root.Parent);
        }

        [TestMethod]
        public void FullRightPart()
        {
            _tree = TreeSeeder.SeedFullTreeGeneric();
            Assert.IsTrue(_tree.DeleteNodeByKey(12));
            Assert.IsTrue(_tree.DeleteNodeByKey(14));
            Assert.IsTrue(_tree.DeleteNodeByKey(15));
            Assert.AreEqual(12, _tree.Count());
            Assert.IsFalse(_tree.DeleteNodeByKey(15));
            Assert.IsFalse(_tree.DeleteNodeByKey(14));
            Assert.IsFalse(_tree.DeleteNodeByKey(12));
            Assert.AreEqual(12, _tree.Count());
            Assert.IsTrue(_tree.DeleteNodeByKey(9));
            Assert.IsTrue(_tree.DeleteNodeByKey(10));
            Assert.IsTrue(_tree.DeleteNodeByKey(11));
            Assert.IsTrue(_tree.DeleteNodeByKey(13));
            Assert.AreEqual(8, _tree.Count());
            _tree.AddKeyValue(9, 9);
            _tree.AddKeyValue(10, 10);
            _tree.AddKeyValue(11, 11);
            _tree.AddKeyValue(13, 13);
            Assert.AreEqual(12, _tree.Count());
            Assert.IsTrue(_tree.DeleteNodeByKey(8));
            Assert.AreEqual(11, _tree.Count());
            BSTFind<int> found = _tree.FindNodeByKey(8);
            Assert.IsFalse(found.NodeHasKey);
            Assert.AreEqual(7, found.Node.NodeKey);
            Assert.IsFalse(found.ToLeft);
            BSTNode<int> root = _tree.FindNodeByKey(9).Node;
            Assert.IsNotNull(root);
            Assert.AreEqual(9, root.NodeKey);
            Assert.AreEqual(9, root.NodeValue);
            Assert.IsNull(root.Parent);
            Assert.AreEqual(10, root.RightChild.NodeKey);
            Assert.AreEqual(root, root.RightChild.Parent);
        }

        [TestMethod]
        public void Leaf()
        {
            _tree = TreeSeeder.SeedFullTreeGeneric();
            Assert.IsTrue(_tree.DeleteNodeByKey(1));
            Assert.AreEqual(14, _tree.Count());
            BSTFind<int> found = _tree.FindNodeByKey(1);
            Assert.IsFalse(found.NodeHasKey);
            Assert.AreEqual(2, found.Node.NodeKey);
            Assert.IsTrue(found.ToLeft);
            BSTNode<int> twoNode = _tree.FindNodeByKey(2).Node;
            Assert.IsNotNull(twoNode);
            Assert.AreEqual(2, twoNode.NodeKey);
            Assert.AreEqual(2, twoNode.NodeValue);
            Assert.IsNull(twoNode.LeftChild);
        }

        [TestMethod]
        public void NodeWithOneChild()
        {
            _tree = TreeSeeder.SeedFullTreeGeneric();
            Assert.IsTrue(_tree.DeleteNodeByKey(2));
            Assert.AreEqual(14, _tree.Count());
            BSTNode<int> threeNode = _tree.FindNodeByKey(3).Node;
            Assert.IsNotNull(threeNode.LeftChild);
            Assert.AreEqual(1, threeNode.LeftChild.NodeKey);
            Assert.IsNull(threeNode.RightChild);
            Assert.AreEqual(4, threeNode.Parent.NodeKey);
            Assert.IsTrue(_tree.DeleteNodeByKey(3));
            Assert.AreEqual(13, _tree.Count());
            BSTFind<int> found = _tree.FindNodeByKey(3);
            Assert.IsFalse(found.NodeHasKey);
            Assert.AreEqual(1, found.Node.NodeKey);
            Assert.IsFalse(found.ToLeft);
            BSTNode<int> fourNode = _tree.FindNodeByKey(4).Node;
            Assert.IsNotNull(fourNode.LeftChild);
            Assert.AreEqual(1, fourNode.LeftChild.NodeKey);
            Assert.IsNotNull(fourNode.RightChild);
        }
    }

    [TestClass]
    public class TestTraversalGeneric
    {
        private BST<int> _tree = new (null);


        [TestMethod]
        public void InOrderRecursion()
        {
            _tree = TreeSeeder.SeedFullTreeGeneric();
            List<BSTNode> inOrderNodes = _tree.DeepAllNodes(0);
            Assert.AreEqual(15, inOrderNodes.Count);

            for (int i = 1; i <= 15; i++)
            {
                Assert.AreEqual(inOrderNodes[i - 1].NodeKey, i);
            }
        }

        [TestMethod]
        public void InOrderStack()
        {
            _tree = TreeSeeder.SeedFullTreeGeneric();
            List<BSTNode> inOrderNodes = _tree.DeepAllNodesStack(0);
            Assert.AreEqual(15, inOrderNodes.Count);

            for (int i = 1; i <= 15; i++)
            {
                Assert.AreEqual(inOrderNodes[i - 1].NodeKey, i);
            }
        }

        [TestMethod]
        public void InOrderStackSecond()
        {
            _tree = TreeSeeder.SeedRandomOrderGeneric();
            List<BSTNode> inOrderNodes = _tree.DeepAllNodesStack(0);

            Assert.AreEqual(6, inOrderNodes.Count);

            Assert.AreEqual(inOrderNodes[0].NodeKey, 1);
            Assert.AreEqual(inOrderNodes[1].NodeKey, 50);
            Assert.AreEqual(inOrderNodes[2].NodeKey, 55);
            Assert.AreEqual(inOrderNodes[3].NodeKey, 60);
            Assert.AreEqual(inOrderNodes[4].NodeKey, 70);
            Assert.AreEqual(inOrderNodes[5].NodeKey, 100);
        }

        [TestMethod]
        public void PostOrderRecursion()
        {
            _tree = TreeSeeder.SeedFullTreeGeneric();
            List<BSTNode> postOrderNodes = _tree.DeepAllNodes(1);
            Assert.AreEqual(15, postOrderNodes.Count);

            Assert.AreEqual(postOrderNodes[0].NodeKey, 1);
            Assert.AreEqual(postOrderNodes[1].NodeKey, 3);
            Assert.AreEqual(postOrderNodes[2].NodeKey, 2);
            Assert.AreEqual(postOrderNodes[3].NodeKey, 5);
            Assert.AreEqual(postOrderNodes[4].NodeKey, 7);
            Assert.AreEqual(postOrderNodes[5].NodeKey, 6);
            Assert.AreEqual(postOrderNodes[6].NodeKey, 4);
            Assert.AreEqual(postOrderNodes[7].NodeKey, 9);
            Assert.AreEqual(postOrderNodes[8].NodeKey, 11);
            Assert.AreEqual(postOrderNodes[9].NodeKey, 10);
            Assert.AreEqual(postOrderNodes[10].NodeKey, 13);
            Assert.AreEqual(postOrderNodes[11].NodeKey, 15);
            Assert.AreEqual(postOrderNodes[12].NodeKey, 14);
            Assert.AreEqual(postOrderNodes[13].NodeKey, 12);
            Assert.AreEqual(postOrderNodes[14].NodeKey, 8);
        }

        [TestMethod]
        public void PostOrderStack()
        {
            _tree = TreeSeeder.SeedFullTreeGeneric();
            List<BSTNode> postOrderNodes = _tree.DeepAllNodesStack(1);
            Assert.AreEqual(15, postOrderNodes.Count);

            Assert.AreEqual(postOrderNodes[0].NodeKey, 1);
            Assert.AreEqual(postOrderNodes[1].NodeKey, 3);
            Assert.AreEqual(postOrderNodes[2].NodeKey, 2);
            Assert.AreEqual(postOrderNodes[3].NodeKey, 5);
            Assert.AreEqual(postOrderNodes[4].NodeKey, 7);
            Assert.AreEqual(postOrderNodes[5].NodeKey, 6);
            Assert.AreEqual(postOrderNodes[6].NodeKey, 4);
            Assert.AreEqual(postOrderNodes[7].NodeKey, 9);
            Assert.AreEqual(postOrderNodes[8].NodeKey, 11);
            Assert.AreEqual(postOrderNodes[9].NodeKey, 10);
            Assert.AreEqual(postOrderNodes[10].NodeKey, 13);
            Assert.AreEqual(postOrderNodes[11].NodeKey, 15);
            Assert.AreEqual(postOrderNodes[12].NodeKey, 14);
            Assert.AreEqual(postOrderNodes[13].NodeKey, 12);
            Assert.AreEqual(postOrderNodes[14].NodeKey, 8);
        }

        [TestMethod]
        public void PostOrderStackSecond()
        {
            _tree = TreeSeeder.SeedRandomOrderGeneric();
            List<BSTNode> postOrderNodes = _tree.DeepAllNodesStack(1);
            Assert.AreEqual(6, postOrderNodes.Count);

            Assert.AreEqual(postOrderNodes[0].NodeKey, 1);
            Assert.AreEqual(postOrderNodes[1].NodeKey, 55);
            Assert.AreEqual(postOrderNodes[2].NodeKey, 70);
            Assert.AreEqual(postOrderNodes[3].NodeKey, 60);
            Assert.AreEqual(postOrderNodes[4].NodeKey, 50);
            Assert.AreEqual(postOrderNodes[5].NodeKey, 100);
        }

        [TestMethod]
        public void PreOrderRecursion()
        {
            _tree = TreeSeeder.SeedFullTreeGeneric();
            List<BSTNode> preOrderNodes = _tree.DeepAllNodes(2);
            Assert.AreEqual(15, preOrderNodes.Count);

            Assert.AreEqual(8, preOrderNodes[0].NodeKey);
            Assert.AreEqual(4, preOrderNodes[1].NodeKey);
            Assert.AreEqual(2, preOrderNodes[2].NodeKey);
            Assert.AreEqual(1, preOrderNodes[3].NodeKey);
            Assert.AreEqual(3, preOrderNodes[4].NodeKey);
            Assert.AreEqual(6, preOrderNodes[5].NodeKey);
            Assert.AreEqual(5, preOrderNodes[6].NodeKey);
            Assert.AreEqual(7, preOrderNodes[7].NodeKey);
            Assert.AreEqual(12, preOrderNodes[8].NodeKey);
            Assert.AreEqual(10, preOrderNodes[9].NodeKey);
            Assert.AreEqual(9, preOrderNodes[10].NodeKey);
            Assert.AreEqual(11, preOrderNodes[11].NodeKey);
            Assert.AreEqual(14, preOrderNodes[12].NodeKey);
            Assert.AreEqual(13, preOrderNodes[13].NodeKey);
            Assert.AreEqual(15, preOrderNodes[14].NodeKey);
        }

        [TestMethod]
        public void PreOrderStack()
        {
            _tree = TreeSeeder.SeedFullTreeGeneric();
            List<BSTNode> preOrderNodes = _tree.DeepAllNodesStack(2);
            Assert.AreEqual(15, preOrderNodes.Count);

            Assert.AreEqual(8, preOrderNodes[0].NodeKey);
            Assert.AreEqual(4, preOrderNodes[1].NodeKey);
            Assert.AreEqual(2, preOrderNodes[2].NodeKey);
            Assert.AreEqual(1, preOrderNodes[3].NodeKey);
            Assert.AreEqual(3, preOrderNodes[4].NodeKey);
            Assert.AreEqual(6, preOrderNodes[5].NodeKey);
            Assert.AreEqual(5, preOrderNodes[6].NodeKey);
            Assert.AreEqual(7, preOrderNodes[7].NodeKey);
            Assert.AreEqual(12, preOrderNodes[8].NodeKey);
            Assert.AreEqual(10, preOrderNodes[9].NodeKey);
            Assert.AreEqual(9, preOrderNodes[10].NodeKey);
            Assert.AreEqual(11, preOrderNodes[11].NodeKey);
            Assert.AreEqual(14, preOrderNodes[12].NodeKey);
            Assert.AreEqual(13, preOrderNodes[13].NodeKey);
            Assert.AreEqual(15, preOrderNodes[14].NodeKey);
        }

        [TestMethod]
        public void PreOrderStackSecond()
        {
            _tree = TreeSeeder.SeedRandomOrderGeneric();
            List<BSTNode> preOrderNodes = _tree.DeepAllNodesStack(2);
            Assert.AreEqual(6, preOrderNodes.Count);

            Assert.AreEqual(100, preOrderNodes[0].NodeKey);
            Assert.AreEqual(50, preOrderNodes[1].NodeKey);
            Assert.AreEqual(1, preOrderNodes[2].NodeKey);
            Assert.AreEqual(60, preOrderNodes[3].NodeKey);
            Assert.AreEqual(55, preOrderNodes[4].NodeKey);
            Assert.AreEqual(70, preOrderNodes[5].NodeKey);
        }

        [TestMethod]
        public void BreadthTraversal()
        {
            _tree = TreeSeeder.SeedFullTreeGeneric();
            List<BSTNode> inBreadthNodes = _tree.WideAllNodes();

            Assert.AreEqual(15, inBreadthNodes.Count);

            Assert.AreEqual(8, inBreadthNodes[0].NodeKey);
            Assert.AreEqual(4, inBreadthNodes[1].NodeKey);
            Assert.AreEqual(12, inBreadthNodes[2].NodeKey);
            Assert.AreEqual(2, inBreadthNodes[3].NodeKey);
            Assert.AreEqual(6, inBreadthNodes[4].NodeKey);
            Assert.AreEqual(10, inBreadthNodes[5].NodeKey);
            Assert.AreEqual(14, inBreadthNodes[6].NodeKey);
            Assert.AreEqual(1, inBreadthNodes[7].NodeKey);
            Assert.AreEqual(3, inBreadthNodes[8].NodeKey);
            Assert.AreEqual(5, inBreadthNodes[9].NodeKey);
            Assert.AreEqual(7, inBreadthNodes[10].NodeKey);
            Assert.AreEqual(9, inBreadthNodes[11].NodeKey);
            Assert.AreEqual(11, inBreadthNodes[12].NodeKey);
            Assert.AreEqual(13, inBreadthNodes[13].NodeKey);
            Assert.AreEqual(15, inBreadthNodes[14].NodeKey);
        }

        [TestMethod]
        public void BreadthTraversalSecond()
        {
            _tree = TreeSeeder.SeedRandomOrderGeneric();
            List<BSTNode> inBreadthNodes = _tree.WideAllNodes();

            Assert.AreEqual(6, inBreadthNodes.Count);

            Assert.AreEqual(100, inBreadthNodes[0].NodeKey);
            Assert.AreEqual(50, inBreadthNodes[1].NodeKey);
            Assert.AreEqual(1, inBreadthNodes[2].NodeKey);
            Assert.AreEqual(60, inBreadthNodes[3].NodeKey);
            Assert.AreEqual(55, inBreadthNodes[4].NodeKey);
            Assert.AreEqual(70, inBreadthNodes[5].NodeKey);
        }
    }

    public static class TreeSeeder
    {
        public static BST<int> SeedFullTreeGeneric()
        {
            BSTNode<int> root = new BSTNode<int>(8, 8, null);
            BST<int> tree = new BST<int>(root);

            tree.AddKeyValue(4, 4);
            tree.AddKeyValue(12, 12);

            tree.AddKeyValue(2, 2);
            tree.AddKeyValue(6, 6);
            tree.AddKeyValue(1, 1);
            tree.AddKeyValue(3, 3);
            tree.AddKeyValue(5, 5);
            tree.AddKeyValue(7, 7);

            tree.AddKeyValue(10, 10);
            tree.AddKeyValue(14, 14);
            tree.AddKeyValue(9, 9);
            tree.AddKeyValue(11, 11);
            tree.AddKeyValue(13, 13);
            tree.AddKeyValue(15, 15);

            return tree;
        }

        public static BST<int> SeedRandomOrderGeneric()
        {
            BST<int> tree = new BST<int>(null);
            tree.AddKeyValue(100, 100);
            tree.AddKeyValue(50, 50);
            tree.AddKeyValue(1, 1);
            tree.AddKeyValue(60, 60);
            tree.AddKeyValue(55, 55);
            tree.AddKeyValue(70, 70);

            return tree;
        }

        public static BST<int> SeedThreeNodesGeneric()
        {
            BSTNode<int> root = new BSTNode<int>(2, 2, null);
            BST<int> tree = new BST<int>(root);

            tree.AddKeyValue(1, 1);
            tree.AddKeyValue(3, 3);

            return tree;
        }
    }
}