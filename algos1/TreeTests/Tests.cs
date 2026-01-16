using AlgorithmsDataStructures2;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TreeTests
{
    [TestClass]
    public class TestAdd
    {
        [TestMethod]
        public void ManyNodes()
        {
            var tree = new SimpleTree<int>(new SimpleTreeNode<int>(1, null));
            TreeSeeder.PopulateTree(tree);
            Assert.AreEqual(1, tree.Root.NodeValue);
            Assert.AreEqual(2, tree.Root.Children[0].NodeValue);
            Assert.AreEqual(3, tree.Root.Children[1].NodeValue);
            Assert.AreEqual(4, tree.Root.Children[2].NodeValue);
            Assert.AreEqual(5, tree.Root.Children[0].Children[0].NodeValue);
            Assert.AreEqual(6, tree.Root.Children[0].Children[1].NodeValue);
            Assert.AreEqual(5, tree.Root.Children[0].Children[2].NodeValue);
            Assert.AreEqual(7, tree.Root.Children[1].Children[0].NodeValue);
            Assert.AreEqual(8, tree.Root.Children[1].Children[1].NodeValue);
            Assert.AreEqual(7, tree.Root.Children[1].Children[2].NodeValue);
            Assert.AreEqual(10, tree.Root.Children[2].Children[0].NodeValue);
            Assert.AreEqual(10, tree.Root.Children[2].Children[1].NodeValue);
            Assert.AreEqual(10, tree.Root.Children[2].Children[2].NodeValue);
        }
    }

    [TestClass]
    public class TestFind
    {

        private static SimpleTree<int> tree;

        [TestInitialize]
        public void SetupBeforeEachTest()
        {
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(1, null));
            TreeSeeder.PopulateTree(tree);
        }


        [TestMethod]
        public void FindMany()
        {
            var foundTens = tree.FindNodesByValue(10);
            Assert.AreEqual(3, foundTens.Count);
            Assert.AreEqual(10, foundTens[0].NodeValue);
            Assert.AreEqual(10, foundTens[1].NodeValue);
            Assert.AreEqual(10, foundTens[2].NodeValue);
        }

        [TestMethod]
        public void FindOne()
        {
            var foundTens = tree.FindNodesByValue(8);
            Assert.AreEqual(1, foundTens.Count);
            Assert.AreEqual(8, foundTens[0].NodeValue);
        }
    }

    [TestClass]
    public class TestDelete
    {

        private static SimpleTree<int> tree;

        [TestInitialize]
        public void SetupBeforeEachTest()
        {
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(1, null));
            TreeSeeder.PopulateTree(tree);
        }

        [TestMethod]
        public void FirstLevel()
        {
            var twoValueNode = tree.Root.Children[0];
            tree.DeleteNode(twoValueNode);
            Assert.AreEqual(1, tree.Root.NodeValue);
            Assert.AreEqual(2, tree.Root.Children.Count);
            Assert.AreEqual(3, tree.Root.Children[0].NodeValue);
            Assert.AreEqual(4, tree.Root.Children[1].NodeValue);
            Assert.AreEqual(0,tree.FindNodesByValue(2).Count);
            Assert.AreEqual(6, tree.LeafCount());
        }

        [TestMethod]
        public void SecondLevel()
        {
            var fiveValueNode = tree.Root.Children[0].Children[0];
            tree.DeleteNode(fiveValueNode);
            Assert.AreEqual(1, tree.Root.NodeValue);
            Assert.AreEqual(3, tree.Root.Children.Count);
            Assert.AreEqual(2, tree.Root.Children[0].Children.Count);
            Assert.AreEqual(6, tree.Root.Children[0].Children[0].NodeValue);
            Assert.AreEqual(5, tree.Root.Children[0].Children[1].NodeValue);
            Assert.AreEqual(1, tree.FindNodesByValue(5).Count);
            Assert.AreEqual(8, tree.LeafCount());
        }

        [TestMethod]
        public void AllFirst()
        {
            var twoValueNode = tree.Root.Children[0];
            tree.DeleteNode(twoValueNode);
            Assert.AreEqual(1, tree.Root.NodeValue);
            Assert.AreEqual(2, tree.Root.Children.Count);
            Assert.AreEqual(3, tree.Root.Children[0].NodeValue);
            Assert.AreEqual(4, tree.Root.Children[1].NodeValue);
            Assert.AreEqual(0,tree.FindNodesByValue(2).Count);
            Assert.AreEqual(6, tree.LeafCount());
            var threeValueNode = tree.Root.Children[0];
            tree.DeleteNode(threeValueNode);
            Assert.AreEqual(1, tree.Root.NodeValue);
            Assert.AreEqual(1, tree.Root.Children.Count);
            Assert.AreEqual(4, tree.Root.Children[0].NodeValue);
            Assert.AreEqual(0,tree.FindNodesByValue(3).Count);
            Assert.AreEqual(3, tree.LeafCount());
            var fourValueNode = tree.Root.Children[0];
            tree.DeleteNode(fourValueNode);
            Assert.AreEqual(1, tree.Root.NodeValue);
            Assert.AreEqual(0, tree.Root.Children.Count);
            Assert.AreEqual(0,tree.FindNodesByValue(4).Count);
            Assert.AreEqual(1, tree.LeafCount());
        }
    }

    [TestClass]
    public class TestCount
    {

        private static SimpleTree<int> tree;

        [TestInitialize]
        public void SetupBeforeEachTest()
        {
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(1, null));
            TreeSeeder.PopulateTree(tree);
        }

        [TestMethod]
        public void Count()
        {
            Assert.AreEqual(13, tree.Count());
        }

        [TestMethod]
        public void LeafCount()
        {
            Assert.AreEqual(9, tree.LeafCount());
        }
    }

    [TestClass]
    public class TestGetAll
    {
        private static  SimpleTree<int> tree;

        [TestInitialize]
        public void SetupBeforeEachTest()
        {
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(1, null));
            TreeSeeder.PopulateTree(tree);
        }

        [TestMethod]
        public void GetAll()
        {
            var allNodes = tree.GetAllNodes();
            Assert.AreEqual(13, allNodes.Count);
            Assert.AreEqual(5, allNodes[0].NodeValue);
            Assert.AreEqual(6, allNodes[1].NodeValue);
            Assert.AreEqual(5, allNodes[2].NodeValue);
            Assert.AreEqual(2, allNodes[3].NodeValue);
            Assert.AreEqual(7, allNodes[4].NodeValue);
            Assert.AreEqual(8, allNodes[5].NodeValue);
            Assert.AreEqual(7, allNodes[6].NodeValue);
            Assert.AreEqual(3, allNodes[7].NodeValue);
            Assert.AreEqual(10, allNodes[8].NodeValue);
            Assert.AreEqual(10, allNodes[9].NodeValue);
            Assert.AreEqual(10, allNodes[10].NodeValue);
            Assert.AreEqual(4, allNodes[11].NodeValue);
            Assert.AreEqual(1, allNodes[12].NodeValue);
        }
    }

    [TestClass]
    public class TestMove
    {
        private static SimpleTree<int> tree;

        [TestInitialize]
        public void SetupBeforeEachTest()
        {
            tree = new SimpleTree<int>(new SimpleTreeNode<int>(1, null));
            TreeSeeder.PopulateTree(tree);
        }

        [TestMethod]
        public void FirstLevelToSecond()
        {
            var twoValueNode = tree.Root.Children[0];
            var threeValueNode = tree.Root.Children[1];
            tree.MoveNode(twoValueNode, threeValueNode);
            Assert.AreEqual(1, tree.Root.NodeValue);
            Assert.AreEqual(2, tree.Root.Children.Count);
            Assert.AreEqual(3, tree.Root.Children[0].NodeValue);
            Assert.AreEqual(4, tree.Root.Children[1].NodeValue);
            Assert.AreEqual(4, tree.Root.Children[0].Children.Count);
            Assert.AreEqual(2, tree.Root.Children[0].Children[3].NodeValue);
        }

        [TestMethod]
        public void FirstLevelToThird()
        {
            var twoValueNode = tree.Root.Children[0];
            var lastNode = tree.Root.Children[2].Children[2];
            tree.MoveNode(twoValueNode, lastNode);
            Assert.AreEqual(1, tree.Root.NodeValue);
            Assert.AreEqual(2, tree.Root.Children.Count);
            Assert.AreEqual(3, tree.Root.Children[0].NodeValue);
            Assert.AreEqual(4, tree.Root.Children[1].NodeValue);
            Assert.AreEqual(2, tree.Root.Children[1].Children[2].Children[0].NodeValue);
        }

        [TestMethod]
        public void SecondLevelToFirst()
        {
            var firstFiveValueNode = tree.Root.Children[0].Children[0];
            tree.MoveNode(firstFiveValueNode, tree.Root);
            Assert.AreEqual(1, tree.Root.NodeValue);
            Assert.AreEqual(4, tree.Root.Children.Count);
            Assert.AreEqual(2, tree.Root.Children[0].NodeValue);
            Assert.AreEqual(3, tree.Root.Children[1].NodeValue);
            Assert.AreEqual(4, tree.Root.Children[2].NodeValue);
            Assert.AreEqual(5, tree.Root.Children[3].NodeValue);
        }

        [TestMethod]
        public void SecondLevelToSecond()
        {
            var firstFiveValueNode = tree.Root.Children[0].Children[0];
            var fourValueNode = tree.Root.Children[2];
            tree.MoveNode(firstFiveValueNode, fourValueNode);
            Assert.AreEqual(1, tree.Root.NodeValue);
            Assert.AreEqual(4, tree.Root.Children[2].Children.Count);
            Assert.AreEqual(5, tree.Root.Children[2].Children[3].NodeValue);
        }

        [TestMethod]
        public void SecondLevelToThird()
        {
            var firstFiveValueNode = tree.Root.Children[0].Children[0];
            var lastNode = tree.Root.Children[2].Children[2];
            tree.MoveNode(firstFiveValueNode, lastNode);
            Assert.AreEqual(1, tree.Root.NodeValue);
            Assert.AreEqual(3, tree.Root.Children.Count);
            Assert.AreEqual(2, tree.Root.Children[0].NodeValue);
            Assert.AreEqual(3, tree.Root.Children[1].NodeValue);
            Assert.AreEqual(4, tree.Root.Children[2].NodeValue);
            Assert.AreEqual(5, tree.Root.Children[2].Children[2].Children[0].NodeValue);
        }
    }


    public static class TreeSeeder
    {
        public static void PopulateTree(SimpleTree<int> tree)
        {
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
        }
    }
}