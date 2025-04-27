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
            "leetcode leetcode leetcode leetcode leetcode leetcode leetcode leetcode leetcode leetcode leetcode leetcode leetcode leetcode leetcode leetcode leetcode leetcode leetcode leetcode leetcode leetcode leetcode leetcode",
            "is cool is cool is cool is cool is cool is cool is cool is cool is cool", "is cool is cool is cool is cool is coolis cool"
        });
    }
}