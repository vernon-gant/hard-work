public class Solution
{
    public int[] Intersect(int[] nums1, int[] nums2)
    {
        Array.Sort(nums1);
        Array.Sort(nums2);

        var upperBound = Math.Min(nums1.Length, nums2.Length);
        int[] result = new int[upperBound];
        int firstPointer = 0, secondPointer = 0, resultPointer = 0;

        while(firstPointer < nums1.Length && secondPointer < nums2.Length)
        {
            if (nums1[firstPointer] < nums2[secondPointer])
            {
                firstPointer++;
            } else if (nums2[secondPointer] < nums1[firstPointer])
            {
                secondPointer++;
            } else
            {
                int tempFirst = firstPointer, tempSecond = secondPointer;

                for(; firstPointer < nums1.Length && nums1[tempFirst] == nums1[firstPointer]; firstPointer++) {}

                for(; secondPointer < nums2.Length && nums2[tempSecond] == nums2[secondPointer]; secondPointer++) {}

                var toAdd = Math.Min(firstPointer - tempFirst, secondPointer - tempSecond);

                for(int i = 0; i < toAdd; i++)
                    result[resultPointer++] = nums1[tempFirst];
            }
        }

        return result[..resultPointer];
    }
}

/*
public class Solution
{
    public int[] Intersect(int[] nums1, int[] nums2)
    {
        var occurences = new int[1001];

        foreach(var num in nums1) occurences[num]++;

        var result = new List<int>();

        foreach(var num in nums2)
        {
            if (occurences[num] != 0)
            {
                result.Add(num);
                occurences[num]--;
            }
        }

        return result.ToArray();
    }
}
*/