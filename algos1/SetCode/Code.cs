using System;

namespace AlgorithmsDataStructures
{
    public class PowerSet<T>
    {
        public readonly T[] slots;

        public int size, counter;

        public PowerSet()
        {
            size = 20000;
            counter = 0;
            slots = new T[size];
        }

        public int Size()
        {
            return counter;
        }

        public int Compare(T v1, T v2)
        {
            int result;

            if (typeof(T) == typeof(String))
            {
                // trim strings and compare them
                var str1 = v1 as string;
                var str2 = v2 as string;
                str1 = str1?.Trim();
                str2 = str2?.Trim();
                result = String.Compare(str1, str2);

                if (result < 0) result = -1;
                else if (result > 0) result = 1;
                else result = 0;
            }
            else
            {
                // use object for type casting and then to int
                var int1 = (int)(object)v1;
                var int2 = (int)(object)v2;

                if (int1 < int2) result = -1;
                else if (int1 > int2) result = 1;
                else result = 0;
            }

            return result;
            // -1 если v1 < v2
            // 0 если v1 == v2
            // +1 если v1 > v2
        }

        public int Find(T value)
        {
            int left = 0;
            int right = counter - 1;

            while (left <= right)
            {
                var idx = (right + left) / 2;
                var currentElement = slots[idx];

                if (Compare(currentElement, value) == -1) left = idx + 1;
                else if (Compare(currentElement, value) == 1) right = idx - 1;
                else return idx;
            }

            return -1;
        }

        public void Put(T value)
        {
            if (counter == size) return;

            if (counter != 0 && Compare(value, slots[counter - 1]) == 1)
            {
                slots[counter] = value;
                counter++;

                return;
            }

            int foundIndex = Find(value);

            if (foundIndex != -1) return;

            int putIdx = 0;

            for (; Compare(slots[putIdx], value) == -1 && slots[putIdx] != null; putIdx++) { }

            for (int i = counter; i != putIdx; i--)
            {
                slots[i] = slots[i - 1];
            }

            slots[putIdx] = value;
            counter++;
        }

        public bool Get(T value)
        {
            return Find(value) != -1;
        }

        public bool Remove(T value)
        {
            int idx = Find(value);

            if (idx == -1) return false;

            for (int i = idx + 1; i < counter; i++)
            {
                slots[i - 1] = slots[i];
            }

            slots[counter - 1] = default;
            counter--;

            return true;
        }

        public PowerSet<T> Intersection(PowerSet<T> set2)
        {
            var resultSet = new PowerSet<T>();

            for (int i = 0; i < counter; i++)
            {
                if (set2.Find(slots[i]) != -1) resultSet.Put(slots[i]);
            }

            return resultSet;
        }

        public PowerSet<T> Union(PowerSet<T> set2)
        {
            var resultSet = new PowerSet<T>();
            int maxLength = counter > set2.counter ? counter : set2.counter;
            int thisPointer = 0;
            int paramPointer = 0;

            while (thisPointer < maxLength && paramPointer < maxLength)
            {
                T element;

                if (slots[thisPointer] != null && (Compare(slots[thisPointer], set2.slots[paramPointer]) == -1 || set2.slots[paramPointer] == null))
                {
                    element = slots[thisPointer];
                    thisPointer++;
                }
                else
                {
                    element = set2.slots[paramPointer];
                    paramPointer++;
                }

                resultSet.Put(element);
            }

            return resultSet;
        }

        public PowerSet<T> Difference(PowerSet<T> set2)
        {
            var resultSet = new PowerSet<T>();

            for (int i = 0; i < counter; i++)
            {
                if (set2.Find(slots[i]) == -1) resultSet.Put(slots[i]);
            }

            return resultSet;
        }

        public bool IsSubset(PowerSet<T> set2)
        {
            var thisWithoutSet2 = Difference(set2);

            return set2.counter + thisWithoutSet2.counter == counter;
        }
    }
}