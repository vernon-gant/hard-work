// BEFORE

public class XlsxReader : ImportReader
{
    private IExcelReader _reader;

    public bool CanProcess(string format) => format == "xlsx";

    public List<...> Read(Stream importStream)
    {
        ...
    }

    internal void SetReader(IExcelReader reader)
    {
        _reader = reader;
    }
}

// AFTER

public class XlsxReader : ImportReader
{
    private IExcelReader _reader;

    public bool CanProcess(string format) => format == "xlsx";

    public List<...> Read(Stream importStream)
    {
        ...
    }
}