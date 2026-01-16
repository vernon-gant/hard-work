using AlgorithmsDataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BloomFilterTests
{
    [TestClass]
    public class TestHash
    {

        private readonly BloomFilter _filter = new(32);

        [DataTestMethod]
        [DataRow("12345")]
        [DataRow("12345678901234567890")]
        [DataRow("1")]
        public void CorrectWork1(string input)
        {
            Assert.IsTrue(_filter.Hash1(input) < 32);
        }

        [DataTestMethod]
        [DataRow("12345")]
        [DataRow("12345678901234567890")]
        [DataRow("1")]
        public void CorrectWork2(string input)
        {
            Assert.IsTrue(_filter.Hash2(input) < 32);
        }

    }

    [TestClass]
    public class TestAdd
    {

        private readonly BloomFilter _filter = new(32);

        [DataTestMethod]
        [DataRow("0123456789")]
        [DataRow("1234567890")]
        [DataRow("8901234567")]
        [DataRow("9012345678")]
        [DataRow("2345678901")]
        [DataRow("5678901234")]
        [DataRow("6789012345")]
        [DataRow("3456789012")]
        [DataRow("4567890123")]
        [DataRow("7890123456")]
        public void SuccessSingle(string input)
        {
            int hash1 = _filter.Hash1(input);
            int hash2 = _filter.Hash2(input);
            _filter.Add(input);
            uint newValue = (uint)((1 << hash1) + (1 << hash2));
            Assert.AreEqual(_filter._bitArray, newValue);
            Assert.IsTrue(_filter.IsValue(input));
        }

        [DataTestMethod]
        [DataRow("0123456789")]
        [DataRow("1234567890")]
        [DataRow("8901234567")]
        [DataRow("9012345678")]
        [DataRow("2345678901")]
        [DataRow("5678901234")]
        [DataRow("6789012345")]
        [DataRow("3456789012")]
        [DataRow("4567890123")]
        [DataRow("7890123456")]
        public void SuccessMultiple(string input)
        {
            _filter.Add("0123123789");
            _filter.Add("1234123890");
            _filter.Add("8901123567");
            _filter.Add("9012123678");
            _filter.Add("2345123901");
            _filter.Add("5678123234");
            _filter.Add("6789123345");
            _filter.Add("3456123012");
            _filter.Add("4567123123");
            _filter.Add("7890123456");
            _filter.Add(input);
            Assert.IsTrue(_filter.IsValue(input));
        }

    }
}