using System;
using AlgorithmsDataStructures;

namespace DoublyLinkedListCode
{
    static class Program
    {

        static void Main()
        {
            var list = new AdvancedLinkedList();
            list.AddInTail(new Node(1));
            list.AddInTail(new Node(2));
            list.AddInTail(new Node(3));
            list.AddInTail(new Node(4));
            list.AddInTail(new Node(5));
            Console.WriteLine(list.RemoveNode(6));
            list.RemoveNode(5);
            list.RemoveNode(4);
            list.RemoveNode(3);
            list.RemoveNode(2);
            list.RemoveNode(1);
            Console.WriteLine(list.RemoveNode(1));
        }

    }
}