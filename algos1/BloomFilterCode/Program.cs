using System;
using System.Reflection.Metadata;
using AlgorithmsDataStructures;

namespace BloomFilterCode
{
    class Program
    {

        static void Main(string[] args)
        {
            var filter = new BloomFilter(32);
            filter.Add("0123456789");
            filter.Add("1234567890");
            filter.Add("8901234567");
            filter.Add("9012345678");
            Console.WriteLine(filter.IsValue("0123456789"));
            Console.WriteLine(filter.IsValue("1234567890"));
            Console.WriteLine(filter.IsValue("8901234567"));
            Console.WriteLine(filter.IsValue("9012345678"));
        }

    }
}