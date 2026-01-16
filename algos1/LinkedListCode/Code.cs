using System.Collections.Generic;

namespace AlgorithmsDataStructures
{
    public class Node
    {

        public int value;

        public Node next;

        public Node(int _value)
        {
            value = _value;
        }

    }

    public class LinkedList
    {

        public Node head;

        public Node tail;


        public LinkedList()
        {
            head = null;
            tail = null;
        }

        public LinkedList(Node head) : this()
        {
            this.head = head;
        }

        public void AddInTail(Node _item)
        {
            if (head == null) head = _item;
            else tail.next = _item;
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

            if (head.value == _value)
            {
                head = head.next;

                if (head == null)
                {
                    tail = null;
                }

                return true;
            }

            Node toDel, previous = head;

            for (toDel = head; toDel != null && toDel.value != _value; toDel = toDel.next)
            {
                previous = toDel;
            }

            if (toDel == null) return false;

            previous.next = toDel.next;

            if (toDel.next == null) tail = previous;

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
            if (_nodeAfter == null)
            {
                head = _nodeToInsert;
                tail = _nodeToInsert;
                return;
            }

            var next = _nodeAfter.next;
            _nodeAfter.next = _nodeToInsert;
            _nodeToInsert.next = next;
            if (next == null) tail = _nodeToInsert;
        }

    }
}