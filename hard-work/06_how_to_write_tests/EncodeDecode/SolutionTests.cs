namespace EncodeDecode;

public class SolutionTests
{
    [TestCaseSource(nameof(TestData))]
    public void EncodeDecode_ShouldReturnOriginalList(List<string> input)
    {
        var encoded = Solution.Encode(input);
        var decoded = Solution.Decode(encoded);

        Assert.That(decoded, Is.EqualTo(input));
    }

    private static IEnumerable<TestCaseData> TestData()
    {
        yield return new TestCaseData(new List<string>());
        yield return new TestCaseData(new List<string> { "" });
        yield return new TestCaseData(new List<string> { "test" });
        yield return new TestCaseData(new List<string> { "hello", "my", "friend", "!" });
        yield return new TestCaseData(new List<string>
        {
            "leetcode leetcode leetcode leetcode leetcode leetcode leetcode leetcode leetcode",
            "is cool is cool is cool is cool is cool is cool is cool is cool is cool", "is cool is cool is cool is cool is coolis cool"
        });
        yield return new TestCaseData(new List<string> { "54he36llo", "6my3453", "6fr436ie3436nd6" });
        yield return new TestCaseData(new List<string> { "\u0001\u0002\u0003", "\u0004\u0005" });
        yield return new TestCaseData(new List<string> { "áéíóú", "ßçü" });
        yield return new TestCaseData(new List<string> { "!@#$%^&*()", "<>[]{}" });
        yield return new TestCaseData(new List<string> { "!@#$%^&*()_;:'\"|\\]|\\||-" });
        yield return new TestCaseData(new List<string> { "!@#$%", "^&*()", "_;:'\"", "|\\]|\\||-" });
        yield return new TestCaseData(new List<string> { "hello!@#", "world%^&", "test_|:" });
    }
}