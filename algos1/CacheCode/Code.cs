using System;

namespace CacheCode
{
    public class NativeCache<T>
    {

        public int _size;

        public int _count;

        public String[] _slots;

        public T[] _values;

        public int[] _hits;

        public NativeCache(int size)
        {
            _size = GetNextPrime(size);
            _slots = new string[_size];
            _values = new T[_size];
            _hits = new int[_size];
        }

        public int hash1(string input)
        {
            const int basePrime = 2089;
            int hash = 0;
            for (int i = 0; i < input.Length; i++)
            {
                hash = (hash * basePrime + input[i]) % _size;
            }
            return hash;
        }

        public int hash2(string input)
        {
            const int secondaryPrime = 103;
            int hash = 0;
            for (int i = 0; i < input.Length; i++)
            {
                hash = (hash * secondaryPrime + input[i]) % (_size - 1);
            }
            return 1 + hash;
        }

        private static int GetNextPrime(int size)
        {
            if (IsPrime(size)) return size;

            for (int i = size | 1; i < int.MaxValue; i += 2)
            {
                if (IsPrime(i)) return i;
            }
            return size;
        }

        private static bool IsPrime(int candidate)
        {
            if (candidate % 2 != 0)
            {
                int limit = (int)Math.Sqrt(candidate);
                for (int divisor = 3; divisor <= limit; divisor += 2)
                {
                    if (candidate % divisor == 0) return false;
                }
                return true;
            }
            return candidate == 2;
        }


        public T Get(string key)
        {
            for (int i = 0; i < _size - 1; i++)
            {
                int idx = (hash1(key) + i * hash2(key)) % _size;
                if (_slots[idx] == key)
                {
                    _hits[idx]++;
                    return _values[idx];
                }
            }
            return default;
        }

        public void Put(string key, T value)
        {
            if (IsFull())
            {
                int idx = ClearElementWithSmallestHits();
                _slots[idx] = null;
                _values[idx] = default;
                _hits[idx] = 0;
                _count--;
            }

            put(key, value);
        }

        private int ClearElementWithSmallestHits()
        {
            int smallestHit = int.MaxValue;
            int smallestHitIdx = 0;
            for (int i = 0; i < _size; i++)
            {
                if (_hits[i] == 0 && _slots[i] != null) return i;

                if (_hits[i] < smallestHit)
                {
                    smallestHit = _hits[i];
                    smallestHitIdx = i;
                }
            }
            return smallestHitIdx;
        }

        private void put(string key, T value)
        {
            int idx = hash1(key);
            int i = 1;
            int step = hash2(key);
            while (_slots[idx] != null && _slots[idx] != key)
            {
                idx = (idx + i * step) % _size;
                i++;
            }
            if (_slots[idx] == null) _count++;
            _slots[idx] = key;
            _values[idx] = value;
        }

        public bool IsFull()
        {
            return _count == _size;
        }

    }
}