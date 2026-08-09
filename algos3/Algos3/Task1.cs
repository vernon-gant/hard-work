namespace Algos3;

public static class Task1
{
    public static void SelectionSortStep(int[] array, int i)
    {
        if (i >= array.Length - 1)
            return;

        var minIndex = i;

        for (int j = i + 1; j < array.Length; j++)
        {
            if (array[minIndex] > array[j])
                minIndex = j;
        }

        Swap(minIndex, i, array);
    }

    public static void Swap(int x, int y, int[] array)
    {
        var temp = array[x];
        array[x] = array[y];
        array[y] = temp;
    }

    public static bool BubbleSortStep(int[] array)
    {
        if (array.Length < 2)
            return true;

        bool noSwaps = true;

        for (int i = 1; i < array.Length; i++)
        {
            if (array[i - 1] > array[i])
            {
                Swap(i, i - 1, array);
                noSwaps = false;
            }
        }

        return noSwaps;
    }
}
