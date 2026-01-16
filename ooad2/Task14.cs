public interface ISummable<T>
{
    T Add(T other);
}

public class Vector<T> : MyAny where T : General, ISummable<T>
{
    public Vector(List<T> elements)
    {
        Elements = elements;
    }

    public List<T> Elements { get; }

    public Vector<T> Add(Vector<T> vector)
    {
        if (Elements.Count != vector.Elements.Count)
            return null;

        var resultElements = Elements.Zip(vector.Elements, (a, b) => a.Add(b)).ToList();
        return new Vector<T>(resultElements);
    }
}

public class NumberValue(int value) : MyAny, ISummable<NumberValue>
{
    public int Value { get; } = value;

    public NumberValue Add(NumberValue other)
    {
        return new NumberValue(Value + other.Value);
    }
}

public class SummableVector<T>(List<T> elements) : Vector<T>(elements), ISummable<SummableVector<T>> where T : General, ISummable<T>
{
    public SummableVector<T> Add(SummableVector<T> other)
    {
        var addedArray = base.Add(other);

        return addedArray == null ? null : new SummableVector<T>(addedArray.Elements);
    }
}

...

var innerMostArray1 = new SummableVector<NumberValue>([
    new(1),
    new(2)
]); // [1,2]

var innerMostArray2 = new SummableVector<NumberValue>([
    new(3),
    new(4)
]); // [3,4]


var middleArray1 = new SummableVector<SummableVector<NumberValue>>([innerMostArray1]); // [[1,2]]
var middleArray2 = new SummableVector<SummableVector<NumberValue>>([innerMostArray2]); // [[3,4]]


var outerMostArray1 = new SummableVector<SummableVector<SummableVector<NumberValue>>>([middleArray1]); // [[[1,2]]]
var outerMostArray2 = new SummableVector<SummableVector<SummableVector<NumberValue>>>([middleArray2]); // [[[3,4]]]


var nestedSum = outerMostArray1.Add(outerMostArray2); // [[[4,6]]]