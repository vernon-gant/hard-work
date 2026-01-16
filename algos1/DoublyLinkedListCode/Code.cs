using System.Collections.Generic;

namespace AlgorithmsDataStructures
{
    public class Node
    {

        public int value;

        public Node next, prev;

        public Node(int _value)
        {
            value = _value;
            next = null;
            prev = null;
        }

    }

    public class LinkedList2
    {

        public Node head;

        public Node tail;

        public LinkedList2()
        {
            head = null;
            tail = null;
        }

        public void AddInTail(Node _item)
        {
            if (head == null)
            {
                head = _item;
                head.next = null;
                head.prev = null;
            }
            else
            {
                tail.next = _item;
                _item.prev = tail;
            }

            tail = _item;
        }

        public Node Find(int _value)
        {
            Node node = head;

            while (node != null)
            {
                if (node.value == _value) return node;

                node = node.next;
            }

            return null;
        }

        public List<Node> FindAll(int _value)
        {
            List<Node> nodes = new List<Node>();

            Node temp = head;

            while (temp != null)
            {
                if (temp.value == _value) nodes.Add(temp);

                temp = temp.next;
            }

            return nodes;
        }

        public bool Remove(int _value)
        {
            if (head == null) return false;

            // Check if head must be removed
            if (head.value == _value)
            {
                head = head.next;

                // Check if there was only one Node in the list and set tail to null
                if (head == null)
                {
                    tail = null;
                }
                else
                {
                    head.prev = null;
                }

                return true;
            }

            // Otherwise iterate till node containing this value
            Node toDel;

            for (toDel = head; toDel != null && toDel.value != _value; toDel = toDel.next) { }

            // False if null -> not found
            if (toDel == null) return false;

            // Link previous node of found to next of it
            toDel.prev.next = toDel.next;

            // If we must delete last node, then just assign
            // previous of to delete to tail
            if (toDel.next == null)
            {
                tail = toDel.prev;
            }
            else
            {
                // If it is an ordinary node, connect next after toDelete
                // in reverse direction
                toDel.next.prev = toDel.prev;
            }

            return true;
        }

        public void RemoveAll(int _value)
        {
            bool goOn;

            do
            {
                goOn = Remove(_value);
            } while (goOn);
        }

        public void Clear()
        {
            head = null;
            tail = null;
        }

        public int Count()
        {
            int result = 0;

            for (Node temp = head; temp != null; temp = temp.next)
            {
                result++;
            }

            return result;
        }

        public void InsertAfter(Node _nodeAfter, Node _nodeToInsert)
        {
            // If we insert at 1st Position, AddFront
            if (_nodeAfter == null)
            {
                // Link nodeToInsert to head and make prev null
                // and change head
                _nodeToInsert.next = head;
                _nodeToInsert.prev = null;
                head = _nodeToInsert;

                // If the list is empty(tail will be null)
                tail ??= _nodeToInsert;

                return;
            }
            
            // Link nodeAfter and nodeToInsert together but without linking
            // nodeAfter next with nodeToInsert in reverse direction
            _nodeToInsert.next = _nodeAfter.next;
            _nodeAfter.next = _nodeToInsert;
            _nodeToInsert.prev = _nodeAfter;

            // Last "but" above to avoid null pointer exception
            // as if we insert after tail we would access null.prev
            // in "else"
            if (_nodeToInsert.next == null)
            {
                tail = _nodeToInsert;
            }
            else
            {
                _nodeToInsert.next.prev = _nodeToInsert;
            }
        }

    }
}