public class Solution
{
    public IList<IList<int>> FindDifference(int[] nums1, int[] nums2)
    {
        var occurences = new Dictionary<int, int>();

        foreach(var num in nums1)
            occurences[num] = 1;

        var secondUnique = new HashSet<int>();

        foreach(var num in nums2)
        {
            occurences.TryGetValue(num, out var counter);

            if (counter == 0)
                secondUnique.Add(num);
            else
                occurences[num]++;
        }

        var firstUnique = new List<int>();

        foreach(var pair in occurences)
            if (pair.Value == 1) firstUnique.Add(pair.Key);

        return new List<IList<int>>() { firstUnique, secondUnique.ToList() };
    }
}

/* Too nice :)
 * 
 * public class Solution
{
    public IList<IList<int>> FindDifference(int[] nums1, int[] nums2)
    {
        Span<bool> set1 = stackalloc bool[2001];
        Span<bool> set2 = stackalloc bool[2001];

        foreach(var num in nums1)
            set1[num + 1000] = true;

        foreach(var num in nums2)
            set2[num + 1000] = true;

        List<int> firstUnique = new(), secondUnique = new();

        for(int i = 0; i < 2001; i++)
        {
            if (set1[i] && !set2[i])
                firstUnique.Add(i - 1000);
            else if (set2[i] && !set1[i])
                secondUnique.Add(i - 1000);
        }

        return new List<IList<int>>() { firstUnique, secondUnique };
    }
} */