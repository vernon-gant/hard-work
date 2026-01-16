using System;
using AlgorithmsDataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StackTests
{
    [TestClass]
    public class TestSize
    {

        private readonly Stack<int> _stack = new();

        [TestMethod]
        public void EmptyStack()
        {
            Assert.AreEqual(_stack.Size(), 0);
        }

        [TestMethod]
        public void FullStack()
        {
            StackGenerator.GenerateManyIntElements(_stack);
            Assert.AreEqual(_stack.Size(), 10);
        }

    }

    [TestClass]
    public class TestPop
    {

        private readonly Stack<String> _stack = new();

        [TestMethod]
        public void EmptyStack()
        {
            Assert.AreEqual(_stack.Pop(), null);
        }

        [TestMethod]
        public void FullStack()
        {
            StackGenerator.GenerateManyStringElements(_stack);
            var afterHead = _stack._head._next;
            Assert.AreEqual(_stack.Pop(), "I love you 9!!");
            Assert.AreSame(_stack._head, afterHead);
        }

    }

    [TestClass]
    public class TestPush
    {

        private readonly Stack<String> _stack = new();

        [TestMethod]
        public void EmptyStack()
        {
            _stack.Push("Hello World!");
            Assert.IsNotNull(_stack._head);
            Assert.AreEqual(_stack._head._data, "Hello World!");
        }

        [TestMethod]
        public void FullStack()
        {
            StackGenerator.GenerateManyStringElements(_stack);
            var oldHead = _stack._head;
            _stack.Push("Hello World!");
            Assert.AreEqual(_stack._head._data, "Hello World!");
            Assert.AreSame(_stack._head._next, oldHead);
        }

        [TestMethod]
        public void NullPush()
        {
            StackGenerator.GenerateManyStringElements(_stack);
            var head = _stack._head;
            _stack.Push(null);
            Assert.AreSame(_stack._head, head);
        }

    }
    
    [TestClass]
    public class TestPeek
    {

        private readonly Stack<String> _stack = new();

        [TestMethod]
        public void EmptyStack()
        {
            Assert.AreEqual(_stack.Peek(), null);
        }

        [TestMethod]
        public void FullStack()
        {
            StackGenerator.GenerateManyStringElements(_stack);
            var head = _stack._head;
            Assert.AreEqual(_stack.Peek(), "I love you 9!!");
            Assert.AreSame(_stack._head, head);
        }

    }

    static class StackGenerator
    {

        public static void GenerateManyIntElements(Stack<int> stack)
        {
            for (int i = 0; i < 10; i++)
            {
                stack.Push(i);
            }
        }

        public static void GenerateManyStringElements(Stack<String> stack)
        {
            for (int i = 0; i < 10; i++)
            {
                stack.Push($"I love you {i}!!");
            }
        }

    }
}