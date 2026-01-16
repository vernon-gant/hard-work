public class Solution 
{
    public static int FindBitwiseComplement(int num)
    {
        int nextTwoPower = 2;
	    for(;nextTwoPower <= num; nextTwoPower <<= 1) {}
	    return nextTwoPower ^ num;
    }
}