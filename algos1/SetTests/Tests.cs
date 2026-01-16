using System;
using System.Diagnostics;
using System.Linq;
using AlgorithmsDataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SetTests
{
    [TestClass]
    public class TestPut
    {

        private readonly PowerSet<string> _set = new();

        [TestMethod]
        public void Empty()
        {
            _set.Put("1");
            Assert.AreEqual(_set.Size(), 1);
            Assert.AreEqual(_set.slots[0], "1");
            _set.Put("1");
            Assert.AreEqual(_set.Size(), 1);
        }

        [TestMethod]
        public void PutManyWithDuplicateCheck()
        {
            SetGenerator.From0To10000(_set);
            Assert.AreEqual(_set.Size(), 10000);
            SetGenerator.From0To10000(_set);
            Assert.AreEqual(_set.Size(), 10000);
            for (int i = 1; i < _set.Size(); i++)
            {
                Assert.IsTrue(_set.Compare(_set.slots[i], _set.slots[i - 1]) == 1);
            }
        }

    }

    [TestClass]
    public class TestRemove
    {

        private readonly PowerSet<string> _set = new();

        [TestMethod]
        public void OneElementSet()
        {
            _set.Put("1");
            Assert.AreEqual(_set.Size(), 1);
            Assert.IsTrue(_set.Remove("1"));
            Assert.AreEqual(_set.Size(), 0);
        }

        [TestMethod]
        public void ManyElementSet()
        {
            _set.Put("Aleks");
            _set.Put("Bober");
            _set.Put("Caesar");
            _set.Put("Dima");
            _set.Put("Test");
            Assert.AreEqual(_set.Size(), 5);
            Assert.IsTrue(_set.Remove("Bober"));
            Assert.AreEqual(_set.Size(), 4);
            Assert.IsNull(_set.slots[4]);
            Assert.IsFalse(_set.Get("Bober"));
            Assert.IsFalse(_set.Remove("Undefined"));
            Assert.AreEqual(_set.Size(), 4);
        }

    }

    [TestClass]
    public class TestIntersection
    {

        private readonly PowerSet<string> _set1 = new();

        private readonly PowerSet<string> _set2 = new();

        [TestMethod]
        public void NotEmptyResult()
        {
            SetGenerator.From0To20000(_set1);
            SetGenerator.From0To10000(_set2);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var resultSet = _set1.Intersection(_set2);
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;

            Assert.IsTrue(ts.TotalMilliseconds < 1500,
                          $"Execution Time of Intersection exceeded 1,5 seconds: {ts.TotalMilliseconds} ms");
            Assert.AreEqual(resultSet.Size(), 10000);
            for (int i = 0; i < 10000; i++)
            {
                Assert.IsTrue(resultSet.Get($"{i}"));
            }
        }

        [TestMethod]
        public void EmptyResult()
        {
            SetGenerator.From0To10000(_set1);
            SetGenerator.From10000To20000(_set2);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var resultSet = _set1.Intersection(_set2);
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;

            Assert.IsTrue(ts.TotalMilliseconds < 1500,
                          $"Execution Time of Intersection exceeded 1,5 seconds: {ts.TotalMilliseconds} ms");
            Assert.AreEqual(resultSet.Size(), 0);
        }

        [TestMethod]
        public void TwoEmptySets()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var resultSet = _set1.Intersection(_set2);
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;

            Assert.IsTrue(ts.TotalMilliseconds < 1500,
                          $"Execution Time of Intersection exceeded 1,5 seconds: {ts.TotalMilliseconds} ms");
            Assert.AreEqual(resultSet.Size(), 0);
        }

        [TestMethod]
        public void OneEmptySecondNot()
        {
            SetGenerator.From0To10000(_set1);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var resultSet = _set1.Intersection(_set2);
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;

            Assert.IsTrue(ts.TotalMilliseconds < 1500,
                          $"Execution Time of Intersection exceeded 1,5 seconds: {ts.TotalMilliseconds} ms");
            Assert.AreEqual(resultSet.Size(), 0);
        }

        [TestMethod]
        public void OneEmptySecondNot2()
        {
            SetGenerator.From0To10000(_set2);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var resultSet = _set1.Intersection(_set2);
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;

            Assert.IsTrue(ts.TotalMilliseconds < 1500,
                          $"Execution Time of Intersection exceeded 1,5 seconds: {ts.TotalMilliseconds} ms");
            Assert.AreEqual(resultSet.Size(), 0);
        }

    }

    [TestClass]
    public class TestUnion
    {

        private readonly PowerSet<string> _set1 = new();

        private readonly PowerSet<string> _set2 = new();

        [TestMethod]
        public void TwoDistinctSets()
        {
            SetGenerator.From0To10000(_set1);
            SetGenerator.From10000To20000(_set2);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var resultSet = _set1.Union(_set2);
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;

            Assert.IsTrue(ts.TotalMilliseconds < 1500,
                          $"Execution Time of Intersection exceeded 1,5 seconds: {ts.TotalMilliseconds} ms");

            Assert.AreEqual(resultSet.Size(), 20000);
            for (int i = 0; i < 20000; i++)
            {
                Assert.IsTrue(resultSet.Get($"{i}"));
            }
        }

        [TestMethod]
        public void OneIsSubset()
        {
            SetGenerator.From0To10000(_set1);
            SetGenerator.From0To20000(_set2);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var resultSet = _set1.Union(_set2);
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;

            Assert.IsTrue(ts.TotalMilliseconds < 1500,
                          $"Execution Time of Intersection exceeded 1,5 seconds: {ts.TotalMilliseconds} ms");

            Assert.AreEqual(resultSet.Size(), 20000);
            for (int i = 0; i < 20000; i++)
            {
                Assert.IsTrue(resultSet.Get($"{i}"));
            }
        }

        [TestMethod]
        public void OneEmpty()
        {
            SetGenerator.From0To10000(_set1);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var resultSet = _set1.Union(_set2);
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;

            Assert.IsTrue(ts.TotalMilliseconds < 1500,
                          $"Execution Time of Intersection exceeded 1,5 seconds: {ts.TotalMilliseconds} ms");

            Assert.AreEqual(resultSet.Size(), 10000);
            for (int i = 0; i < 10000; i++)
            {
                Assert.IsTrue(resultSet.Get($"{i}"));
            }
        }
        
        [TestMethod]
        public void OneEmpty2()
        {
            SetGenerator.From0To10000(_set2);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var resultSet = _set1.Union(_set2);
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;

            Assert.IsTrue(ts.TotalMilliseconds < 1500,
                          $"Execution Time of Intersection exceeded 1,5 seconds: {ts.TotalMilliseconds} ms");

            Assert.AreEqual(resultSet.Size(), 10000);
            for (int i = 0; i < 10000; i++)
            {
                Assert.IsTrue(resultSet.Get($"{i}"));
            }
        }

    }
    
    [TestClass]
    public class TestDifference
    {

        private readonly PowerSet<string> _set1 = new();

        private readonly PowerSet<string> _set2 = new();

        [TestMethod]
        public void NotEmptyResult()
        {
            SetGenerator.From0To20000(_set1);
            SetGenerator.From10000To20000(_set2);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var resultSet = _set1.Difference(_set2);
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;

            Assert.IsTrue(ts.TotalMilliseconds < 1500,
                          $"Execution Time of Intersection exceeded 1,5 seconds: {ts.TotalMilliseconds} ms");
            Assert.AreEqual(resultSet.Size(), 10000);
            for (int i = 0; i < 10000; i++)
            {
                Assert.IsTrue(resultSet.Get($"{i}"));
            }
        }
        
        [TestMethod]
        public void SetFromWhichSubtractedEmpty()
        {
            SetGenerator.From10000To20000(_set2);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var resultSet = _set1.Difference(_set2);
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;

            Assert.IsTrue(ts.TotalMilliseconds < 1500,
                          $"Execution Time of Intersection exceeded 1,5 seconds: {ts.TotalMilliseconds} ms");
            Assert.AreEqual(resultSet.Size(), 0);
        }
        
        [TestMethod]
        public void SetWhichIsSubtracted()
        {
            SetGenerator.From10000To20000(_set1);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var resultSet = _set1.Difference(_set2);
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;

            Assert.IsTrue(ts.TotalMilliseconds < 1500,
                          $"Execution Time of Intersection exceeded 1,5 seconds: {ts.TotalMilliseconds} ms");
            Assert.AreEqual(resultSet.Size(), 10000);
        }

    }
    
    [TestClass]
    public class TestIsSubset
    {

        private readonly PowerSet<string> _set1 = new();

        private readonly PowerSet<string> _set2 = new();

        [TestMethod]
        public void True()
        {
            SetGenerator.From0To20000(_set1);
            SetGenerator.From10000To20000(_set2);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = _set1.IsSubset(_set2);
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;

            Assert.IsTrue(ts.TotalMilliseconds < 1500,
                          $"Execution Time of Intersection exceeded 1,5 seconds: {ts.TotalMilliseconds} ms");
            Assert.IsTrue(result);
        }
        
        [TestMethod]
        public void FalseBothNotEmpty()
        {
            SetGenerator.From0To10000(_set1);
            SetGenerator.From0To20000(_set2);
            for (int i = 0; i < 500; i++)
            {
                _set2.Put($"{i}");
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = _set1.IsSubset(_set2);
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;

            Assert.IsTrue(ts.TotalMilliseconds < 1500,
                          $"Execution Time of Intersection exceeded 1,5 seconds: {ts.TotalMilliseconds} ms");
            Assert.IsFalse(result);
        }
        
        [TestMethod]
        public void BothEmpty()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = _set1.IsSubset(_set2);
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;

            Assert.IsTrue(ts.TotalMilliseconds < 1500,
                          $"Execution Time of Intersection exceeded 1,5 seconds: {ts.TotalMilliseconds} ms");
            Assert.IsFalse(result);
        }
        
        [TestMethod]
        public void ParamEmpty()
        {
            SetGenerator.From0To10000(_set1);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = _set1.IsSubset(_set2);
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;

            Assert.IsTrue(ts.TotalMilliseconds < 1500,
                          $"Execution Time of Intersection exceeded 1,5 seconds: {ts.TotalMilliseconds} ms");
            Assert.IsTrue(result);
        }

    }

    public static class SetGenerator
    {

        public static void From0To10000(PowerSet<string> powerSet)
        {
            for (int i = 9999; i >= 0; i--)
            {
                powerSet.Put($"{i}");
            }
        }

        public static void From10000To20000(PowerSet<string> powerSet)
        {
            for (int i = 19999; i >= 10000; i--)
            {
                powerSet.Put($"{i}");
            }
        }

        public static void From0To20000(PowerSet<string> powerSet)
        {
            for (int i = 0; i < 20000; i++)
            {
                powerSet.Put($"{i}");
            }
        }

    }
}