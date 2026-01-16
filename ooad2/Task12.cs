public class None : ...
{
    private static readonly None instance = new None();

    private None() { }

    public static None Instance => instance;
}

public class MyAny : General
{
    public T AssignmentAttempt<T>() where T : General
    {
        return this is T ? (T)this : None.Instance as T;
    }
}