using System;
using AlgorithmsDataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HashTableTests
{
    [TestClass]
    public class TestHash
    {

        private readonly HashTable _hashTable = new(19, 1);

        [TestMethod]
        public void TestEmpty()
        {
            Assert.AreEqual(_hashTable.HashFun(""), 0);
        }
        
        [DataTestMethod]
        [DataRow("2039enghubntoerbn")]
        [DataRow("12345678901234567890")]
        [DataRow("hbdvudksbvruegbnrhutihgbiurebvuierbvieu")]
        public void TestSuccess(string value)
        {
            Assert.AreNotEqual(_hashTable.HashFun(value), 0);
        }

        [TestMethod]
        public void TestConsistency()
        {
            int result = _hashTable.HashFun("TEST");
            Assert.AreEqual(_hashTable.HashFun("TEST"), result);
        }

        [TestMethod]
        public void TestDifferentHashValues()
        {
            int first = _hashTable.HashFun("ALEKSANDR");
            int second = _hashTable.HashFun("RAFIK");
            Assert.AreNotEqual(first, second);
        }

    }

    [TestClass]
    public class TestSeekSlot
    {

        private readonly HashTable _hashTable = new(19, 3);

        [TestMethod]
        public void TestSuccessEmpty()
        {
            var idx = _hashTable.SeekSlot("0123456789");
            Assert.AreNotEqual(idx, -1);
        }
        
        [TestMethod]
        public void TestSuccessNotEmpty()
        {
            _hashTable.Put("Hello");
            _hashTable.Put("My");
            _hashTable.Put("Friend!");
            var idx = _hashTable.SeekSlot("TEST");
            Assert.AreNotEqual(idx, -1);
        }

        [TestMethod]
        public void TestSuccessSameHashValue()
        {
            int idx = _hashTable.Put("Etergerb");
            Assert.AreEqual(_hashTable.SeekSlot("aeqrew"), idx + 3);
        }

        [TestMethod]
        public void TestFailSameInput()
        {
            _hashTable.Put("TEST");
            Assert.AreEqual(_hashTable.SeekSlot("TEST"), -1);
        }

    }

    [TestClass]
    public class TestFind
    {

        private readonly HashTable _hashTable = new(19, 10);

        [TestMethod]
        public void TestSuccess()
        {
            _hashTable.Put("TEST");
            Assert.AreNotEqual(_hashTable.Find("TEST"), -1);
        }

        [TestMethod]
        public void TestSuccessSameHashValue()
        {
            int idx = _hashTable.Put("Etergerb");
            _hashTable.slots[idx - 1] = "aeqrew";
            Assert.AreEqual(_hashTable.Find("aeqrew"), idx - 1);
        }

        [TestMethod]
        public void TestFailEmpty()
        {
            Assert.AreEqual(_hashTable.Find("TEST"), -1);
        }
        
        [TestMethod]
        public void TestFailNotEmpty()
        {
            _hashTable.Put("Hello");
            _hashTable.Put("My");
            _hashTable.Put("Friend!");
            Assert.AreEqual(_hashTable.Find("TEST"), -1);
        }

    }

    [TestClass]
    public class TestPut
    {

        private readonly HashTable _hashTable = new(19, 1);

        [TestMethod]
        public void TestSuccess()
        {
            int idx = _hashTable.Put("TEST");
            Assert.AreEqual(_hashTable.counter, 1);
            Assert.AreEqual(_hashTable.Find("TEST"), idx);
        }

        [TestMethod]
        public void TestFailureSame()
        {
            int idx = _hashTable.Put("TEST");
            Assert.AreEqual(_hashTable.Put("TEST"), -1);
        }

        [TestMethod]
        public void TestFailFull()
        {
            HashTableGenerator.PopulateFull(_hashTable);
            Assert.AreEqual(_hashTable.Put("TEST"), -1);
        }

    }

    public static class HashTableGenerator
    {

        public static void PopulateFull(HashTable hashTable)
        {
            for (int i = 0; i < 19; i++)
            {
                int intValue = 65 + i;
                hashTable.Put($"{(char)intValue}");
            }
        }

    }
}