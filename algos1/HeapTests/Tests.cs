using System.Linq;

using AlgorithmsDataStructures2;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HeapTests
{
    [TestClass]
    public class MakeHeap
    {
        private Heap _heap;

        [TestInitialize]
        public void Initialize()
        {
            _heap = new Heap();
        }

        [TestMethod]
        public void EmptyArray()
        {
            _heap.MakeHeap(new int[] { }, 3);
            Assert.AreEqual(15, _heap.HeapArray.Length);
            Assert.AreEqual(0, _heap.Count);
            Assert.IsTrue(_heap.HeapArray.All(i => i == 0));
        }

        [TestMethod]
        public void FiveElementsArray()
        {
            _heap.MakeHeap(new[] { 50, 1, 100, 20, 8 }, 2);
            Assert.AreEqual(7, _heap.HeapArray.Length);
            Assert.AreEqual(5, _heap.Count);
            Assert.AreEqual(100, _heap.HeapArray[0]);
            Assert.AreEqual(20, _heap.HeapArray[1]);
            Assert.AreEqual(50, _heap.HeapArray[2]);
            Assert.AreEqual(1, _heap.HeapArray[3]);
            Assert.AreEqual(8, _heap.HeapArray[4]);
        }

        [TestMethod]
        public void TenElementsArray()
        {
            _heap.MakeHeap(new[] { 8, 6, 2, 5, 11, 4, 3, 1, 7, 9 }, 3);
            Assert.AreEqual(15, _heap.HeapArray.Length);
            Assert.AreEqual(10, _heap.Count);
            Assert.AreEqual(11, _heap.HeapArray[0]);
            Assert.AreEqual(9, _heap.HeapArray[1]);
            Assert.AreEqual(4, _heap.HeapArray[2]);
            Assert.AreEqual(7, _heap.HeapArray[3]);
            Assert.AreEqual(8, _heap.HeapArray[4]);
            Assert.AreEqual(2, _heap.HeapArray[5]);
            Assert.AreEqual(3, _heap.HeapArray[6]);
            Assert.AreEqual(1, _heap.HeapArray[7]);
            Assert.AreEqual(5, _heap.HeapArray[8]);
            Assert.AreEqual(6, _heap.HeapArray[9]);
        }
    }

    [TestClass]
    public class Add
    {
        [TestMethod]
        public void FullHeap()
        {
            var heap = new Heap();
            heap.MakeHeap(new[] { 50, 1, 100, 20, 8, 30 }, 2);
            Assert.AreEqual(7, heap.HeapArray.Length);
            Assert.AreEqual(6, heap.Count);
            Assert.AreEqual(100, heap.HeapArray[0]);
            Assert.AreEqual(20, heap.HeapArray[1]);
            Assert.AreEqual(50, heap.HeapArray[2]);
            Assert.AreEqual(1, heap.HeapArray[3]);
            Assert.AreEqual(8, heap.HeapArray[4]);
            Assert.AreEqual(30, heap.HeapArray[5]);
            Assert.IsTrue(heap.Add(40));
            Assert.AreEqual(7, heap.Count);
            Assert.AreEqual(40, heap.HeapArray[6]);
            Assert.IsFalse(heap.Add(10));
        }
    }

    [TestClass]
    public class Remove
    {
        private Heap _heap;

        [TestInitialize]
        public void Initialize()
        {
            _heap = new Heap();
        }

        [TestMethod]
        public void EmptyHeap()
        {
            Assert.AreEqual(-1, _heap.GetMax());
        }

        [TestMethod]
        public void OneElementHeap()
        {
            _heap.MakeHeap(new[] { 50 }, 0);
            Assert.AreEqual(1, _heap.Count);
            Assert.AreEqual(50, _heap.GetMax());
            Assert.AreEqual(0, _heap.Count);
        }

        [TestMethod]
        public void FiveElementHeap()
        {
            _heap.MakeHeap(new[] { 8, 6, 2, 5, 11, 4, 3, 1, 7, 9 }, 3);
            Assert.AreEqual(11, _heap.GetMax());
            Assert.AreEqual(9, _heap.Count);
            Assert.AreEqual(9, _heap.HeapArray[0]);
            Assert.AreEqual(8, _heap.HeapArray[1]);
            Assert.AreEqual(4, _heap.HeapArray[2]);
            Assert.AreEqual(7, _heap.HeapArray[3]);
            Assert.AreEqual(6, _heap.HeapArray[4]);
            Assert.AreEqual(2, _heap.HeapArray[5]);
            Assert.AreEqual(3, _heap.HeapArray[6]);
            Assert.AreEqual(1, _heap.HeapArray[7]);
            Assert.AreEqual(5, _heap.HeapArray[8]);
            Assert.AreEqual(9, _heap.GetMax());
            Assert.AreEqual(8, _heap.GetMax());
            Assert.AreEqual(7, _heap.GetMax());
            Assert.AreEqual(6, _heap.GetMax());
            Assert.AreEqual(5, _heap.GetMax());
            Assert.AreEqual(4, _heap.GetMax());
            Assert.AreEqual(3, _heap.GetMax());
            Assert.AreEqual(2, _heap.GetMax());
            Assert.AreEqual(1, _heap.GetMax());
            Assert.AreEqual(0, _heap.Count);
            Assert.IsTrue(_heap.HeapArray.All(i => i == 0));
            Assert.AreEqual(-1, _heap.GetMax());
        }
    }
}