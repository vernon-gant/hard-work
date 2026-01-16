public abstract class NativeDictionary<T> {
    // Status codes

    public const int REMOVE_OK = 0;
    public const int REMOVE_NOT_FOUND = 1;
    public const int REMOVE_NOT_CALLED_YET = 2;
    
    public const int GET_OK = 0;
    public const int GET_NOT_FOUND = 1;
    public const int GET_NOT_CALLED_YET = 2;

    // Constructors

    // post condition : new NativeDictionary is created
    public NativeDictionary() { }


    // Commands

    // post condition : value for given key is written to the dictionary
    public abstract void Put(string key, T value);

    // precondition : key value pair with given key is present in the dictionary
    // post condition : key value pair is removed
    public abstract void Remove(string key);


    // post condition: dictionary has 0 elements
    public abstract void Clear();


    // Queries
    
    // precondition : key is present in the dictionary
    public abstract T Get(string key);

    public abstract int Size { get; }

    public abstract bool Contains(string key);


    // Helper queries

    public abstract int RemoveStatus { get; } // ok; key not found; not called yet;
    
    public abstract int GetStatus { get; } // ok; key not found; not called yet;
}

public class DefaultDictionary<T> : NativeDictionary<T> {
    private readonly Dictionary<string, T> _dict = new();
    private int _removeStatus = REMOVE_NOT_CALLED_YET;
    private int _getStatus = GET_NOT_CALLED_YET;

    public override void Put(string key, T value) {
        _dict[key] = value;
    }

    public override void Remove(string key)
    {
        if (!Contains(key))
        {
            _removeStatus = REMOVE_NOT_FOUND;
            return;
        }

        _dict.Remove(key);
        _removeStatus = REMOVE_OK;
    }

    public override void Clear() {
        _dict.Clear();
    }

    public override T Get(string key)
    {
        if (!Contains(key))
        {
            _getStatus = GET_NOT_FOUND;
            return default;
        }

        _getStatus = GET_OK;
        return _dict[key];
    }

    public override int Size => _dict.Count;

    public override bool Contains(string key) => _dict.ContainsKey(key);

    public override int RemoveStatus => _removeStatus;
    
    public override int GetStatus => _getStatus;
}