public class Solution
{
    public string RemoveDuplicates(string s)
    {
        Stack<char> letters = new();

        foreach(char letter in s)
        {
            if (letters.Count > 0 && letters.Peek() == letter)
            {
                letters.Pop();
                continue;
            }

            letters.Push(letter);
        }

        var result = new char[letters.Count];

        for(var idx = letters.Count - 1; idx >= 0; idx--)
        {
            result[idx] = letters.Pop();
        }

        return new string(result);
    }
}
