public class Solution
{
    public static int[][] FlipAndInvertImage(int[][] image)
    {
        for(int row = 0; row < image.Length; row++)
        {
            int middle = image.Length % 2 == 0 ? image.Length / 2 : (image.Length + 1) / 2;

            for(int col = 0; col < middle; col++)
            {
                var first = image[row][col] == 0 ? 1 : 0;
 	            var last = image[row][image.Length - 1 - col] == 0 ? 1 : 0;
 	            image[row][image.Length - 1 - col] = first;
                image[row][col] = last;
            }
        }
        return image;
    }
}