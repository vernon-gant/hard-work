using MarkedNet;
using SharpFuzz;

Fuzzer.OutOfProcess.Run(s =>
{
    try
    {
        var html = new Marked().Parse(s);
        Console.WriteLine(html);
    }
    catch (Exception e)
    {

    }
});