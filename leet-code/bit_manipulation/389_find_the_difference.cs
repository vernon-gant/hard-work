public class Solution
{
    public static int ExtraCharacterIndex(String str1, String str2)
	{
        int result = 0;

	    foreach(char letter in t)
	    {
	        result ^= (int) letter;
	    }

	    foreach(char letter in s)
	    {
	        result ^= (int) letter;
    	}

	    return (char) result;
    }
}