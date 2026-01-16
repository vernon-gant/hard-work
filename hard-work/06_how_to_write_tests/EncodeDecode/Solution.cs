using System.Text;

namespace EncodeDecode;

public class Solution
{
    public static string Encode(List<String> strings)
    {
        if (strings.Count == 0)
            return string.Empty;

        var builder = new StringBuilder();
        foreach (var word in strings)
        {
            builder.Append(word.Length);
            builder.Append(':');
            builder.Append(word);
        }

        return builder.ToString();
    }

    public static List<String> Decode(string str)
    {
        var decodedStrings = new List<string>();
        int idx = 0;

        while (idx < str.Length)
        {
            int colonIdx = str.IndexOf(':', idx);
            int length = int.Parse(str.Substring(idx, colonIdx - idx));

            int wordStart = colonIdx + 1;
            string word = str.Substring(wordStart, length);
            decodedStrings.Add(word);

            idx = wordStart + length;
        }

        return decodedStrings;
    }
}