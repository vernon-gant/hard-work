using System.Linq;
using AlgorithmsDataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DictionaryTests
{
    [TestClass]
    public class TestPut
    {

        private readonly NativeDictionary<int> _dictionary = new(19);

        [TestMethod]
        public void Empty()
        {
            string key = "one";
            int value = 1;
            int idx = _dictionary.HashFun(key);
            _dictionary.Put(key, value);
            Assert.AreEqual(_dictionary.slots[idx],key);
            Assert.AreEqual(_dictionary.values[idx],value);
        }
        
        [TestMethod]
        public void NotEmptyPutNewPair()
        {
            DictionaryGenerator.PopulateDict(_dictionary);
            string key = "aleks";
            int value = 100;
            int idx = _dictionary.HashFun(key);
            _dictionary.Put(key, value);
            Assert.AreEqual(_dictionary.slots[idx],key);
            Assert.AreEqual(_dictionary.values[idx],value);
        }
        
        [DataTestMethod]
        [DataRow("one")]
        [DataRow("two")]
        [DataRow("five")]
        public void NotEmptyPutExistingKey(string key)
        {
            DictionaryGenerator.PopulateDict(_dictionary);
            int oldValue = _dictionary.Get(key);
            int newValue = 100;
            int idx = _dictionary.HashFun(key);
            _dictionary.Put(key, newValue);
            Assert.AreEqual(_dictionary.slots[idx],key);
            Assert.AreEqual(_dictionary.values[idx],newValue);
            Assert.IsTrue(_dictionary.values.All(value => value != oldValue));
        }
    }
    
    [TestClass]
    public class TestIsKey
    {
        
        private readonly NativeDictionary<int> _dictionary = new(19);

        [DataTestMethod]
        [DataRow("one")]
        [DataRow("test")]
        [DataRow("aleks")]
        public void Empty(string key)
        {
            Assert.IsFalse(_dictionary.IsKey(key));
        }
        
        [DataTestMethod]
        [DataRow("haris")]
        [DataRow("test")]
        [DataRow("aleks")]
        public void NotEmptyAbsentKey(string key)
        {
            DictionaryGenerator.PopulateDict(_dictionary);
            Assert.IsFalse(_dictionary.IsKey(key));
        }
        
        [DataTestMethod]
        [DataRow("one")]
        [DataRow("three")]
        [DataRow("five")]
        public void NotEmptyPreseneKey(string key)
        {
            DictionaryGenerator.PopulateDict(_dictionary);
            Assert.IsTrue(_dictionary.IsKey(key));
        }
    }
    
    [TestClass]
    public class TestGet
    {

        private readonly NativeDictionary<int> _dictionary = new(19);

        [DataTestMethod]
        [DataRow("one")]
        [DataRow("test")]
        [DataRow("aleks")]
        public void Empty(string key)
        {
            Assert.AreEqual(_dictionary.Get(key),0);
        }
        
        [DataTestMethod]
        [DataRow("haris")]
        [DataRow("test")]
        [DataRow("aleks")]
        public void NotEmptyAbsentKey(string key)
        {
            DictionaryGenerator.PopulateDict(_dictionary);
            Assert.AreEqual(_dictionary.Get(key),0);
        }
        
        [DataTestMethod]
        [DataRow("one",1)]
        [DataRow("three",3)]
        [DataRow("five",5)]
        public void NotEmptyPresentKeyValue(string key, int value)
        {
            DictionaryGenerator.PopulateDict(_dictionary);
            Assert.AreEqual(_dictionary.Get(key), value);
        }

    }

    public static class DictionaryGenerator
    {

        public static void PopulateDict(NativeDictionary<int> dictionary)
        {
            dictionary.Put("one",1);
            dictionary.Put("two",2);
            dictionary.Put("three",3);
            dictionary.Put("four",4);
            dictionary.Put("five",5);
        }

    }
}