public class Solution
{
    public static string Encode(List<String> strings)
    {
        StringBuilder encodedString = new StringBuilder();

        foreach(var word in strings)
        {
	        encodedString.Append(word.Length.ToString().PadLeft(3));
	        encodedString.Append(word);
        }

        return encodedString.ToString();
    }

    public static List<String> Decode(string str)
    {
        List<String> decodedString = new List<string>();
        int strIdx = 0;

        do
        {
            var wordLen = GetLength(str, strIdx);
      	    var wordStart = strIdx + 3;
	          var word = str[wordStart..(wordStart + wordLen)];
	          decodedString.Add(word);
	          strIdx = wordStart + wordLen;
        } while(strIdx != str.Length);

        return decodedString;
    }

    private static int GetLength(string encoded, int firstNumberIdx)
    {
      int start = firstNumberIdx, end = firstNumberIdx + 3;
      for (; Char.IsWhiteSpace(encoded[start]); start++) {}
      return int.Parse(encoded[start..end]);
    }
}
