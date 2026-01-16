using System;
using AlgorithmsDataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DynamicArrayTests
{
    [TestClass]
    public class TestMakeArray
    {

        private readonly DynArray<int> _array = new();

        [TestMethod]
        public void CreateArray()
        {
            Assert.AreEqual(_array.count, 0);
            Assert.AreEqual(_array.capacity, 16);
            Assert.AreEqual(_array.array.Length, 16);
        }

        [TestMethod]
        public void ManuallyGrow()
        {
            ArrayGenerator.PopulateFullArray(_array);
            _array.MakeArray(100);
            Assert.AreEqual(_array.capacity, 100);
            Assert.AreEqual(_array.array.Length, 100);
        }

    }

    [TestClass]
    public class TestGetItem
    {

        private readonly DynArray<int> _array = new();

        [DataTestMethod]
        [DataRow(-1)]
        [DataRow(16)]
        public void TestInvalidIndexes(int idx)
        {
            Assert.ThrowsException<ArgumentException>(() => _array.GetItem(idx));
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        public void TestValidIndexes(int idx)
        {
            ArrayGenerator.PopulateFullArray(_array);
            Assert.IsNotNull(_array.GetItem(idx));
        }

    }

    [TestClass]
    public class TestAppend
    {

        private readonly DynArray<int> _array = new();

        [DataTestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(1000)]
        public void TestEmptyArray(int value)
        {
            int oldCount = _array.count;
            _array.Append(value);
            Assert.AreEqual(oldCount + 1, _array.count);
            Assert.AreEqual(value, _array.array[oldCount]);
        }

        [TestMethod]
        public void TestAppendAndResize()
        {
            ArrayGenerator.PopulateFullArray(_array);
            int oldCount = _array.count;
            int oldCapacity = _array.capacity;
            _array.Append(100);
            Assert.AreEqual(oldCount + 1, _array.count);
            Assert.AreEqual(oldCapacity * 2, _array.capacity);
            Assert.AreEqual(100, _array.array[oldCount]);
        }

    }

    [TestClass]
    public class TestInsert
    {

        private readonly DynArray<int> _array = new();

        [DataTestMethod]
        [DataRow(-100)]
        [DataRow(16)]
        public void TestInvalidIndex(int idx)
        {
            Assert.ThrowsException<ArgumentException>(() => _array.GetItem(idx));
        }

        [TestMethod]
        public void TestBigBuffer()
        {
            ArrayGenerator.PopulateFullAndOneArray(_array);
            int oldCount = _array.count;
            int oldCapacity = _array.capacity;
            _array.Insert(100, 5);
            Assert.AreEqual(oldCount + 1, _array.count);
            Assert.AreEqual(oldCapacity, _array.capacity);
            Assert.AreEqual(100, _array.array[5]);
        }

        [TestMethod]
        public void TestSmallBuffer()
        {
            ArrayGenerator.PopulateFullArray(_array);
            int oldCount = _array.count;
            int oldCapacity = _array.capacity;
            _array.Insert(100, oldCount);
            Assert.AreEqual(oldCount + 1, _array.count);
            Assert.AreEqual(oldCapacity * 2, _array.capacity);
            Assert.AreEqual(100, _array.array[oldCount]);
        }

    }

    [TestClass]
    public class TestRemove
    {

        private readonly DynArray<int> _array = new();

        [DataTestMethod]
        [DataRow(-100)]
        [DataRow(16)]
        public void TestInvalidIndex(int idx)
        {
            Assert.ThrowsException<ArgumentException>(() => _array.GetItem(idx));
        }

        [TestMethod]
        public void TestNormalBufferMidElement()
        {
            ArrayGenerator.PopulateFullAndOneArray(_array);
            int oldCount = _array.count;
            int oldCapacity = _array.capacity;
            _array.Remove(5);
            Assert.AreEqual(oldCount - 1, _array.count);
            Assert.AreEqual(oldCapacity, _array.capacity);
            Assert.AreEqual(6, _array.array[5]);
        }

        [TestMethod]
        public void TestNormalBufferLastElement()
        {
            ArrayGenerator.PopulateFullArray(_array);
            int oldCount = _array.count;
            int oldCapacity = _array.capacity;
            Assert.AreEqual(15, _array.array[15]);
            _array.Remove(15);
            Assert.AreEqual(oldCount - 1, _array.count);
            Assert.AreEqual(oldCapacity, _array.capacity);
            Assert.AreEqual(default(int), _array.array[15]);
        }

        [TestMethod]
        public void TestBigBuffer()
        {
            ArrayGenerator.PopulateFullAndOneArray(_array);
            int oldCount = _array.count;
            int oldCapacity = _array.capacity;
            _array.Remove(oldCount - 1);
            Assert.AreEqual(oldCount - 1, _array.count);
            Assert.AreEqual(oldCapacity, _array.capacity);
            Assert.AreEqual(default, _array.array[oldCount - 1]);
            oldCount = _array.count;
            oldCapacity = _array.capacity;
            _array.Remove(oldCount - 1);
            Assert.AreEqual(oldCount - 1, _array.count);
            Assert.AreEqual((int)(oldCapacity / 1.5), _array.capacity);
            Assert.AreEqual(default, _array.array[oldCount - 1]);
        }

    }


    public static class ArrayGenerator
    {

        public static void PopulateFullArray(DynArray<int> array)
        {
            for (int i = 0; i < 16; i++)
            {
                array.Append(i);
            }
        }

        public static void PopulateFullAndOneArray(DynArray<int> array)
        {
            for (int i = 0; i < 17; i++)
            {
                array.Append(i);
            }
        }

    }
}