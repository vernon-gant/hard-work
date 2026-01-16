using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AlgorithmsDataStructures;

namespace DoublyLinkedListTests
{
    [TestClass]
    public class RemoveTests
    {

        private readonly LinkedList2 _linkedList = new();

        [DataTestMethod]
        [DataRow(-100)]
        [DataRow(0)]
        [DataRow(11)]
        public void TestEmptyList(int value)
        {
            Assert.IsFalse(_linkedList.Remove(value));
        }

        [DataTestMethod]
        [DataRow(-100)]
        [DataRow(0)]
        [DataRow(11)]
        public void TestListManyNodesAndAbsentValues(int value)
        {
            ListGenerator.PopulateListWithManyNodes(_linkedList);
            Assert.IsFalse(_linkedList.Remove(value));
        }

        [DataTestMethod]
        [DataRow(2)]
        [DataRow(7)]
        [DataRow(9)]
        public void TestListManyNodesAndPresentValues(int value)
        {
            ListGenerator.PopulateListWithManyNodes(_linkedList);
            var deleted = _linkedList.Find(value);
            Assert.IsTrue(_linkedList.Remove(value));
            Assert.IsFalse(_linkedList.Remove(value));
            Assert.AreSame(deleted.prev.next, deleted.next);
            Assert.AreSame(deleted.next.prev, deleted.prev);
        }

        [TestMethod]
        public void TestListManyNodesFirstValue()
        {
            ListGenerator.PopulateListWithManyNodes(_linkedList);
            var afterFirst = _linkedList.head.next;
            Assert.IsTrue(_linkedList.Remove(1));
            Assert.IsFalse(_linkedList.Remove(1));
            Assert.AreSame(_linkedList.head, afterFirst);
            Assert.IsNull(_linkedList.head.prev);
        }

        [TestMethod]
        public void TestListManyNodesLastValue()
        {
            ListGenerator.PopulateListWithManyNodes(_linkedList);
            Assert.IsTrue(_linkedList.Remove(10));
            Assert.IsFalse(_linkedList.Remove(10));
            Assert.AreEqual(_linkedList.tail.value, 9);
            Assert.IsNull(_linkedList.tail.next);
        }

        [DataTestMethod]
        [DataRow(-100)]
        [DataRow(0)]
        [DataRow(11)]
        public void TestListOneNodeAndAbsentValues(int value)
        {
            ListGenerator.PopulateListWithOneNode(_linkedList);
            Assert.IsFalse(_linkedList.Remove(value));
        }

        [TestMethod]
        public void TestListOneNodeAndPresentValue()
        {
            ListGenerator.PopulateListWithOneNode(_linkedList);
            Assert.IsTrue(_linkedList.Remove(5));
            Assert.IsFalse(_linkedList.Remove(5));
            Assert.IsNull(_linkedList.head);
            Assert.IsNull(_linkedList.tail);
        }

    }

    [TestClass]
    public class RemoveAllTests
    {

        private readonly LinkedList2 _linkedList = new();

        [DataTestMethod]
        [DataRow(-100)]
        [DataRow(0)]
        [DataRow(11)]
        public void TestEmptyList(int value)
        {
            _linkedList.RemoveAll(value);
            Assert.IsNull(_linkedList.Find(value));
        }

        [DataTestMethod]
        [DataRow(-100)]
        [DataRow(0)]
        [DataRow(11)]
        public void TestListWithDulpicatesAndAbsentValues(int value)
        {
            ListGenerator.PopulateListWithDuplicates(_linkedList);
            _linkedList.RemoveAll(value);
            Assert.IsNull(_linkedList.Find(value));
        }

        [DataTestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(5)]
        [DataRow(8)]
        [DataRow(10)]
        public void TestListWithDuplicatesAndPresentValues(int value)
        {
            ListGenerator.PopulateListWithDuplicates(_linkedList);
            _linkedList.RemoveAll(value);
            Assert.IsNull(_linkedList.Find(value));
        }

        [TestMethod]
        public void TestListSameDuplicates()
        {
            ListGenerator.PopulateListWithSameDuplicates(_linkedList);
            _linkedList.RemoveAll(1);
            Assert.IsNull(_linkedList.Find(1));
            Assert.IsNull(_linkedList.head);
            Assert.IsNull(_linkedList.tail);
        }

    }

    [TestClass]
    public class ClearTests
    {

        private readonly LinkedList2 _linkedList = new();

        [TestMethod]
        public void TestEmptyList()
        {
            _linkedList.Clear();
            Assert.IsNull(_linkedList.head);
            Assert.IsNull(_linkedList.tail);
        }

        [TestMethod]
        public void TestListManyNodes()
        {
            ListGenerator.PopulateListWithManyNodes(_linkedList);
            _linkedList.Clear();
            Assert.IsNull(_linkedList.head);
            Assert.IsNull(_linkedList.tail);
        }

        [TestMethod]
        public void TestListOneNode()
        {
            ListGenerator.PopulateListWithOneNode(_linkedList);
            _linkedList.Clear();
            Assert.IsNull(_linkedList.head);
            Assert.IsNull(_linkedList.tail);
        }

    }

    [TestClass]
    public class FindAllTests
    {

        private readonly LinkedList2 _linkedList = new();

        [DataTestMethod]
        [DataRow(-100)]
        [DataRow(0)]
        [DataRow(11)]
        public void TestEmptyList(int value)
        {
            var result = _linkedList.FindAll(value);
            Assert.AreEqual(0, result.Count);
        }

        [DataTestMethod]
        [DataRow(-100)]
        [DataRow(0)]
        [DataRow(11)]
        public void TestListWithDulpicatesAndAbsentValues(int value)
        {
            ListGenerator.PopulateListWithDuplicates(_linkedList);
            var result = _linkedList.FindAll(value);
            Assert.AreEqual(0, result.Count);
        }

        [DataTestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(5)]
        [DataRow(8)]
        [DataRow(10)]
        public void TestListWithDuplicatesAndPresentValues(int value)
        {
            ListGenerator.PopulateListWithDuplicates(_linkedList);
            var result = _linkedList.FindAll(value);
            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.All(v => v.value == value));
        }

        [TestMethod]
        public void TestListSameDuplicates()
        {
            ListGenerator.PopulateListWithSameDuplicates(_linkedList);
            var result = _linkedList.FindAll(1);
            Assert.AreEqual(4, result.Count);
            Assert.IsTrue(result.All(v => v.value == 1));
        }

    }

    [TestClass]
    public class CountTests
    {

        private readonly LinkedList2 _linkedList = new();

        [TestMethod]
        public void TestEmptyList()
        {
            var result = _linkedList.Count();
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestListWithManyValues()
        {
            ListGenerator.PopulateListWithManyNodes(_linkedList);
            var result = _linkedList.Count();
            Assert.AreEqual(10, result);
        }

        [TestMethod]
        public void TestListWithOneValue()
        {
            ListGenerator.PopulateListWithOneNode(_linkedList);
            var result = _linkedList.Count();
            Assert.AreEqual(1, result);
        }

    }

    [TestClass]
    public class InsertAfterTests
    {

        private readonly LinkedList2 _linkedList = new();

        [TestMethod]
        public void TestEmptyList()
        {
            var node = new Node(1);
            _linkedList.InsertAfter(null, node);
            Assert.AreSame(node, _linkedList.Find(1));
            Assert.IsNull(node.prev);
            Assert.IsNull(node.next);
            Assert.AreSame(node, _linkedList.head);
            Assert.AreSame(node, _linkedList.tail);
        }

        // [TestMethod]
        // public void TestListManyNodesAfterFirstNode()
        // {
        //     ListGenerator.PopulateListWithManyNodes(_linkedList);
        //     var afterHead = _linkedList.head.next;
        //     var toInsert = new Node(100);
        //     _linkedList.InsertAfter(_linkedList.head, toInsert);
        //     Assert.AreSame(_linkedList.head.next, toInsert);
        //     Assert.AreSame(toInsert.next, afterHead);
        // }

        [DataTestMethod]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(5)]
        [DataRow(9)]
        public void TestListManyNodesOrdinaryNode(int value)
        {
            ListGenerator.PopulateListWithManyNodes(_linkedList);
            var toInsert = new Node(100);
            var beforeInsert = _linkedList.Find(value);
            var afterInsert = beforeInsert.next;
            _linkedList.InsertAfter(beforeInsert, toInsert);
            Assert.AreSame(beforeInsert.next, toInsert);
            Assert.AreSame(toInsert.prev, beforeInsert);
            Assert.AreSame(toInsert.next, afterInsert);
            Assert.AreSame(afterInsert.prev, toInsert);
        }

        [TestMethod]
        public void TestListManyNodesAfterLastNode()
        {
            ListGenerator.PopulateListWithManyNodes(_linkedList);
            var oldTail = _linkedList.tail;
            var toInsert = new Node(100);
            _linkedList.InsertAfter(oldTail, toInsert);
            Assert.AreSame(oldTail.next, toInsert);
            Assert.AreSame(toInsert.prev, oldTail);
            Assert.AreSame(_linkedList.tail, toInsert);
            Assert.IsNull(toInsert.next);
        }

        [TestMethod]
        public void TestListOneNode()
        {
            ListGenerator.PopulateListWithOneNode(_linkedList);
            var toInsert = new Node(100);
            var head = _linkedList.head;
            _linkedList.InsertAfter(head, toInsert);
            Assert.AreSame(head.next, toInsert);
            Assert.AreSame(toInsert.prev, head);
            Assert.AreSame(_linkedList.tail, toInsert);
            Assert.IsNull(toInsert.next);
        }

    }

    public static class ListGenerator
    {

        public static void PopulateListWithManyNodes(LinkedList2 linkedList)
        {
            linkedList.AddInTail(new Node(1));
            linkedList.AddInTail(new Node(2));
            linkedList.AddInTail(new Node(3));
            linkedList.AddInTail(new Node(4));
            linkedList.AddInTail(new Node(5));
            linkedList.AddInTail(new Node(6));
            linkedList.AddInTail(new Node(7));
            linkedList.AddInTail(new Node(8));
            linkedList.AddInTail(new Node(9));
            linkedList.AddInTail(new Node(10));
        }

        public static void PopulateListWithDuplicates(LinkedList2 linkedList)
        {
            linkedList.AddInTail(new Node(1));
            linkedList.AddInTail(new Node(1));
            linkedList.AddInTail(new Node(1));
            linkedList.AddInTail(new Node(2));
            linkedList.AddInTail(new Node(2));
            linkedList.AddInTail(new Node(2));
            linkedList.AddInTail(new Node(3));
            linkedList.AddInTail(new Node(4));
            linkedList.AddInTail(new Node(5));
            linkedList.AddInTail(new Node(5));
            linkedList.AddInTail(new Node(5));
            linkedList.AddInTail(new Node(6));
            linkedList.AddInTail(new Node(7));
            linkedList.AddInTail(new Node(8));
            linkedList.AddInTail(new Node(8));
            linkedList.AddInTail(new Node(8));
            linkedList.AddInTail(new Node(9));
            linkedList.AddInTail(new Node(10));
            linkedList.AddInTail(new Node(10));
            linkedList.AddInTail(new Node(10));
        }

        public static void PopulateListWithSameDuplicates(LinkedList2 linkedList)
        {
            linkedList.AddInTail(new Node(1));
            linkedList.AddInTail(new Node(1));
            linkedList.AddInTail(new Node(1));
            linkedList.AddInTail(new Node(1));
        }

        public static void PopulateListWithOneNode(LinkedList2 linkedList)
        {
            linkedList.AddInTail(new Node(5));
        }

    }
}