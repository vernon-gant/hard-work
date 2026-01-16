using SharpFuzz;
using WarAndPeace;

Fuzzer.OutOfProcess.Run(stream =>
{
    using var reader = new StreamReader(stream);
    var lines = new List<string>();
    for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
    {
        lines.Add(line);
    }

    var countResult = WordCounting.CountWords(lines);
});