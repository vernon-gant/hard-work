namespace Algos3;

public static class Task2
{
    public static void InsertionSortStep(int[] array, int step, int i)
    {
        if (i >= array.Length - 1)
            return;

        for (int j = i + step; j < array.Length; j += step)
        {
            for (int n = j - step; n >= i && array[n] > array[n + step]; n -= step)
            {
                Swap(n, n + step, array);
            }
        }
    }

    public static void Swap(int x, int y, int[] array)
    {
        var temp = array[x];
        array[x] = array[y];
        array[y] = temp;
    }
}
