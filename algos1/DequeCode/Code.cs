namespace AlgorithmsDataStructures
{
    public class Node<T>
    {

        public readonly T _data;

        public Node<T> _next;

        public Node<T> _prev;

        public Node(T data)
        {
            _data = data;
            _next = null;
            _prev = null;
        }

    }

    public class Deque<T>
    {

        public Node<T> _head;

        public Node<T> _tail;

        public int _size;

        public Deque()
        {
            _head = null;
            _tail = null;
            _size = 0;
        }

        public void AddFront(T item)
        {
            var newNode = new Node<T>(item);
            if (_head == null)
            {
                _head = newNode;
                _tail = newNode;
            }
            else
            {
                newNode._next = _head;
                _head._prev = newNode;
                _head = newNode;
            }
            _size++;
        }

        public void AddTail(T item)
        {
            var newNode = new Node<T>(item);
            if (_head == null)
            {
                _head = newNode;
                _tail = newNode;
            }
            else
            {
                newNode._prev = _tail;
                _tail._next = newNode;
                _tail = newNode;
            }
            _size++;
        }

        public T RemoveFront()
        {
            if (_head == null) return default;

            var oldHeadValue = _head._data;
            _head = _head._next;
            if (_head != null) _head._prev = null;
            _size--;

            if (_head == null) _tail = null;

            return oldHeadValue;
        }

        public T RemoveTail()
        {
            if (_tail == null) return default;

            var oldTailValue = _tail._data;
            _tail = _tail._prev;
            if (_tail != null) _tail._next = null;
            _size--;

            if (_tail == null) _head = null;

            return oldTailValue;
        }

        public int Size()
        {
            return _size; 
        }

    }
}