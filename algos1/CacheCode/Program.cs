using System;

namespace CacheCode
{
    class Program
    {

        static void Main(string[] args)
        {
            var cache = new NativeCache<string>(8);
            cache.Get("ktk");
            cache.Get("ktk");
            cache.Get("ktk");
            cache.Get("ktk");
            cache.Get("ktk");
            cache.Get("kak");
            cache.Get("kak");
            cache.Get("kbk");
            cache.Get("kbk");
            cache.Get("kbk");
            cache.Get("sfregerdgrg");
            cache.Get("ergerg");
            cache.Get("ergergerg");
            cache.Get("cvbbf");
            cache.Get("lihkuihjkh");
            cache.Get("uyuijnnn");
            
            Console.WriteLine(cache.hash1("alpha"));
            Console.WriteLine(cache.hash1("charlie"));
            Console.WriteLine(cache.hash1("golf"));
            Console.WriteLine(cache.hash1("india"));
            Console.WriteLine(cache.hash1("kilo"));
            Console.WriteLine(cache.hash1("sierra"));
            Console.WriteLine(cache.hash1("whiskey"));
            Console.WriteLine(cache.hash1("echo"));
            Console.WriteLine(cache.hash1("quebec"));
            Console.WriteLine(cache.hash1("uniform"));
            Console.WriteLine(cache.hash1("mike"));
        }

    }
}