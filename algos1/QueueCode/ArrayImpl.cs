using System;

namespace AlgorithmsDataStructures
{
    public class Queue<T>
    {

        private static readonly int MAX_BUFFER_SIZE = 10000;

        public readonly T[] _buffer;

        public int _start, _size;

        public Queue()
        {
            _buffer = new T[MAX_BUFFER_SIZE];
            _start = 0;
            _size = 0;
        }

        public void Enqueue(T item)
        {
            // We expext queue not to overflow our buffer
            if (_size == MAX_BUFFER_SIZE) throw new InvalidOperationException();

            int idx = (_start + _size) % MAX_BUFFER_SIZE;

            _buffer[idx] = item;
            _size++;
        }

        public T Dequeue()
        {
            if (_size == 0) return default;

            var first = _buffer[_start];
            _buffer[_start] = default;

            _start = (_start + 1) % MAX_BUFFER_SIZE;
            _size--;

            return first;
        }

        public int Size()
        {
            return _size;
        }

    }
}