public abstract class Queue<T>
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
    public Queue() { }


    // Commands

    // post condition : new element is added at the end of the queue
    public abstract void Enqueue(T item);

    // precondition : there must be at least one element at the queue
    // post condition : first element is removed from the queue
    public abstract void Dequeue();

    // post condition: queue becomes empty
    public abstract void Clear();


    // Queries

    // precondition : there must be at least one element at the queue
    public abstract T Peek { get; }

    public abstract int Size { get; }

    public abstract bool IsEmpty { get; }


    // Helper queries

    public abstract int DequeueStatus { get; } // ok; empty queue; not called yet;

    public abstract int PeekStatus { get; } // ok; empty queue; not called yet;
}

public class TwoStackQueue<T> : Queue<T>
{
    private readonly Stack<T> _enqueueStack = new();
    private readonly Stack<T> _dequeueStack = new();

    private int _dequeueStatus = DEQUEUE_NOT_CALLED_YET;
    private int _peekStatus = PEEK_NOT_CALLED_YET;

    // Commands

    // post condition : new element is added at the end of the queue
    public override void Enqueue(T item)
    {
        _enqueueStack.Push(item);
    }

    // precondition : there must be at least one element at the queue
    // post condition : first element is removed from the queue
    public override void Dequeue()
    {
        if (IsEmpty)
        {
            _dequeueStatus = DEQUEUE_EMPTY_QUEUE;
            return;
        }

        EnsureDequeueStackNotEmpty();

        _dequeueStack.Pop();
        _dequeueStatus = DEQUEUE_OK;
    }

    // post condition: queue becomes empty
    public override void Clear()
    {
        _enqueueStack.Clear();
        _dequeueStack.Clear();
        _dequeueStatus = DEQUEUE_NOT_CALLED_YET;
        _peekStatus = PEEK_NOT_CALLED_YET;
    }

    // precondition : there must be at least one element at the queue
    public override T Peek
    {
        get
        {
            if (IsEmpty)
            {
                _peekStatus = PEEK_EMPTY_QUEUE;
                return default!;
            }

            EnsureDequeueStackNotEmpty();

            _peekStatus = PEEK_OK;
            return _dequeueStack.Peek();
        }
    }

    public override bool IsEmpty => _enqueueStack.Count == 0 && _dequeueStack.Count == 0;

    public override int Size => _enqueueStack.Count + _dequeueStack.Count;

    // Helper queries

    public override int DequeueStatus => _dequeueStatus;

    public override int PeekStatus => _peekStatus;

    private void EnsureDequeueStackNotEmpty()
    {
        if (_dequeueStack.Count != 0) return;

        while (_enqueueStack.Count > 0)
        {
            _dequeueStack.Push(_enqueueStack.Pop());
        }
    }
}