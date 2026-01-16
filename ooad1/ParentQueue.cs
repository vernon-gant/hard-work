public class QueueBase<T>
{
    // Status codes

    public const int DEQUEUE_OK = 0;
    public const int DEQUEUE_EMPTY_QUEUE = 1;
    public const int DEQUEUE_NOT_CALLED_YET = 2;

    public const int PEEK_OK = 0;
    public const int PEEK_EMPTY_QUEUE = 1;
    public const int PEEK_NOT_CALLED_YET = 2;

    // Constructors

    // post condition : new Queue is created
    public QueueBase() { }

    // Hidden fields

    protected readonly List<T> _list = new();
    protected int _dequeueStatus = DEQUEUE_NOT_CALLED_YET;
    protected int _peekStatus = PEEK_NOT_CALLED_YET;

    // Commands

    // post condition : new element is added at the end of the queue
    public void Enqueue(T item)
    {
        _list.Add(item);
    }

    // precondition : there must be at least one element at the queue
    // post condition : first element is removed from the queue
    public void Dequeue()
    {
        if (_list.Count == 0)
        {
            _dequeueStatus = DEQUEUE_EMPTY_QUEUE;
            return;
        }

        _list.RemoveAt(0);
        _dequeueStatus = DEQUEUE_OK;
    }

    // post condition: queue becomes empty
    public void Clear()
    {
        _list.Clear();
    }


    // Queries

    // precondition : there must be at least one element at the queue
    public T Peek
    {
        get
        {
            if (_list.Count == 0)
            {
                _peekStatus = PEEK_EMPTY_QUEUE;
                return default;
            }

            _peekStatus = PEEK_OK;
            return _list[0];
        }
    }

    public int Size => _list.Count;

    public bool IsEmpty => _list.Count == 0;


    // Helper queries

    public int DequeueStatus => _dequeueStatus; // ok; empty queue; not called yet;

    public int PeekStatus => _peekStatus; // ok; empty queue; not called yet;
}

public class Queue<T> : QueueBase<T>;

public class Deque<T> : QueueBase<T>
{
    public const int DEQUEUE_BACK_OK = 0;
    public const int DEQUEUE_BACK_EMPTY_QUEUE = 1;
    public const int DEQUEUE_BACK_NOT_CALLED_YET = 2;

    public const int PEEK_BACK_OK = 0;
    public const int PEEK_BACK_EMPTY_QUEUE = 1;
    public const int PEEK_BACK_NOT_CALLED_YET = 2;
    
    // Constructors
    
    // post condition: new empty Deque is created
    public Deque() {}

    // Hidden fields

    protected int _dequeueBackStatus = DEQUEUE_BACK_NOT_CALLED_YET;
    protected int _peekBackStatus = PEEK_BACK_NOT_CALLED_YET;

    // post condition : new element is added at the beginning of the queue
    public void EnqueueFront(T item)
    {
        _list.Insert(0, item);
    }

    // precondition : there must be at least one element at the queue
    // post condition : last element is removed from the queue
    public void DequeueBack()
    {
        if (_list.Count == 0)
        {
            _dequeueBackStatus = DEQUEUE_BACK_EMPTY_QUEUE;
            return;
        }

        _list.RemoveAt(_list.Count - 1);
        _dequeueBackStatus = DEQUEUE_BACK_OK;
    }

    // precondition : there must be at least one element at the queue
    public T PeekBack
    {
        get
        {
            if (_list.Count == 0)
            {
                _peekBackStatus = PEEK_BACK_EMPTY_QUEUE;
                return default;
            }

            _peekBackStatus = PEEK_BACK_OK;
            return _list[^1];
        }
    }

    public int DequeueBackStatus => _dequeueBackStatus; // ok; empty queue; not called yet;

    public int PeekBackStatus => _peekBackStatus; // ok; empty queue; not called yet;
}