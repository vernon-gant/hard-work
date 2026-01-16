using AlgorithmsDataStructures2;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BinarySearchTreeTests
{
    [TestClass]
    public class TestAdd
    {
        private aBST _aBst;

        [TestInitialize]
        public void TestInitialize()
        {
            _aBst = new aBST(3);
        }

        [TestMethod]
        public void Full()
        {
            Assert.AreEqual(0, _aBst.AddKey(50));
            Assert.AreEqual(1, _aBst.AddKey(25));
            Assert.AreEqual(2, _aBst.AddKey(75));
            Assert.AreEqual(3, _aBst.AddKey(12));
            Assert.AreEqual(4, _aBst.AddKey(37));
            Assert.AreEqual(5, _aBst.AddKey(62));
            Assert.AreEqual(6, _aBst.AddKey(87));
            Assert.AreEqual(7, _aBst.AddKey(6));
            Assert.AreEqual(8, _aBst.AddKey(18));
            Assert.AreEqual(9, _aBst.AddKey(31));
            Assert.AreEqual(10, _aBst.AddKey(43));
            Assert.AreEqual(11, _aBst.AddKey(56));
            Assert.AreEqual(12, _aBst.AddKey(68));
            Assert.AreEqual(13, _aBst.AddKey(81));
            Assert.AreEqual(14, _aBst.AddKey(93));
            Assert.AreEqual(-1, _aBst.AddKey(99));
            Assert.AreEqual(-1, _aBst.AddKey(0));
        }

        [TestMethod]
        public void OnlyRight()
        {
            Assert.AreEqual(0, _aBst.AddKey(50));
            Assert.AreEqual(0, _aBst.AddKey(50));
            Assert.AreEqual(2, _aBst.AddKey(75));
            Assert.AreEqual(2, _aBst.AddKey(75));
            Assert.AreEqual(6, _aBst.AddKey(87));
            Assert.AreEqual(6, _aBst.AddKey(87));
            Assert.AreEqual(14, _aBst.AddKey(93));
            Assert.AreEqual(14, _aBst.AddKey(93));
            Assert.AreEqual(-1, _aBst.AddKey(99));
        }

        [TestMethod]
        public void OnlyLeft()
        {
            Assert.AreEqual(0, _aBst.AddKey(50));
            Assert.AreEqual(0, _aBst.AddKey(50));
            Assert.AreEqual(1, _aBst.AddKey(25));
            Assert.AreEqual(1, _aBst.AddKey(25));
            Assert.AreEqual(3, _aBst.AddKey(12));
            Assert.AreEqual(3, _aBst.AddKey(12));
            Assert.AreEqual(7, _aBst.AddKey(6));
            Assert.AreEqual(7, _aBst.AddKey(6));
            Assert.AreEqual(-1, _aBst.AddKey(-99));
        }
    }

    [TestClass]
    public class TestFind
    {
        private aBST _aBst;

        [TestInitialize]
        public void TestInitialize()
        {
            _aBst = new aBST(3);
        }

        [TestMethod]
        public void OnlyRight()
        {
            Assert.AreEqual(0, _aBst.AddKey(50));
            Assert.AreEqual(2, _aBst.AddKey(75));
            Assert.AreEqual(6, _aBst.AddKey(87));
            Assert.AreEqual(14, _aBst.AddKey(93));
            Assert.AreEqual(null, _aBst.FindKeyIndex(99));
            Assert.AreEqual(0, _aBst.FindKeyIndex(50));
            Assert.AreEqual(-1, _aBst.FindKeyIndex(25));
        }

        [TestMethod]
        public void OnlyLeft()
        {
            Assert.AreEqual(0, _aBst.AddKey(50));
            Assert.AreEqual(1, _aBst.AddKey(25));
            Assert.AreEqual(3, _aBst.AddKey(12));
            Assert.AreEqual(7, _aBst.AddKey(6));
            Assert.AreEqual(-1, _aBst.AddKey(-99));
            Assert.AreEqual(3, _aBst.FindKeyIndex(12));
            Assert.AreEqual(-4, _aBst.FindKeyIndex(37));
        }
    }
}