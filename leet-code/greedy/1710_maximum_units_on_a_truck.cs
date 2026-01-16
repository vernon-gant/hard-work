public class Solution
{
    public int MaximumUnits(int[][] boxTypes, int truckSize)
    {
        Array.Sort(boxTypes, (a, b) => b[1].CompareTo(a[1]));

        int result = 0;

        for(int i = 0; i < boxTypes.Length; i++)
        {
            if (truckSize < boxTypes[i][0])
            {
                result += truckSize * boxTypes[i][1];
                break;
            }

            result += boxTypes[i][0] * boxTypes[i][1];
            truckSize -= boxTypes[i][0];
        }

        return result;
    }
}