using System;

namespace AlgorithmsDataStructures
{
    public class DynArray<T>
    {

        private static readonly double INCREASE_RATIO = 2.0;

        private static readonly double DECREASE_RATIO = 1.5;

        private static readonly double DECREASE_CONDITION = 0.5;

        private static readonly int MIN_CAPACITY = 16;

        public T[] array;

        public int count;

        public int capacity;

        public DynArray()
        {
            count = 0;
            MakeArray(MIN_CAPACITY);
        }

        public void MakeArray(int new_capacity)
        {
            var newArray = new T[new_capacity];
            // Use count because when shrinking an array,
            // using array length will lead to not enough space in new array
            if (count > 0) Array.Copy(array, newArray, count);
            array = newArray;
            capacity = new_capacity;
        }

        public T GetItem(int index)
        {
            if (index < 0 || index >= count)
                throw new ArgumentException();

            return array[index];
        }

        public void Append(T itm)
        {
            if (count + 1 > capacity) MakeArray((int)(capacity * INCREASE_RATIO));
            array[count] = itm;
            count++;
        }

        public void Insert(T itm, int index)
        {
            // If insert to last position
            if (index == count)
            {
                Append(itm);

                return;
            }

            if (index < 0 || index > count)
                throw new ArgumentException();

            if (count + 1 > capacity)
                MakeArray((int)(capacity * INCREASE_RATIO));

            // Shift right
            for (int j = count; j > index; j--)
            {
                array[j] = array[j - 1];
            }

            // Insert the element
            array[index] = itm;
            count++;
        }

        public void Remove(int index)
        {
            if (index < 0 || index >= count)
                throw new ArgumentException();

            // Check also for unequal to min capacity to avoid redundant resizing
            if (count - 1 < (int)(capacity * DECREASE_CONDITION) && capacity != MIN_CAPACITY)
            {
                // maintain min size
                int newCapacity = capacity / DECREASE_RATIO < 16 ? 16 : (int)(capacity / DECREASE_RATIO);
                MakeArray(newCapacity);
            }

            // Do not sue loop if last element must be deleted, otherwise
            // will get OutOfBounds cause of i = index + 1
            if (index != count - 1)
            {
                // Shift left
                for (int i = index + 1; i < count; i++)
                {
                    array[i - 1] = array[i];
                }
            }
            // Reset last elements
            array[count - 1] = default;
            count--;
        }

    }
}