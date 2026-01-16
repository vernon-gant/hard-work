public class Solution
{
    public int[] Intersection(int[] nums1, int[] nums2)
    {
        var occurences = new int[1001];

        foreach(var num in nums1)
            occurences[num] = Math.Min(occurences[num] + 1, 1);

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