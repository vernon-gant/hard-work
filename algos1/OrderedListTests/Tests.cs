using AlgorithmsDataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OrderedListTests
{
    [TestClass]
    public class TestCompare
    {

        [TestMethod]
        public void Integers()
        {
            var _list = new OrderedList<int>(true);
            var int1 = 10;
            var int2 = 20;
            Assert.AreEqual(_list.Compare(int1, int2), -1);
            Assert.AreEqual(_list.Compare(int2, int1), 1);
            Assert.AreEqual(_list.Compare(int1, int1), 0);
        }

        [TestMethod]
        public void Strings()
        {
            var _list = new OrderedList<string>(true);
            var str1 = "Aleks";
            var str2 = "Test";
            Assert.AreEqual(_list.Compare(str1, str2), -1);
            Assert.AreEqual(_list.Compare(str2, str1), 1);
            Assert.AreEqual(_list.Compare(str1, str1), 0);
        }

    }

    [TestClass]
    public class TestAddAsc
    {

        private readonly OrderedList<int> _list = new(true);


        [TestMethod]
        public void InitialEmpty()
        {
            _list.Add(1);
            Assert.AreEqual(_list.head.value, 1);
            Assert.AreEqual(_list.tail.value, 1);
            Assert.AreSame(_list.head, _list.tail);
            Assert.AreEqual(_list.Count(), 1);
        }

        [TestMethod]
        public void InitialOneElementAddToEnd()
        {
            _list.Add(1);
            _list.Add(2);
            Assert.AreEqual(_list.head.value, 1);
            Assert.AreEqual(_list.tail.value, 2);
            Assert.AreNotSame(_list.head, _list.tail);
            Assert.AreEqual(_list.Count(), 2);
        }

        [TestMethod]
        public void InitialOneElementAddToStart()
        {
            _list.Add(1);
            _list.Add(-1);
            Assert.AreEqual(_list.head.value, -1);
            Assert.AreEqual(_list.tail.value, 1);
            Assert.AreNotSame(_list.head, _list.tail);
            Assert.AreEqual(_list.Count(), 2);
        }

        [TestMethod]
        public void ManyElementsMiddle()
        {
            OrderedListGenerator.PopulateMany(_list);
            _list.Add(25);
            var newElement = _list.Find(25);
            Assert.IsNotNull(newElement);
            Assert.AreEqual(newElement.prev.value, 20);
            Assert.AreEqual(newElement.next.value, 30);
            Assert.AreEqual(_list.Count(), 11);
        }

    }

    [TestClass]
    public class TestAddDesc
    {

        private readonly OrderedList<int> _list = new(false);


        [TestMethod]
        public void InitialEmpty()
        {
            _list.Add(1);
            Assert.AreEqual(_list.head.value, 1);
            Assert.AreEqual(_list.tail.value, 1);
            Assert.AreSame(_list.head, _list.tail);
            Assert.AreEqual(_list.Count(), 1);
        }

        [TestMethod]
        public void InitialOneElementAddToStart()
        {
            _list.Add(1);
            _list.Add(2);
            Assert.AreEqual(_list.head.value, 2);
            Assert.AreEqual(_list.tail.value, 1);
            Assert.AreNotSame(_list.head, _list.tail);
            Assert.AreEqual(_list.Count(), 2);
        }

        [TestMethod]
        public void InitialOneElementAddToEnd()
        {
            _list.Add(1);
            _list.Add(-1);
            Assert.AreEqual(_list.head.value, 1);
            Assert.AreEqual(_list.tail.value, -1);
            Assert.AreNotSame(_list.head, _list.tail);
            Assert.AreEqual(_list.Count(), 2);
        }

        [TestMethod]
        public void ManyElementsMiddle()
        {
            OrderedListGenerator.PopulateMany(_list);
            _list.Add(25);
            var newElement = _list.Find(25);
            Assert.IsNotNull(newElement);
            Assert.AreEqual(newElement.prev.value, 30);
            Assert.AreEqual(newElement.next.value, 20);
            Assert.AreEqual(_list.Count(), 11);
        }

    }

    [TestClass]
    public class TestFind
    {

        private readonly OrderedList<int> _list = new(true);

        [TestMethod]
        public void EmptyList()
        {
            Assert.IsNull(_list.Find(100));
        }

        [TestMethod]
        public void NullSearch()
        {
            var stringList = new OrderedList<string>(true);
            Assert.IsNull(stringList.Find(null));
        }

        [DataRow(10)]
        [DataRow(50)]
        [DataRow(90)]
        [DataTestMethod]
        public void SuccessSearch(int value)
        {
            OrderedListGenerator.PopulateMany(_list);
            var foundNode = _list.Find(value);
            Assert.IsNotNull(foundNode);
            Assert.AreEqual(foundNode.value, value);
        }

        [DataRow(75)]
        [DataRow(-50)]
        [DataRow(900)]
        [DataTestMethod]
        public void FailedSearch(int value)
        {
            OrderedListGenerator.PopulateMany(_list);
            var foundNode = _list.Find(value);
            Assert.IsNull(foundNode);
        }

    }

    [TestClass]
    public class TestDelete
    {

        private readonly OrderedList<int> _list = new(false);
        
        [TestMethod]
        public void DeleteNull()
        {
            var stringList = new OrderedList<string>(true);
            stringList.Add("1");
            stringList.Add("2");
            stringList.Add("3");
            stringList.Add("4");
            stringList.Delete(null);
            Assert.AreEqual(stringList.Count(), 4);
        }

        [TestMethod]
        public void OneElement()
        {
            _list.Add(1);
            _list.Delete(1);
            Assert.AreEqual(_list.Count(), 0);
            Assert.IsNull(_list.head);
            Assert.IsNull(_list.tail);
        }

        [TestMethod]
        public void TwoElementsDeleteFirst()
        {
            _list.Add(1);
            _list.Add(2);
            _list.Delete(1);
            Assert.AreEqual(_list.Count(), 1);
            Assert.AreSame(_list.head, _list.tail);
        }

        [TestMethod]
        public void TwoElementsDeleteSecond()
        {
            _list.Add(1);
            _list.Add(2);
            _list.Delete(2);
            Assert.AreEqual(_list.Count(), 1);
            Assert.AreSame(_list.head, _list.tail);
        }

        [DataRow(10)]
        [DataRow(50)]
        [DataRow(80)]
        [DataTestMethod]
        public void SuccessDelete(int value)
        {
            OrderedListGenerator.PopulateMany(_list);
            var foundNode = _list.Find(value);
            _list.Delete(value);
            Assert.AreEqual(_list.Count(), 9);
            Assert.AreSame(foundNode.prev.next, foundNode.next);
            Assert.AreSame(foundNode.next.prev, foundNode.prev);
        }
        
        [DataRow(0)]
        [DataRow(50)]
        [DataRow(80)]
        [DataTestMethod]
        public void DeleteWithDuplicates(int value)
        {
            OrderedListGenerator.PopulateMany(_list);
            _list.Add(value);
            _list.Add(value);
            _list.Delete(value);
            Assert.IsNotNull(_list.Find(value));
            _list.Delete(value);
            Assert.IsNotNull(_list.Find(value));
            _list.Delete(value);
            Assert.IsNull(_list.Find(value));
        }

        [DataRow(75)]
        [DataRow(-50)]
        [DataRow(900)]
        [DataTestMethod]
        public void FailedDelete(int value)
        {
            OrderedListGenerator.PopulateMany(_list);
            _list.Delete(value);
            Assert.AreEqual(_list.Count(), 10);
        }

    }

    public static class OrderedListGenerator
    {

        public static void PopulateMany(OrderedList<int> list)
        {
            for (int i = 0; i < 100; i += 10)
            {
                list.Add(i);
            }
        }

    }
}