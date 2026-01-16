namespace AlgorithmsDataStructures
{
    /*
     * 2. Добавлений и удалений в очередь в большинстве случаев О(1). Связный список отличный тому пример: имея ссылки
     * на head и tail, мы можем безболезненно менять их ссылки(в случае с head) или их ссылки на next(в случае с tail).
     * В случае реализации с помощью массива всё зависит от контекста: если мы знаем, что количество элементов
     * в очереди не привысит N, то можно сделать буфер такого размера, и тогда добавление и удаление будет О(1).
     * В случае использования подхода с динамическим массивом, брать во внимание релокацию при сужении/увеличении
     * буфера, что даёт нам О(n). Поэтому в универе все делают очередь через LL :)
     */

    // 3. 
    public class Roller
    {

        public static void RollQueue(Queue<int> queue, int n)
        {
            // For unnecessary loops
            n %= queue.Size();

            for (int i = 0; i < n; i++)
            {
                int temp = queue.Dequeue();
                queue.Enqueue(temp);
            }
        }

    }


    // 4. O(n) for dequeue, not a great deal...
    public class StackQueue<T>
    {

        private readonly Stack<T> _enqueue = new();

        private readonly Stack<T> _dequeue = new();

        public StackQueue()
        {
            
        }

        public void Enqueue(T item)
        {
            _enqueue.Push(item);
        }

        public T Dequeue()
        {
            if (!_dequeue.IsEmpty()) return _dequeue.Pop();
            if (_enqueue.IsEmpty()) return default;

            while (!_enqueue.IsEmpty())
            {
                _dequeue.Push(_enqueue.Pop());
            }
            return _dequeue.Pop();
        }

        public int Size()
        {
            return _enqueue.Size() + _dequeue.Size();
        }

    }
}