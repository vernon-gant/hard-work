using AlgorithmsDataStructures;

namespace DequeCode
{
    public class SpecialTask
    {
        /*
         * 2. Для выравнивание сложности(особенно removeLast, где в обычном связном списке сложность была бы O(n)) я
         * использовал Doubly Linked List, который даёт нам О(1) на все операции. За это, конечно, приходится
         * платит 4 лишними байтами на ссылку prev, но зато удаление с конца константное :)
         */

        
        // 3.
        public static bool IsPalindrome(string word)
        {
            var deque = new Deque<char>();
            foreach (var letter in word) deque.AddFront(letter);
            while (deque.Size() > 1)
            {
                if (deque.RemoveTail() != deque.RemoveFront()) return false;
            }
            return true;
        }

    }
}