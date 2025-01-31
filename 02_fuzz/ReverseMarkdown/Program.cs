using SharpFuzz;

Fuzzer.OutOfProcess.Run(input =>
{
    var converter = new ReverseMarkdown.Converter();

    converter.Convert(input);
});