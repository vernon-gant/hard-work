/*
 * ALGORITHM 3Sum Closest(INPUTS, TARGET):
 *     SORT(inputs)
 *     RESULT = - INT.MIN
 *     FOR i = 0 TO [LEN(INPUTS) - 3]
 *         // INVARIANT :
 *         //   PROCESSED = {(INPUTS[a], INPUTS[b], INPUTS[c]) | a ∈ [0, i] ^ b ∈ [i + 1, LEN(inputs)) ^ c ∈ [i + 1, LEN(INPUTS)) ^ b < c}
 *         //   RESULT = MIN(s - target | s ∈ SUMS | SUMS = {INPUTS[a] + INPUTS[b] + INPUTS[c] | (INPUTS[a], INPUTS[b], INPUTS[c] ∈ PROCESSED)})
 *         START, END = i + 1, LEN(inputs) - 1
 *
 *         WHILE START < END
 *             // INVARIANT :
 *             //     NEXT
 *             CURRENT = INPUTS[i] + INPUTS[START] + INPUTS[END]
 *
 *             IF ABS(CURRENT - TARGET) < ABS(RESULT - TARGET):
                   RESULT = CURRENT
 *
 *             IF CURRENT < TARGET: START++
 *             ELSE IF CURRENT > TARGET: END--
 *             ELSE RETURN TARGET
 *     RETURN RESULT
 */

public class Solution
{
    public int ThreeSumClosest(int[] nums, int target)
    {
        Array.Sort(nums);

        var result = nums[0] + nums[1] + nums[2];

        for(int i = 0; i < nums.Length - 2; i++)
        {
           int start = i + 1, end = nums.Length - 1;

           while(start < end)
           {
               var current = nums[i] + nums[start] + nums[end];

                if (Math.Abs(current - target) < Math.Abs(result - target))
                    result = current;

               if (current < target)
                  start++;
               else if (current > target)
                  end--;
               else
                  return target;
           }
        }

        return result;
    }
}