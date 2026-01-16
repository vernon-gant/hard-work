public class Solution
{
    public IList<IList<int>> ThreeSum(int[] nums)
    {
        Array.Sort(nums);

        var result = new List<IList<int>>();

        for(int i = 0; i < nums.Length - 2; i++)
        {
            if (nums[i] > 0) break;

            if (i > 0 && nums[i] == nums[i - 1]) continue;

            int start = i + 1, end = nums.Length - 1;

            while(start < end)
            {
                var current = nums[i] + nums[start] + nums[end];

                if (current < 0)
                    start++;
                else if (current > 0)
                    end--;
                else
                {
                    result.Add(new int[] {nums[i], nums[start++], nums[end--]});

                    for(; start < end && nums[start] == nums[start - 1]; start++) {}

                    for(; start < end && nums[end] == nums[end + 1]; end--) {}
                }
            }
        }

        return result;
    }
}