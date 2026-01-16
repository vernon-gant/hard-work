public class ParentHashTable<T>
{
    // Status codes

    public const int ADD_OK = 0;
    public const int ADD_FULL = 1;
    public const int ADD_NOT_CALLED_YET = 2;

    public const int REMOVE_OK = 0;
    public const int REMOVE_NOT_FOUND = 1;
    public const int REMOVE_NOT_CALLED_YET = 2;

    // Hidden attributes

    protected HashSet<T> elements;
    protected int _capacity;
    protected int _addStatus = ADD_NOT_CALLED_YET;
    protected int _removeStatus = REMOVE_NOT_CALLED_YET;

    // Constructors

    protected ParentHashTable() { }

    // post condition : new HashTable object is created with given capacity
    public ParentHashTable(int capacity)
    {
        elements = new HashSet<T>(capacity);
        _capacity = capacity;
    }


    // Commands

    // precondition : there is at least one free slot in the hash table
    // post condition : new element is added to the hash table
    public void Add(T element)
    {
        if (IsFull)
        {
            _addStatus = ADD_FULL;
            return;
        }

        elements.Add(element);
        _addStatus = ADD_OK;
    }

    // precondition : given element is present in the hash table
    // post condition : given element is removed
    public void Remove(T element)
    {
        if (!elements.Remove(element))
        {
            _removeStatus = REMOVE_NOT_FOUND;
            return;
        }

        _removeStatus = REMOVE_OK;
    }

    // post condition: hash table has 0 elements
    public void Clear()
    {
        elements.Clear();
    }


    // Queries

    public int Size => elements.Count;

    public bool Contains(T element) => elements.Contains(element);

    public bool IsFull => elements.Count == _capacity;


    // Helper queries

    public int AddStatus => _addStatus;

    public int RemoveStatus => _removeStatus;
}

public class PowerSet<T> : ParentHashTable<T>
{
    // Constructors

    // post condition : new PowerSet object is created with given capacity
    public PowerSet(int capacity) : base(capacity) { }

    private PowerSet(HashSet<T> elements, int capacity)
    {
        _capacity = capacity;
        this.elements = elements;
    }

    // Queries

    public PowerSet<T> Intersection(PowerSet<T> set2) => new(elements.Intersect(set2.elements).ToHashSet(), _capacity);

    public PowerSet<T> Union(PowerSet<T> set2) => new(elements.Union(set2.elements).ToHashSet(), _capacity);

    public PowerSet<T> Difference(PowerSet<T> set2) => new(elements.Except(set2.elements).ToHashSet(), _capacity);

    public bool IsSubset(PowerSet<T> set2) => elements.IsSubsetOf(set2.elements);
}