using AlgorithmsDataStructures2;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BalancedBST2Tests
{
    [TestClass]
    public class IsBalanced
    {
        private BalancedBST _bst;

        [TestInitialize]
        public void Init()
        {
            _bst = new BalancedBST();
        }

        [TestMethod]
        public void FalseFirst()
        {
            TreeSeeder.SeedNotBalancedFirst(_bst);
            Assert.IsFalse(_bst.IsBalanced(_bst.Root));
        }

        [TestMethod]
        public void TrueFirst()
        {
            TreeSeeder.SeedBalancedFirst(_bst);
            Assert.IsTrue(_bst.IsBalanced(_bst.Root));
        }

        [TestMethod]
        public void FalseSecond()
        {
            TreeSeeder.SeedNotBalancedSecond(_bst);
            Assert.IsFalse(_bst.IsBalanced(_bst.Root));
        }

        [TestMethod]
        public void TrueSecond()
        {
            TreeSeeder.SeedBalancedSecond(_bst);
            Assert.IsTrue(_bst.IsBalanced(_bst.Root));
        }
    }

    [TestClass]
    public class GenerateTree
    {
        private BalancedBST _bst;

        [TestInitialize]
        public void Init()
        {
            _bst = new BalancedBST();
        }

        [TestMethod]
        public void ThreeElements()
        {
            int[] a = { 10, 5, 9 };
            _bst.GenerateTree(a);
            Assert.IsTrue(_bst.IsBalanced(_bst.Root));
            Assert.AreEqual(9, _bst.Root.NodeKey);
            Assert.AreEqual(5, _bst.Root.LeftChild.NodeKey);
            Assert.AreEqual(10, _bst.Root.RightChild.NodeKey);
        }

        [TestMethod]
        public void FiveElements()
        {
            int[] a = { 10, 50, 9, 1000, -5 };
            _bst.GenerateTree(a);
            Assert.IsTrue(_bst.IsBalanced(_bst.Root));
            Assert.AreEqual(10, _bst.Root.NodeKey);
            Assert.AreEqual(1000, _bst.Root.RightChild.NodeKey);
            Assert.AreEqual(50, _bst.Root.RightChild.LeftChild.NodeKey);
            Assert.AreEqual(9, _bst.Root.LeftChild.NodeKey);
            Assert.AreEqual(-5, _bst.Root.LeftChild.LeftChild.NodeKey);
        }

        [TestMethod]
        public void SevenElements()
        {
            int[] a = { 7, 8, 2, 5, 3, 4, 1, 6 };
            _bst.GenerateTree(a);
            Assert.IsTrue(_bst.IsBalanced(_bst.Root));
            Assert.AreEqual(5, _bst.Root.NodeKey);
            Assert.AreEqual(7, _bst.Root.RightChild.NodeKey);
            Assert.AreEqual(6, _bst.Root.RightChild.LeftChild.NodeKey);
            Assert.AreEqual(8, _bst.Root.RightChild.RightChild.NodeKey);
            Assert.AreEqual(3, _bst.Root.LeftChild.NodeKey);
            Assert.AreEqual(4, _bst.Root.LeftChild.RightChild.NodeKey);
            Assert.AreEqual(2, _bst.Root.LeftChild.LeftChild.NodeKey);
            Assert.AreEqual(1, _bst.Root.LeftChild.LeftChild.LeftChild.NodeKey);
        }
    }

    public static class TreeSeeder
    {
        public static void SeedNotBalancedFirst(BalancedBST bst)
        {
            bst.AddKeyMain(7);
            bst.AddKeyMain(8);
            bst.AddKeyMain(2);
            bst.AddKeyMain(1);
            bst.AddKeyMain(4);
            bst.AddKeyMain(3);
            bst.AddKeyMain(5);
        }

        public static void SeedBalancedFirst(BalancedBST bst)
        {
            bst.AddKeyMain(5);
            bst.AddKeyMain(2);
            bst.AddKeyMain(1);
            bst.AddKeyMain(4);
            bst.AddKeyMain(3);
            bst.AddKeyMain(8);
            bst.AddKeyMain(7);
        }

        public static void SeedNotBalancedSecond(BalancedBST bst)
        {
            bst.AddKeyMain(6);
            bst.AddKeyMain(7);
            bst.AddKeyMain(5);
            bst.AddKeyMain(2);
            bst.AddKeyMain(1);
            bst.AddKeyMain(3);
            bst.AddKeyMain(4);
        }

        public static void SeedBalancedSecond(BalancedBST bst)
        {
            bst.AddKeyMain(6);
            bst.AddKeyMain(7);
            bst.AddKeyMain(8);
            bst.AddKeyMain(9);
            bst.AddKeyMain(5);
            bst.AddKeyMain(2);
            bst.AddKeyMain(1);
            bst.AddKeyMain(3);
            bst.AddKeyMain(4);
        }
    }
}