using System;

namespace AlgorithmsDataStructures
{

    public class NativeDictionary<T>
    {
        public int size;
        public string [] slots;
        public T [] values;

        public NativeDictionary(int sz)
        {
            size = sz;
            slots = new string[size];
            values = new T[size];
        }

        public int HashFun(string key)
        {
            int result = 0;
            for (int i = 0; i < key.Length; i++)
            {
                result += (key[i] * (int)Math.Pow(26, key.Length - i)) % slots.Length;
            }
            return result % slots.Length;
        }

        public bool IsKey(string key)
        {
            int slowIdx = HashFun(key);
            int fastIdx = (slowIdx + 1) % slots.Length;
            while (slots[slowIdx] != key && slowIdx != fastIdx)
            {
                slowIdx = (slowIdx + 1) % slots.Length;
                fastIdx = ((fastIdx + 1) % slots.Length + 1) % slots.Length;
            }
            if (slots[slowIdx] == key) return true;

            return false;
        }

        public void Put(string key, T value)
        {
            int idx = HashFun(key);
            while (slots[idx] != null && slots[idx] != key)
            {
                idx = (idx + 1) % slots.Length;
            }
            slots[idx] = key;
            values[idx] = value;
        }

        public T Get(string key)
        {
            int slowIdx = HashFun(key);
            int fastIdx = (slowIdx + 1) % slots.Length;
            while (slots[slowIdx] != key && slowIdx != fastIdx)
            {
                slowIdx = (slowIdx + 1) % slots.Length;
                fastIdx = ((fastIdx + 1) % slots.Length + 1) % slots.Length;
            }
            if (slots[slowIdx] == key) return values[slowIdx];

            return default;
        }
    }
}