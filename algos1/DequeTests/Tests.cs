using AlgorithmsDataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DequeTests
{
    [TestClass]
    public class AddFrontTests
    {

        private readonly Deque<int> _deque = new();

        [TestMethod]
        public void Empty()
        {
            _deque.AddFront(1);
            Assert.IsNotNull(_deque._head);
            Assert.IsNotNull(_deque._tail);
            Assert.AreSame(_deque._head, _deque._tail);
            Assert.AreEqual(_deque._head._data, 1);
            Assert.AreEqual(_deque.Size(), 1);
        }

        [TestMethod]
        public void OneNode()
        {
            _deque.AddFront(1);
            
            _deque.AddFront(2);
            Assert.IsNotNull(_deque._head);
            Assert.IsNotNull(_deque._tail);
            Assert.AreNotSame(_deque._head, _deque._tail);
            Assert.AreSame(_deque._head._next, _deque._tail);
            Assert.AreSame(_deque._head, _deque._tail._prev);
            Assert.AreEqual(_deque._head._data, 2);
            Assert.AreEqual(_deque.Size(), 2);
        }

        [TestMethod]
        public void FullQueue()
        {
            DequeGenerator.PopulateQueue(_deque);
            var oldHead = _deque._head;
            _deque.AddFront(10);
            Assert.AreSame(_deque._head._next, oldHead);
            Assert.AreEqual(_deque._head._data, 10);
            Assert.AreEqual(_deque.Size(), 11);
        }

    }

    [TestClass]
    public class AddTailTests
    {

        private readonly Deque<int> _deque = new();

        [TestMethod]
        public void Empty()
        {
            _deque.AddTail(1);
            Assert.IsNotNull(_deque._head);
            Assert.IsNotNull(_deque._tail);
            Assert.AreSame(_deque._head, _deque._tail);
            Assert.AreEqual(_deque._tail._data, 1);
            Assert.IsNull(_deque._tail._next);
            Assert.AreEqual(_deque.Size(), 1);
        }

        [TestMethod]
        public void OneNode()
        {
            _deque.AddFront(1);
            _deque.AddTail(2);
            Assert.AreNotSame(_deque._head, _deque._tail);
            Assert.AreSame(_deque._head._next, _deque._tail);
            Assert.AreSame(_deque._tail._prev, _deque._head);
            Assert.AreEqual(_deque._tail._data, 2);
            Assert.IsNull(_deque._tail._next);
            Assert.AreEqual(_deque.Size(), 2);
        }

        [TestMethod]
        public void FullQueue()
        {
            DequeGenerator.PopulateStack(_deque);
            var oldTail = _deque._tail;
            _deque.AddTail(10);
            Assert.AreSame(_deque._tail._prev, oldTail);
            Assert.IsNull(_deque._tail._next);
            Assert.AreEqual(_deque._tail._data, 10);
            Assert.AreEqual(_deque.Size(), 11);
        }

    }

    [TestClass]
    public class RemoveFrontTests
    {

        private readonly Deque<int> _deque = new();

        [TestMethod]
        public void Empty()
        {
            var removed = _deque.RemoveFront();
            Assert.IsNull(_deque._head);
            Assert.IsNull(_deque._tail);
            Assert.AreEqual(removed, 0);
            Assert.AreEqual(_deque.Size(), 0);
        }

        [TestMethod]
        public void OneNode()
        {
            _deque.AddFront(1);
            var oldHead = _deque._head;
            var removedValue = _deque.RemoveFront();
            Assert.IsNull(_deque._head);
            Assert.IsNull(_deque._tail);
            Assert.AreEqual(removedValue, oldHead._data);
            Assert.AreEqual(_deque.Size(), 0);
        }
        
        [TestMethod]
        public void TwoNodes()
        {
            _deque.AddFront(1);
            _deque.AddFront(2);
            var oldHead = _deque._head;
            var removedValue = _deque.RemoveFront();
            Assert.AreSame(_deque._head,_deque._tail);
            Assert.AreNotSame(oldHead,_deque._head);
            Assert.IsNull(_deque._head._prev);
            Assert.IsNull(_deque._head._next);
            Assert.AreEqual(removedValue, oldHead._data);
            Assert.AreEqual(_deque.Size(), 1);
        }

        [TestMethod]
        public void FullQueue()
        {
            DequeGenerator.PopulateQueue(_deque);
            var oldHead = _deque._head;
            var removedValue = _deque.RemoveFront();
            Assert.AreNotSame(oldHead,_deque._head);
            Assert.AreSame(oldHead._next,_deque._head);
            Assert.IsNull(_deque._head._prev);
            Assert.AreEqual(removedValue, oldHead._data);
            Assert.AreEqual(_deque.Size(), 9);
        }

    }

    [TestClass]
    public class RemoveTailTests
    {

        private readonly Deque<int> _deque = new();

        [TestMethod]
        public void Empty()
        {
            var removed = _deque.RemoveTail();
            Assert.IsNull(_deque._head);
            Assert.IsNull(_deque._tail);
            Assert.AreEqual(removed, 0);
            Assert.AreEqual(_deque.Size(), 0);
        }

        [TestMethod]
        public void OneNode()
        {
            _deque.AddTail(1);
            var oldTail = _deque._tail;
            var removedValue = _deque.RemoveTail();
            Assert.IsNull(_deque._head);
            Assert.IsNull(_deque._tail);
            Assert.AreEqual(removedValue, oldTail._data);
            Assert.AreEqual(_deque.Size(), 0);
        }
        
        [TestMethod]
        public void TwoNodes()
        {
            _deque.AddTail(1);
            _deque.AddTail(2);
            var oldHead = _deque._tail;
            var removedValue = _deque.RemoveTail();
            Assert.AreSame(_deque._head,_deque._tail);
            Assert.IsNull(_deque._tail._prev);
            Assert.IsNull(_deque._tail._next);
            Assert.AreEqual(removedValue, oldHead._data);
            Assert.AreEqual(_deque.Size(), 1);
        }

        [TestMethod]
        public void FullQueue()
        {
            DequeGenerator.PopulateQueue(_deque);
            var oldTail = _deque._tail;
            var removedValue = _deque.RemoveTail();
            Assert.AreSame(oldTail._prev,_deque._tail);
            Assert.IsNull(_deque._tail._next);
            Assert.AreEqual(removedValue, oldTail._data);
            Assert.AreEqual(_deque.Size(), 9);
        }

    }

    public static class DequeGenerator
    {

        public static void PopulateQueue(Deque<int> deque)
        {
            for (int i = 0; i < 10; i++)
            {
                deque.AddTail(i);
            }
        }

        public static void PopulateStack(Deque<int> deque)
        {
            for (int i = 0; i < 10; i++)
            {
                deque.AddFront(i);
            }
        }

    }
}