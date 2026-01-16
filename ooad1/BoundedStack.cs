public abstract class BoundedStack<T>
{
    //---Command result codes---

    public const int PUSH_NIL = 0; // push() has not been called yet
    public const int PUSH_OK = 1; // last push() completed successfully
    public const int PUSH_ERR = 2; // stack is full

    public const int POP_NIL = 0; // push() has not been called yet
    public const int POP_OK = 1; // last pop() completed successfully
    public const int POP_ERR = 2; // stack is empty

    public const int PEEK_NIL = 0; // push() has not been called yet
    public const int PEEK_OK = 1; // last peek() returned a valid result
    public const int PEEK_ERR = 2; // stack is empty

    public const int DEFAULT_MAX_CAPACITY = 32;

    //---Constructors---
    // must be implemented by every sub class

    // post condition : an empty stack with maximum capacity of "DEFAULT_MAX_CAPACITY" is created
    protected BoundedStack()
    {
        Clear();
    }

    // precondition : maxSize argument is a positive number
    // post condition : an empty stack with maximum capacity of "maxSize" is created
    protected BoundedStack(int maxCapacity) : this()
    {
        if (maxCapacity <= 0) throw new ArgumentException("maxSize must be a positive integer");
    }

    //---Commands---
    
    // precondition : current stack size is less than maximum capacity of the stack
    // post condition : new element is pushed on top of the stack
    public abstract void Push(T value);

    // precondition : stack is not empty
    // post condition : top element is deleted from stack
    public abstract void Pop();

    // post condition : all elements from the stack are deleted
    public abstract void Clear();

    //---Queries---

    // precondition : stack is not empty
    // for now return T? so that we can return something in case of an erro.
    // In future version will probably return some null object according to null object pattern
    public abstract T? Peek { get; }

    public abstract int Size { get; }

    //---Helper queries---

    public abstract int PushStatus { get; }

    public abstract int PopStatus { get; }

    public abstract int PeekStatus { get; }
}