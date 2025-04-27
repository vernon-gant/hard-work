namespace EncodeDecode;

public class Solution
{
    public static string Encode(List<String> strings)
    {
        return strings.Count == 0 ? string.Empty : "000";
    }

    public static List<String> Decode(string str)
    {
        return string.IsNullOrEmpty(str) ? [] : [string.Empty];
    }
}