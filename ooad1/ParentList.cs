public class Node<T>(T data, Node<T>? previous, Node<T>? next)
{
    public T Data = data;
    public Node<T>? Previous = previous;
    public Node<T>? Next = next;
}

public abstract class ParentList<T>
{
    public const int HEAD_OK = 0;
    public const int HEAD_EMPTY = 1;
    public const int HEAD_NULL = 2;

    public const int TAIL_OK = 0;
    public const int TAIL_EMPTY = 1;
    public const int TAIL_NULL = 2;

    public const int RIGHT_OK = 0;
    public const int RIGHT_EMPTY = 1;
    public const int RIGHT_LAST = 2;
    public const int RIGHT_NULL = 3;

    public const int GET_OK = 0;
    public const int GET_EMPTY = 1;
    public const int GET_NULL = 2;

    public const int PUT_RIGHT_OK = 0;
    public const int PUT_RIGHT_EMPTY = 1;
    public const int PUT_RIGHT_NULL = 2;

    public const int PUT_LEFT_OK = 0;
    public const int PUT_LEFT_EMPTY = 1;
    public const int PUT_LEFT_NULL = 2;

    public const int REMOVE_OK = 0;
    public const int REMOVE_EMPTY = 1;
    public const int REMOVE_NULL = 2;

    public const int REPLACE_OK = 0;
    public const int REPLACE_EMPTY = 1;
    public const int REPLACE_NULL = 2;

    public const int FIND_OK = 0;
    public const int FIND_NOT_FOUND = 1;
    public const int FIND_NULL = 2;

    public const int ADD_TO_EMPTY_OK = 0;
    public const int ADD_TO_EMPTY_NOT_EMPTY_LIST = 1;
    public const int ADD_TO_EMPTY_NULL = 2;

    protected int _headStatus = HEAD_NULL;
    protected int _tailStatus = TAIL_NULL;
    protected int _rightStatus = RIGHT_NULL;
    protected int _getStatus = GET_NULL;
    protected int _putRightStatus = PUT_RIGHT_NULL;
    protected int _putLeftStatus = PUT_LEFT_NULL;
    protected int _addToEmptyStatus = ADD_TO_EMPTY_NULL;
    protected int _removeStatus = REMOVE_NULL;
    protected int _replaceStatus = REPLACE_NULL;
    protected int _findStatus = FIND_NULL;

    protected Node<T>? _head, _tail;

    protected Node<T> _cursor = null!;

    protected int _size;

    // Commands

    // precondition : list is not empty
    // post condition : cursor points to the first element of the list
    public void Head()
    {
        if (_head is null)
        {
            _headStatus = HEAD_EMPTY;
            return;
        }

        _cursor = _head;
        _headStatus = HEAD_OK;
    }

    // precondition : list is not empty
    // post condition : cursor points to the last element of the list
    public void Tail()
    {
        if (_tail is null)
        {
            _tailStatus = TAIL_EMPTY;
            return;
        }

        _cursor = _tail!;
        _tailStatus = TAIL_OK;
    }

    // precondition : cursor points to an element which has a successor
    // post condition : cursor points to the next element
    public void Right()
    {
        if (_head is null)
        {
            _rightStatus = RIGHT_EMPTY;
            return;
        }

        if (_cursor.Next is null)
        {
            _rightStatus = RIGHT_LAST;
            return;
        }

        _cursor = _cursor.Next;
        _rightStatus = RIGHT_OK;
    }

    // precondition : list is not empty
    // post condition : new node with the given value is inserted after the element which the cursor points to
    public void PutRight(T value)
    {
        if (_head is null)
        {
            _putRightStatus = PUT_RIGHT_EMPTY;
            return;
        }

        Node<T>? tempNext = _cursor.Next;
        Node<T> newNode = new(value, _cursor, tempNext);
        _cursor.Next = newNode;
        if (tempNext is not null)
        {
            tempNext.Previous = newNode;
        }
        else
        {
            _tail = newNode;
        }

        _size++;
        _putRightStatus = PUT_RIGHT_OK;
    }

    // precondition : list is not empty
    // post condition : new node with the given value is inserted before the element which the cursor points to
    public void PutLeft(T value)
    {
        if (_head is null)
        {
            _putLeftStatus = PUT_LEFT_EMPTY;
            return;
        }

        Node<T>? tempPrev = _cursor.Previous;
        Node<T> newNode = new(value, tempPrev, _cursor);
        _cursor.Previous = newNode;
        if (tempPrev is not null)
        {
            tempPrev.Next = newNode;
        }
        else
        {
            _head = newNode;
        }

        _size++;
        _putLeftStatus = PUT_LEFT_OK;
    }

    // precondition : list is not empty
    // post condition : node which the cursor points to is deleted and cursor is moved either to the left or right neighbour. Otherwise we clear the list
    public void Remove()
    {
        if (_head is null)
        {
            _removeStatus = REMOVE_EMPTY;
            return;
        }

        if (_head == _tail)
        {
            Clear();
            _removeStatus = REMOVE_OK;
            return;
        }

        Node<T>? prev = _cursor.Previous;
        Node<T>? next = _cursor.Next;

        if (prev is null)
        {
            _head = next;
            next!.Previous = null;
            _cursor = next;
        }
        else if (next is null)
        {
            _tail = prev;
            _tail!.Next = null;
            _cursor = prev;
        }
        else
        {
            prev.Next = next;
            next.Previous = prev;
            _cursor = next;
        }

        _size--;
        _removeStatus = REMOVE_OK;
    }

    // post condition : list is empty
    public virtual void Clear()
    {
        _head = null;
        _tail = null;
        _cursor = null!;
        _headStatus = HEAD_NULL;
        _tailStatus = TAIL_NULL;
        _rightStatus = RIGHT_NULL;
        _getStatus = GET_NULL;
        _putRightStatus = PUT_RIGHT_NULL;
        _putLeftStatus = PUT_LEFT_NULL;
        _removeStatus = REMOVE_NULL;
        _replaceStatus = REPLACE_NULL;
        _findStatus = FIND_NULL;
    }

    // precondition : list is empty
    // post condition : new value is added to an empty list and cursor points to this value
    public void AddToEmpty(T value)
    {
        if (_head is not null)
        {
            _addToEmptyStatus = ADD_TO_EMPTY_NOT_EMPTY_LIST;
            return;
        }

        Node<T> newNode = new(value, null, null);
        _head = newNode;
        _tail = newNode;
        _cursor = newNode;
        _size++;
        _addToEmptyStatus = ADD_TO_EMPTY_OK;
    }

    // post condition : new node with given value is inserted at the end of the list; cursor is not changed
    public void AddTail(T value)
    {
        _size++;
        if (_tail is null)
        {
            AddToEmpty(value);
            return;
        }

        Node<T> newNode = new(value, _tail, null);
        _tail.Next = newNode;
        _tail = newNode;
    }

    // precondition : list is not empty
    // post condition : value of node which the cursor points to is replaced with given value. In more abstract way can be replace with remove() + AddEmpty() or remove() + PutLeft() or PutRight()
    public void Replace(T value)
    {
        if (_head is null)
        {
            _replaceStatus = REPLACE_EMPTY;
            return;
        }

        _cursor.Data = value;
        _replaceStatus = REPLACE_OK;
    }

    // post condition : list points either to next element with given value if such was found
    public void Find(T value)
    {
        Node<T> temp = _cursor;
        while (temp.Next is not null)
        {
            if (temp.Data!.Equals(value))
            {
                _cursor = temp;
                _findStatus = FIND_OK;
                return;
            }

            temp = temp.Next;
        }

        _findStatus = FIND_NOT_FOUND;
    }


    // Queries

    // precondition : list is not empty
    public T? Get()
    {
        if (_head is null)
        {
            _getStatus = GET_EMPTY;
            return default;
        }

        _getStatus = GET_OK;
        return _cursor.Data;
    }

    public int Size() => _size;

    public bool IsHead() => _cursor == _head;

    public bool IsTail() => _cursor == _tail;

    public bool IsValue() => _head is not null;


    // Helper queries

    public int GetHeadStatus() => _headStatus; // returns the value of HEAD_*

    public int GetTailStatus() => _tailStatus; // returns the value of TAIL_*

    public int GetRightStatus() => _rightStatus; // returns the value of RIGHT_*

    public int GetGetStatus() => _getStatus; // returns the value of GET_*

    public int GetPutRightStatus() => _putRightStatus; // returns the value of PUT_RIGHT_*

    public int GetPutLeftStatus() => _putLeftStatus; // returns the value of PUT_LEFT_*

    public int GetRemoveStatus() => _removeStatus; // returns the value of REMOVE_*

    public int GetReplaceStatus() => _replaceStatus; // returns the value of REPLACE_*

    public int GetFindStatus() => _findStatus; // returns the value of FIND_*

    public int GetAddEmptyStatus() => _addToEmptyStatus; // returns the value of ADD_TO_EMPTY_*
}

public class LinkedList<T> : ParentList<T>;

public class DoublyLinkedList<T> : ParentList<T>
{
    public const int LEFT_OK = 0;
    public const int LEFT_EMPTY = 1;
    public const int LEFT_FIRST = 2;
    public const int LEFT_NULL = 3;

    private int _leftStatus = LEFT_NULL;

    // precondition : list is not empty and cursor has a predecessor
    // post condition : cursor is moved to the predecessor
    public void Left()
    {
        if (_head is null)
        {
            _leftStatus = LEFT_EMPTY;
            return;
        }

        if (_cursor.Previous is null)
        {
            _leftStatus = LEFT_FIRST;
            return;
        }

        _cursor = _cursor.Previous;
        _leftStatus = LEFT_OK;
    }

    public override void Clear()
    {
        base.Clear();
        _leftStatus = LEFT_NULL;
    }

    public int GetLeftStatus() => _leftStatus;
}