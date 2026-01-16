public class Solution
{
    public static int SingleNumber(int[] arr)
    {
        int result = 0;

        foreach(int num in arr)
        {
            result ^= num;
        }

        return result;
    }
}
