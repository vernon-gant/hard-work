using System.Collections.Generic;
using System;
using System.Collections;
using System.IO;

namespace AlgorithmsDataStructures
{
    public class BloomFilter
    {

        public int filter_len;

        public uint _bitArray;

        public BloomFilter(int f_len)
        {
            filter_len = f_len;
            _bitArray = 0;
        }

        // хэш-функции
        public int Hash1(string str1)
        {
            int result = 0;
            for (int i = 0; i < str1.Length; i++)
            {
                int code = str1[i];
                result = (result * 17 + code) % 32;
            }
            return result;
        }

        public int Hash2(string str1)
        {
            int result = 0;
            for (int i = 0; i < str1.Length; i++)
            {
                int code = str1[i];
                result = (result * 223 + code) % 32;
            }
            return result;
        }

        public void Add(string str1)
        {
            int hash1 = Hash1(str1);
            int hash2 = Hash2(str1);
            _bitArray |= (uint)(1 << hash1);
            _bitArray |= (uint)(1 << hash2);
        }

        public bool IsValue(string str1)
        {
            int hash1 = Hash1(str1);
            int hash2 = Hash2(str1);
            return (_bitArray & (uint)(1 << hash1)) != 0 &&
                   (_bitArray & (uint)(1 << hash2)) != 0;
        }

    }
}