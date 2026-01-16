using System;
using AlgorithmsDataStructures;

namespace OrderedListCode
{
    class Program
    {

        static void Main(string[] args)
        {
            var orderedList = new OrderedList<int>(false);
            orderedList.Add(1);
            orderedList.Add(21);
            orderedList.Add(1213);
            orderedList.Add(23324);
            orderedList.Add(23324324);
            orderedList.Add(232221);
            orderedList.Add(-200);
            
            Console.WriteLine(orderedList.Count());
        }

    }
}