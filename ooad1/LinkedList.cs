public abstract class LinkedList<T>
{
    public const int HEAD_OK = 0; // head() completed successfully
    public const int HEAD_EMPTY = 1; // list is empty
    public const int HEAD_NULL = 2; // head() has not been called yet
    
    public const int TAIL_OK = 0; // tail() completed successfully
    public const int TAIL_EMPTY = 1; // list is empty
    public const int TAIL_NULL = 2; // tail() has not been called yet
    
    public const int RIGHT_OK = 0; // right() completed successfully
    public const int RIGHT_EMPTY = 1; // list is empty
    public const int RIGHT_ERR = 2; // current cursor points to the last node of the list
    public const int RIGHT_NULL = 3; // right() has not been called yet
    
    public const int GET_OK = 0; // get() returned successfully
    public const int GET_EMPTY = 1; // list is empty
    public const int GET_NULL = 2; // get() has not been called yet
    
    public const int PUT_RIGHT_OK = 0; // put_right() returned successfully
    public const int PUT_RIGHT_EMPTY = 1; // list is empty
    public const int PUT_RIGHT_NULL = 2; // put_right() has not been called yet
    
    public const int PUT_LEFT_OK = 0; // put_left() returned successfully
    public const int PUT_LEFT_EMPTY = 1; // list is empty
    public const int PUT_LEFT_NULL = 2; // put_left() has not been called yet
    
    public const int REMOVE_OK = 0; // remove() completed successfully
    public const int REMOVE_EMPTY = 1; // list is empty
    public const int REMOVE_NULL = 2; // remove() has not been called yet
    
    public const int ADD_TO_EMPTY_OK = 0; // add_to_empty() completed successfully
    public const int ADD_TO_EMPTY_ERR = 1; // add_to_empty() failed
    public const int ADD_TO_EMPTY_NULL = 2; // AddToEmpty() has not been called yet
    
    public const int ADD_TAIL_OK = 0; // AddTail() returned successfully
    public const int ADD_TAIL_EMPTY = 1; // list is empty
    public const int ADD_TAIL_NULL = 2; // AddTail() has not been called yet
    
    public const int REPLACE_OK = 0; // Replace() returned successfully
    public const int REPLACE_EMPTY = 1; // list is empty
    public const int REPLACE_NULL = 2; // Replace() has not been called yet
    
    public const int FIND_OK = 0; // Find() completed successfully
    public const int FIND_EMPTY = 1; // list is empty
    public const int FIND_ERR = 2; // No node with given value found
    public const int FIND_NULL = 3; // Find() has not been called yet
    
    public const int REMOVE_ALL_OK = 0; // RemoveAll() completed successfully
    public const int REMOVE_EMPTY = 1; // list is empty
    public const int REMOVE_ALL_NULL = 2; // RemoveAll() has not been called yet
    
    
    protected int headStatus = HEAD_NULL;
    protected int tailStatus = TAIL_NULL;
    protected int rigtStatus = RIGHT_NULL;
    protected int getStatus = GET_NULL;
    protected int putRightStatus = PUT_RIGHT_NULL;
    protected int putLeftStatus = PUT_LEFT_NULL;
    protected int removeStatus = REMOVE_NULL;
    protected int addToEmptyStatus = ADD_TO_EMPTY_NULL;
    protected int addTailStatus = ADD_TAIL_NULL;
    protected int replaceStatus = REPLACE_NULL;
    protected int findStatus = FIND_NULL;
    protected int removeAllStatus = REMOVE_ALL_NULL;
    
    // Commands
    
    // precondition : list is not empty
    // post condition : cursor points to the first element of the list
    public abstract void Head();
    
    // precondition : list is not empty
    // post condition : cursor points to the last element of the list
    public abstract void Tail();
    
    // precondition : curson points to an elemement which has a successor
    // post condition : cursor points to the next element
    public abstract void Right();
    
    // precondition : list is not empty
    // post condition : new node with the given value is inserted after the element which the cursor points to
    public abstract void PutRight(T value);
    
    // precondition : list is not empty
    // post condition : new node with the given value is inserted before the element which the cursor points to
    public abstract void PutLeft(T value);
    
    // precondition : list is not empty
    // post condition : node which the cursor points to is deleted and cursor is moved either to the left or right neighbour. Otherwise we clear the list
    public abstract void Remove();
    
    // post condition : list is empty
    public abstract void Clear();
    
    // precondition : list is empty
    // post condition : new value is added to the list and cursor points to this value
    public abstract void AddToEmpty(T value);
    
    // precondition : list is not empty
    // post condition : new node with given value is inserted at the end of the list; cursor is not changed
    public abstract void AddTail(T value);
    
    // precondition : list is not empty
    // post condition : value of node which the cursor points to is replaced with given value. In more abstract way can be replace with remove() + AddEmpty() or remove() + PutLeft() or PutRight()
    public abstract void Replace(T value);
    
    // precondition : list is not empty
    // post condition : list points either to next element relative to cursor or to the end of the list
    public abstract void Find(T value);
    
    // precondition : list not empty
    // post condition : values with given values are deleted what results in either partial removal or full and list becomes empty. In the first case cursor is not changed
    public abstract void RemoveAll(T value);
    
    
    // Queries
    
    // precondition : list is not empty
    public abstract void Get();
    
    public abstract void Size();
    
    // precondition : list is not empty
    public abstract void IsHead();
    
    // precondition : list is not empty
    public abstract void IsTail();
    
    public abstract void IsValue();
    
    
    // Helper queries
    
    public int GetHeadStatus() => headStatus; // returns the value of HEAD_*
    
    public int GetTailStatus() => tailStatus; // returns the value of TAIL_*
    
    public int GetRightStatus() => rightStatus; // returns the value of RIGHT_*
    
    public int GetGetStatus() => getStatus; // returns the value of GET_*
    
    public int GetPutRightStatus() => putRightStatus; // returns the value of PUT_RIGHT_*
    
    public int GetPutLeftStatus() => putLeftStatus; // returns the value of PUT_LEFT_*
    
    public int GetRemoveStatus() => removeStatus; // returns the value of REMOVE_*
     
    public int GetAddToEmptyStatus() => addToEmptyStatus; // returns the value of ADD_TO_EMPTY_*
    
    public int GetAddTailStatus() => addTailStatus; // returns the value of ADD_TAIL_*
    
    public int GetReplaceStatus() => replaceStatus; // returns the value of REPLACE_*
    
    public int GetFindStatus() => findStatus; // returns the value of FIND_*
    
    public int GetRemoveAllStatus() => removeAllStatus; // returns the value of REMOVE_ALL_*
    
    
}

/*
 * 2.2 - because we can not efficiently come to tail rather than not storing another pointer to it internally. All other variants O(n)
 * 2.3 - now we have find() and using it we can build a list if we want just by moving cursor to the next element with given value
 */