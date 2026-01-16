using System;
using System.Collections.Generic;

namespace AlgorithmsDataStructures
{
    public class Node<T>
    {

        public T value;

        public Node<T> next, prev;

        public Node(T _value)
        {
            value = _value;
            prev = null;
            next = null;
        }

    }

    public class OrderedList<T>
    {

        public Node<T> head, tail;

        private bool _ascending;

        public int size;

        public OrderedList(bool asc)
        {
            head = null;
            tail = null;
            size = 0;
            _ascending = asc;
        }

        public int Compare(T v1, T v2)
        {
            int result;
            if (typeof(T) == typeof(String))
            {
                // trim strings and compare them
                var str1 = v1 as string;
                var str2 = v2 as string;
                str1 = str1?.Trim();
                str2 = str2?.Trim();
                result = String.Compare(str1, str2);
                if (result < 0) result = -1;
                else if (result > 0) result = 1;
                else result = 0;
            }
            else
            {
                // use object for type casting and then to int
                var int1 = (int)(object)v1;
                var int2 = (int)(object)v2;
                if (int1 < int2) result = -1;
                else if (int1 > int2) result = 1;
                else result = 0;
            }

            return _ascending ? result : -result;
            // -1 если v1 < v2
            // 0 если v1 == v2
            // +1 если v1 > v2
        }

        public void Add(T value)
        {
            if (value == null) return;

            var newNode = new Node<T>(value);
            size++;
            // If the list is empty
            if (head == null)
            {
                head = newNode;
                tail = newNode;
                return;
            }
            // If value is less then head then or equal(head is smallest element)
            if (Compare(value, head.value) == -1 ||
                Compare(value, head.value) == 0)
            {
                newNode.next = head;
                head.prev = newNode;
                head = newNode;
                return;
            }
            // If value is less then head then or equal(head is smallest element)
            if (Compare(value, tail.value) == 1 ||
                Compare(value, tail.value) == 0)
            {
                tail.next = newNode;
                newNode.prev = tail;
                tail = newNode;
                return;
            }
            var beforeInsert = head;
            for (;
                 Compare(beforeInsert.next.value, value) == -1;
                 beforeInsert = beforeInsert.next) { }
            newNode.next = beforeInsert.next;
            beforeInsert.next.prev = newNode;
            beforeInsert.next = newNode;
            newNode.prev = beforeInsert;
        }

        public Node<T> Find(T val)
        {
            // Check if element is out of max min range, list is empty
            // or passed element is null
            if (size == 0 ||
                val == null ||
                Compare(val, head.value) == -1 ||
                Compare(val, tail.value) == 1) return null;

            var temp = head;
            // Iterate until a bigger number is found
            for (; Compare(temp.value, val) == -1; temp = temp.next) { }
            // If it does not equal to the val => not found
            if (Compare(temp.value, val) != 0) return null;

            return temp;
        }

        public void Delete(T val)
        {
            var toDelete = Find(val);

            if (toDelete == null) return;

            if (toDelete == tail && toDelete == head)
            {
                Clear(_ascending);
                return;
            }
            if (toDelete == head)
            {
                head = head.next;
                head.prev = null;
            }
            else if (toDelete == tail)
            {
                tail = tail.prev;
                tail.next = null;
            }
            else
            {
                var beforeDelete = toDelete.prev;
                var afterDelete = toDelete.next;
                beforeDelete.next = afterDelete;
                afterDelete.prev = beforeDelete;
            }
            size--;
        }

        public void Clear(bool asc)
        {
            _ascending = asc;
            head = null;
            tail = null;
            size = 0;
        }

        public int Count()
        {
            return size;
        }

        List<Node<T>> GetAll() // выдать все элементы упорядоченного 
        {
            List<Node<T>> r = new List<Node<T>>();
            Node<T> node = head;
            while (node != null)
            {
                r.Add(node);
                node = node.next;
            }
            return r;
        }

    }
}