namespace EncodeDecode;

public class Solution
{
    public static string Encode(List<String> strings)
    {
        if (strings.Count == 0)
            return string.Empty;

        if (strings.Count == 1)
        {
            var word = strings[0];
            return word.Length.ToString("D3") + word;
        }

        return string.Empty;
    }

    public static List<String> Decode(string str)
    {
        if (string.IsNullOrEmpty(str))
            return new List<string>();

        if (str.Length >= 3)
        {
            int length = int.Parse(str[..3]);
            string word = str.Substring(3, length);
            return [word];
        }

        return [];
    }
}