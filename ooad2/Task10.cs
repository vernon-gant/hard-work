public abstract class ImportParser
{
    // template pattern is a good example of a case where we might want to prevent derived classes
    // overriding our core method which uses protected abstract methods which must be implemented
    // by the derived classes. For that in c# all methods are "non virtual" by default so if we do not mark
    // method as virtual then we can not override it.
    public ValueTask<ParsingResult> Parse(Stream stream)
    {
        await CreatedReader(stream);

        while (await Read())
        {
            string currentRow = GetCurrentRow();
            ...
        }
    }

    protected abstract ValueTask CreatedReader(Stream stream);

    protected abstract ValueTask<bool> Read();

    protected abstract string GetCurrentRow();
}