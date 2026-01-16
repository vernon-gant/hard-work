using System;
using System.Collections.Generic;

namespace AlgorithmsDataStructures
{
    public class HashTable
    {

        public int size;

        public int step;

        public int counter;

        public string[] slots;

        public HashTable(int sz, int stp)
        {
            size = sz;
            step = stp;
            counter = 0;
            slots = new string[size];
            for (int i = 0; i < size; i++) slots[i] = null;
        }

        public int HashFun(string value)
        {
            int result = 0;
            for (int i = 0; i < value.Length; i++)
            {
                result += (value[i] * (int)Math.Pow(26, value.Length - i)) % slots.Length;
            }
            return result % slots.Length;
        }

        public int SeekSlot(string value)
        {
            int idx = HashFun(value);
            while (slots[idx] != null)
            {
                if (slots[idx] == value) return -1;

                idx = (idx + step) % slots.Length;
            }
            return idx;
        }

        public int Put(string value)
        {
            if (counter == size) return -1;

            int idx = SeekSlot(value);

            if (idx == -1) return idx;

            slots[idx] = value;
            counter++;

            return idx;
        }

        public int Find(string value)
        {
            int slowIdx = HashFun(value);
            int fastIdx = (slowIdx + step) % slots.Length;
            while (slots[slowIdx] != value && slowIdx != fastIdx)
            {
                slowIdx = (slowIdx + step) % slots.Length;
                fastIdx = ((fastIdx + step) % slots.Length + step) % slots.Length;
            }
            if (slots[slowIdx] == value) return slowIdx;

            return -1;
        }

    }
}