using System;
using AlgorithmsDataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QueueTests
{
    [TestClass]
    public class TestEnqueue
    {

        private readonly Queue<int> _queue = new();

        [TestMethod]
        public void EmptyQueue()
        {
            _queue.Enqueue(1);
            Assert.AreEqual(_queue._start, 0);
            Assert.AreEqual(_queue._size, 1);
            Assert.AreEqual(_queue._buffer[_queue._start], 1);
        }

        [TestMethod]
        public void HalfFullQueue()
        {
            for (int i = 0; i < 5; i++)
            {
                _queue.Enqueue(i);
            }
            Assert.AreEqual(_queue._start, 0);
            Assert.AreEqual(_queue._size, 5);
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual(_queue._buffer[_queue._start] + i, i);
            }
        }

        [TestMethod]
        public void StartIdxAtTheEnd()
        {
            QueueGenerator.PopulateMany(_queue);
            for (int i = 0; i < 9; i++) _queue.Dequeue();
            Assert.AreEqual(_queue._start, 9);
            Assert.AreEqual(_queue._size, 1);
            _queue.Enqueue(11);
            _queue.Enqueue(12);
            _queue.Enqueue(13);
            _queue.Enqueue(14);
            Assert.AreEqual(_queue._start, 9);
            Assert.AreEqual(_queue._size, 5);
            Assert.AreEqual(_queue._buffer[0], 11);
            Assert.AreEqual(_queue._buffer[1], 12);
            Assert.AreEqual(_queue._buffer[2], 13);
            Assert.AreEqual(_queue._buffer[3], 14);
        }

        [TestMethod]
        public void FullQueue()
        {
            QueueGenerator.PopulateMany(_queue);
            Assert.ThrowsException<Exception>(() => _queue.Enqueue(1));
        }

    }

    [TestClass]
    public class TestDequeue
    {

        private readonly Queue<string> _queue = new();

        [TestMethod]
        public void EmptyQueue()
        {
            Assert.AreEqual(_queue.Dequeue(), null);
        }

        [TestMethod]
        public void FullQueueHalfDequeue()
        {
            QueueGenerator.PopulateMany(_queue);
            Assert.AreEqual(_queue._start, 0);
            Assert.AreEqual(_queue._size, 10);
            for (int i = 0; i < 5; i++)
            {
                Assert.AreEqual($"{i}", _queue.Dequeue());
            }
            Assert.AreEqual(_queue._start, 5);
            Assert.AreEqual(_queue._size, 5);
        }

        [TestMethod]
        public void StartIdxAtTheEnd()
        {
            QueueGenerator.PopulateMany(_queue);
            for (int i = 0; i < 9; i++) _queue.Dequeue();
            for (int i = 0; i < 5; i++) _queue.Enqueue($"{i}");
            Assert.AreEqual(_queue._start, 9);
            Assert.AreEqual(_queue._size, 6);
            var front = _queue.Dequeue();
            Assert.AreEqual(front, "9");
            Assert.AreEqual(_queue._buffer[9], null);
            Assert.AreEqual(_queue._start,0);
            Assert.AreEqual(_queue._size, 5);
        }

    }

    [TestClass]
    public class TestSize
    {

        private readonly Queue<int> _queue = new();
        
        [TestMethod]
        public void Empty()
        {
            Assert.AreEqual(_queue.Size(),0);
        }
        
        [TestMethod]
        public void Full()
        {
            QueueGenerator.PopulateMany(_queue);
            Assert.AreEqual(_queue.Size(),10);
        }

    }

    public static class QueueGenerator
    {

        public static void PopulateMany(Queue<int> queue)
        {
            for (int i = 0; i < 10; i++)
            {
                queue.Enqueue(i);
            }
        }

        public static void PopulateMany(Queue<string> queue)
        {
            for (int i = 0; i < 10; i++)
            {
                queue.Enqueue($"{i}");
            }
        }

    }
}