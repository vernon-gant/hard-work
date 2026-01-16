public class Solution
{
    public string MinRemoveToMakeValid(string s)
    {
        Stack<int> indices = new();

        for(int i = 0; i < s.Length; i++)
        {
          bool isParanthesis = (s[i] == '(' || s[i] == ')');
          if (!isParanthesis) continue;

          if (indices.Count != 0 && s[indices.Peek()] == '(' && s[i] == ')')
          {
            indices.Pop();
          } else {
            indices.Push(i);
          }
        }

        var temp = s.ToCharArray();
        var result = new char[s.Length - indices.Count];

        while(indices.Count > 0)
        {
          temp[indices.Pop()] = '\0';
        }

        int resultIdx = 0;
        for(int i = 0; i < temp.Length; i++)
        {
          if (temp[i] == '\0') continue;

          result[resultIdx++] = temp[i];
        }

        return new string(result);
    }
}
