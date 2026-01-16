public abstract class HashTable<T> {
    // Status codes

    public const int ADD_OK = 0;
    public const int ADD_FULL_HASH_TABLE = 1;
    public const int ADD_NOT_CALLED_YET = 2;

    public const int REMOVE_OK = 0;
    public const int REMOVE_NOT_FOUND = 1;
    public const int REMOVE_NOT_CALLED_YET = 2;

    // Constructors

    // post condition : new HashTable object is created with given capacity
    public HashTable(int capacity) { }


    // Commands

    // precondition : there is at least one free slot in the hash table
    // post condition : new element is added to the hash table
    public abstract void Add(T element);

    // precondition : given element is present in the hash table
    // post condition : given element is removed
    public abstract void Remove(T element);


    // post condition: hash table has 0 elements
    public abstract void Clear();


    // Queries

    public abstract int Size { get; }

    public abstract bool Contains(T element);

    public abstract bool IsFull { get; }


    // Helper queries

    public abstract int AddStatus { get; } // ok; hash table is full; not called yet;

    public abstract int RemoveStatus { get; } // ok; element not found; not called yet;
}

public class DefaultHashTable<T> : HashTable<T>
{
    // Hidden attributes

    private readonly HashSet<T> elements = new ();
    private readonly int _capacity;
    private int _addStatus = ADD_NOT_CALLED_YET;
    private int _removeStatus = REMOVE_NOT_CALLED_YET;

    public DefaultHashTable(int capacity) : base(capacity)
    {
        _capacity = capacity;
    }

    public override void Add(T element)
    {
        if (IsFull)
        {
            _addStatus = ADD_FULL_HASH_TABLE;
            return;
        }

        elements.Add(element);
        _addStatus = ADD_OK;
    }

    public override void Remove(T element)
    {
        if (!elements.Remove(element))
        {
            _removeStatus = REMOVE_NOT_FOUND;
            return;
        }

        _removeStatus = REMOVE_OK;
    }

    public override void Clear()
    {
        elements.Clear();
    }

    public override int Size => elements.Count;

    public override bool Contains(T element) => elements.Contains(element);

    public override bool IsFull => elements.Count == _capacity;

    public override int AddStatus => _addStatus;

    public override int RemoveStatus => _removeStatus;
}