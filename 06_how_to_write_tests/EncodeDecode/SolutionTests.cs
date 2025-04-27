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
        yield return new TestCaseData(new List<string> { "hello" });
    }
}