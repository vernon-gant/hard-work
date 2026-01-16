public class Solution
{
    public int FindMinArrowShots(int[][] points)
    {
        Array.Sort(points, (x, y) => x[0].CompareTo(y[0]));

        int result = 0, j = 0;

        for(int i = 0; i < points.Length; i = j)
        {
            result++;

            var current = points[i];

            for(j = i + 1; j < points.Length; j++)
            {
                if (current[1] < points[j][0])
                    break;

                current[0] = Math.Max(current[0], points[j][0]);
                current[1] = Math.Min(current[1], points[j][1]);
            }
        }

        return result;
    }

    // Simplified
    public int FindMinArrowShots(int[][] points)
    {
        Array.Sort(points, (x, y) => x[1].CompareTo(y[1]));

        int result = 1;
        int lastDistance = points[0][1];

        for(int i = 1; i < points.Length; i++)
        {
            if (lastDistance < points[i][0])
            {
                result++;
                lastDistance = points[i][1];
            }
        }

        return result;
    }
}