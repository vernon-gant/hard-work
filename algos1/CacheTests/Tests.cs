using CacheCode;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CacheTests
{
    [TestClass]
    public class TestCreatePrime
    {
        [TestMethod]
        public void CreateNotPrime()
        {
            var cache = new NativeCache<string>(8);
            Assert.AreEqual(11, cache._size);
        }

        [TestMethod]
        public void CreatePrime()
        {
            var cache = new NativeCache<string>(47);
            Assert.AreEqual(47, cache._size);
        }
    }


    [TestClass]
    public class TestPut
    {
        private readonly NativeCache<string> _cache = new(11);

        [TestMethod]
        public void PutEmpty()
        {
            _cache.Put("test", "test");
            int idx = _cache.hash1("test");
            Assert.AreEqual("test", _cache._slots[idx]);
            Assert.AreEqual("test", _cache._values[idx]);
            Assert.AreEqual(1, _cache._count);
        }

        [TestMethod]
        public void PutDuplicate()
        {
            _cache.Put("test", "test");
            _cache.Put("test", "newTest");
            int idx = _cache.hash1("test");
            Assert.AreEqual("test", _cache._slots[idx]);
            Assert.AreEqual("newTest", _cache._values[idx]);
            Assert.AreEqual(1, _cache._count);
        }

        [TestMethod]
        public void PutWithCollisions()
        {
            _cache.Put("alpha", "bravo");
            _cache.Put("charlie", "delta");
            _cache.Put("golf", "hotel");
            _cache.Put("india", "juliet");
            _cache.Put("kilo", "lima");
            _cache.Put("sierra", "tango");
            _cache.Put("whiskey", "xray");

            _cache.Put("echo", "foxtrot");
            Assert.AreEqual(_cache.Get("echo"), "foxtrot");
            Assert.AreEqual(8, _cache._count);
        }

        [TestMethod]
        public void PutWhenFull()
        {
            _cache.Put("alpha", "bravo");
            _cache.Put("charlie", "delta");
            _cache.Put("golf", "hotel");
            _cache.Put("india", "juliet");
            _cache.Put("kilo", "lima");
            _cache.Put("sierra", "tango");
            _cache.Put("whiskey", "xray");
            _cache.Put("echo", "foxtrot");
            _cache.Put("quebec", "romeo");
            _cache.Put("uniform", "victor");
            _cache.Put("mike", "november");

            _cache.Get("alpha");
            _cache.Get("alpha");
            _cache.Get("alpha");
            _cache.Get("alpha");
            _cache.Get("alpha");

            _cache.Get("charlie");
            _cache.Get("charlie");
            _cache.Get("charlie");

            _cache.Get("golf");
            _cache.Get("golf");
            _cache.Get("golf");
            _cache.Get("golf");

            _cache.Get("india");
            _cache.Get("india");
            _cache.Get("india");
            _cache.Get("india");
            
            _cache.Get("kilo");
            _cache.Get("kilo");
            
            _cache.Get("sierra");
            _cache.Get("sierra");
            _cache.Get("sierra");
            _cache.Get("sierra");
            _cache.Get("sierra");
            _cache.Get("sierra");
            
            _cache.Get("whiskey");
            _cache.Get("whiskey");
            
            _cache.Get("echo");
            
            _cache.Get("quebec");
            _cache.Get("quebec");
            _cache.Get("quebec");
            
            _cache.Get("uniform");
            _cache.Get("uniform");
            _cache.Get("uniform");
            
            _cache.Get("mike");
            _cache.Get("mike");
            _cache.Get("mike");
            _cache.Get("mike");
            _cache.Get("mike");
            _cache.Get("mike");
            _cache.Get("mike");
            _cache.Get("mike");
            
            Assert.AreEqual(11, _cache._count);
            
            _cache.Put("november", "oscar");
            Assert.AreEqual(11, _cache._count);
            Assert.IsNull(_cache.Get("echo"));
            Assert.AreEqual("oscar", _cache.Get("november"));
        }
    }
}