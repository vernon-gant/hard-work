using AlgorithmsDataStructures;

namespace LinkedListTasks
{
    public class SpecialTask8
    {

        public LinkedList MergeAndMapTwoLists(LinkedList list1, LinkedList list2)
        {
            if (list1.Count() != list2.Count()) return new LinkedList();

            var result = new LinkedList();

            Node temp1 = list1.head;
            Node temp2 = list2.head;

            while (temp1 != null)
            {
                var sum = temp1.value + temp2.value;
                result.AddInTail(new Node(sum));
                temp1 = temp1.next;
                temp2 = temp2.next;
            }

            return result;
        }

    }
}