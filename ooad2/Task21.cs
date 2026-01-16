// Implementation inheritance

// As already mentioned this type of inheritance comes in after a reification inheritance took place and we want to reuse existing good implementation. In the case below we have an abstract entity
// List<T> which is implemented by an AbstractList<T>. When we start implementing our Stack<T> we see that for implementing its methods(because stack is a new abstraction) we might use
// existing implementation of AbstractList<T> so we just inherit it!

public interface List<T>
{
    void Add(T element);

    T Get(int index);

    int Size();
}

public class AbstractList<T> : List<T>
{
    protected List<T> elements = new ArrayList<>();

    public void Add(T element)
    {
        elements.add(element);
    }

    public T Get(int index)
    {
        return elements.get(index);
    }

    public int Size()
    {
        return elements.size();
    }
}

public class Stack<T> : AbstractList<T>
{
    public void Push(T element)
    {
        add(element);
    }

    public T Pop()
    {
        if (size() == 0)
            throw new EmptyStackException();

        return elements.remove(size() - 1);
    }

    public T peek() {
        if (size() == 0)
            throw new EmptyStackException();

        return elements.get(size() - 1);
    }
}


// Facility inheritance

// So the main advantage here is that we can get access to the facility methods of constants of some class without a need to instantiate it and embed as a field into the class itself.
// In this case the subclass is not some specialized type of the base type but uses it just to get access to needed stuff. So we could have a class which combines IteratorUtilities which
// we could use to iterate over objects. And then to get access to these utilities we could inherit this class in our custom import processing class which say on the lowest level works
// with strings and so that we do not instantiate the class(although I would probably do this) we could get access to it through inheritance.

public class IteratorUtilities<T>(IEnumerable<T> collection)
{
    public void ForEach(Action<T> action)
    {
        foreach(T item in collection)
        {
            action(item);
        }
    }

    public void ForEachWhile(Func<T, bool> condition, Action<T> action)
    {
        ...
    }
}

public class XlsxImportProcessingFacade : ImportProcessingFacade, IteratorUtilities<string>
{

}