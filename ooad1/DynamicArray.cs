public abstract class DynamicArray<T>
{
    protected const int DEFAULT_CAPACITY = 16;

    // Constructors

    // post condition : new DynamicArray with default capacity is created
    public DynamicArray() { }

    // precondition : capacity > 0
    // post condition : new DynamicArray with given capacity is created
    public DynamicArray(int capacity) { }


    // Commands

    // post condition : new element is added after the last element of the list or at 0 position if there are no elements
    public abstract void AddBack(T item);

    // post condition : new element is added at the beginning of the array at position 0
    public abstract void AddFront(T item);

    // precondition : idx must be in range of 0<=idx<Count
    // post condition: element at given position is removed. If there are any elements to the right, we shift them all to one position left so that the cell of removed element is taken.
    public abstract void Remove(int idx);

    // post condition: all elements are removed from the array and its capacity is set to default one + all statuses are reset
    public abstract void Clear();


    // Queries

    // precondition : idx must be in range of 0<=idx<Count
    public abstract T Get(int idx);

    public abstract int Count();

    public abstract int Capacity();

    public abstract int FindFirst(T value);


    // Helper queries

    public abstract int RemoveStatus { get; } // ok; idx out of range; not called yet;

    public abstract int GetStatus { get; } // ok; idx out of range; not called yet;

    public abstract int FindStatus { get; } // ok; not found; not called yet;
}

public class DefaultDynamicArray<T> : DynamicArray<T>
{
    private T[] _items;
    private int _count;

    private int _removeStatus;
    private int _getStatus;
    private int _findStatus;

    public const int REMOVE_OK = 0;
    public const int REMOVE_IDX_OUT_OF_RANGE = 1;
    public const int REMOVE_NULL = 2;

    public const int GET_OK = 0;
    public const int GET_IDX_OUT_OF_RANGE = 1;
    public const int GET_NULL = 2;

    public const int FIND_OK = 0;
    public const int FIND_NOT_FOUND = 1;
    public const int FIND_NULL = 2;

    public DefaultDynamicArray() : this(DEFAULT_CAPACITY)
    {
    }

    public DefaultDynamicArray(int capacity)
    {
        _items = new T[capacity];
        _count = 0;
        _removeStatus = REMOVE_NULL;
        _getStatus = GET_NULL;
        _findStatus = FIND_NULL;
    }

    public override void AddBack(T item)
    {
        EnsureCapacity();
        _items[_count++] = item;
    }

    public override void AddFront(T item)
    {
        EnsureCapacity();
        for (int i = _count; i > 0; i--)
        {
            _items[i] = _items[i - 1];
        }

        _items[0] = item;
        _count++;
    }

    private void EnsureCapacity()
    {
        if (_count + 1 <= _items.Length) return;

        T[] newItems = new T[_items.Length * 2];
        Array.Copy(_items, newItems, _count);
        _items = newItems;
    }

    public override void Remove(int idx)
    {
        if (idx < 0 || idx >= _count)
        {
            _removeStatus = REMOVE_IDX_OUT_OF_RANGE;
            return;
        }

        for (int i = idx; i < _count - 1; i++)
        {
            _items[i] = _items[i + 1];
        }

        _items[--_count] = default!;
        _removeStatus = REMOVE_OK;

        if (_count * 2 >= _items.Length) return;

        T[] newItems = new T[_items.Length / 1.5 > DEFAULT_CAPACITY ? (int)(_items.Length / 1.5) : DEFAULT_CAPACITY];
        Array.Copy(_items, newItems, _count);
        _items = newItems;
    }

    public override void Clear()
    {
        _items = new T[DEFAULT_CAPACITY];
        _count = 0;
        _removeStatus = REMOVE_NULL;
        _getStatus = GET_NULL;
        _findStatus = FIND_NULL;
    }

    public override T Get(int idx)
    {
        if (idx < 0 || idx >= _count)
        {
            _getStatus = GET_IDX_OUT_OF_RANGE;
            return default!;
        }

        _getStatus = GET_OK;
        return _items[idx];
    }

    public override int FindFirst(T value)
    {
        for (int i = 0; i < _count; i++)
        {
            if (_items[i]!.Equals(value))
            {
                _findStatus = FIND_OK;
                return i;
            }
        }

        _findStatus = FIND_NOT_FOUND;
        return -1;
    }

    public override int Count() => _count;

    public override int Capacity() => _items.Length;

    public override int RemoveStatus => _removeStatus;
    public override int GetStatus => _getStatus;
    public override int FindStatus => _findStatus;
}