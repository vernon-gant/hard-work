public abstract class BloomFilter<T> {
    // Status codes

    public const int ADD_OK = 0;
    public const int ADD_LIMIT_REACHED = 1;
    public const int ADD_NOT_CALLED_YET = 2;

    // Constructors

    // post condition : new bloom filter is created for the given input items limit and with given false positive probability
    public BloomFilter(int inputItemsLimit, decimal falsePositiveProbability) { }


    // Commands

    // precondition : amount of elements in the bloom filter is less than the limit
    // post condition : element is added to the bloom filter
    public abstract void Add(T element);


    // Queries

    public abstract bool Contains(T element);

    public abstract bool IsFull { get; }


    // Helper queries

    public abstract int AddStatus { get; } // ok; limit reached; not called yet
}

public class DefaultBloomFilter<T> : BloomFilter<T>
{
    // Hidden attributes
    
    private readonly BitArray _bitArray;
    private readonly int _size;
    private readonly int _hashCount;
    private readonly int _inputItemsLimit;
    private int _addCount;
    private int _addStatus;

    // Constructor
    public DefaultBloomFilter(int inputItemsLimit, decimal falsePositiveProbability) : base(inputItemsLimit, falsePositiveProbability)
    {
        _inputItemsLimit = inputItemsLimit;
        _size = GetSize(inputItemsLimit, falsePositiveProbability);
        _hashCount = GetHashCount(_size, inputItemsLimit);
        _bitArray = new BitArray(_size);
        _addCount = 0;
        _addStatus = ADD_NOT_CALLED_YET;
    }
    
    // Commands

    public override void Add(T element)
    {
        if (IsFull)
        {
            _addStatus = ADD_LIMIT_REACHED;
            return;
        }

        byte[] bytes = ObjectToByteArray(element);
        foreach (var position in GetHashPositions(bytes))
        {
            _bitArray[position] = true;
        }

        _addCount++;
        _addStatus = ADD_OK;
    }
    
    // Queries

    public override bool Contains(T element)
    {
        byte[] bytes = ObjectToByteArray(element);
        foreach (var position in GetHashPositions(bytes))
        {
            if (!_bitArray[position]) return false;
        }
        return true;
    }

    public override bool IsFull => _addCount >= _inputItemsLimit;
    
    // Helper queries
    
    public override int AddStatus => _addStatus;

    private int GetSize(int n, decimal p)
    {
        var size = -(n * (decimal)Math.Log((double)p)) / ((decimal)Math.Log(2) * (decimal)Math.Log(2));
        return (int)Math.Ceiling(size);
    }

    private int GetHashCount(int m, int n)
    {
        var hashCount = (m / (decimal)n) * (decimal)Math.Log(2);
        return (int)Math.Ceiling(hashCount);
    }

    private int[] GetHashPositions(byte[] bytes)
    {
        int[] positions = new int[_hashCount];
        for (var i = 0; i < _hashCount; i++)
        {
            int hash = MurmurHash(bytes);
            int position = (hash % _size + _size) % _size;
            positions[i] = position;
        }
        return positions;
    }
}