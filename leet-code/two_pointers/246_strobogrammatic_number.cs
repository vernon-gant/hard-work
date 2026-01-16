using System;
using System.Collections.Generic;

public class Solution
{
    public static bool IsStrobogrammatic(string num)
    {
      var numberToRotated = new Dictionary<char, char>()
      {
        { '0', '0'},
        { '1', '1' },
        { '6', '9' },
        { '8', '8' },
        { '9', '6' }
      };
      int start = 0, end = num.Length - 1;
      while (start <= end)
      {
        var areSameWhenRotated = numberToRotated.TryGetValue(num[start], out var rotated) && num[end] == rotated;

        if (!areSameWhenRotated) return false;

        start++;
        end--;
      }

      return true;
    }
}