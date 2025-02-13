# Fuzz testing

Did not know at all that there exists such a testing approach. Although primarily developed for testing programs written in unsafe languages like C and C++, we can easily use it in our favorite managed languages to check for unhandled `OutOfBounds` or `FormatException`. I decided to research how we can apply this art of testing for C# programs.

## Environment

The first problem - or even thought - was that we need a way to instrument our C# code, because after reading the AFL short docs I quickly understood that it needs access to native code as it instruments the code paths to manipulate the input later in order to explore new paths and increase coverage.

This problem has already been solved by smart people :) There is a library called `SharpFuzz` which offers a command line tool for instrumenting the C# DLLs. As it turned out later, this might also be the root of the problems.

To make things work, the best way is to use WSL, install .NET 8 and then install the AFL tool. Or we can also download the sources and build it ourselves with make.

Actually, that's it. The library also offers a PowerShell script for a fast run of AFL, so I also installed PowerShell on WSL :) The rest is just to configure the projects under test.

One important point, which was not mentioned in the lib docs and confused me at the beginning, is that the project which will be started by AFL must not contain any code that is to be tested. In other words, we must have a dedicated class library which we reference from the running project, because all referenced DLLs are instrumented by the command line tool before the AFL call in the PowerShell script.

## Projects

### 1. Word counting

Although we can try to test any method or function using fuzzing, in some cases it will be very hard to come up with a meaningful strategy when we have a function which takes 2 ints and one custom data type with one double and one bool inside. The best projects to test are some string-driven projects. I had only one such project - the WordCounting project which was our final assignment on the functional programming course.

I did not know what to expect and just composed the corpus using different patterns in text and started the fuzzing session. After 16 minutes and 150 covered paths I realized that I did not get any crashes, simply because I did not find any places where I would have an `IndexOutOfBounds` trap or something else. We just split and count unique. So no crashes found.

// First report

### 2. HTML to MD

So I thought we needed something highly data-driven and format-dependent so that our smart tool can challenge the logic. The best projects for this are parsers, converters, and so on. But my mistake was that I took a project which has 1k stars on GitHub, so people probably trust it and it is well tested in production (probably). After one hour of tests - no crashes found.

// Second report

### 3. INI, TOML and instrumentation problems...

That's why I decided to take a project with 20–100 stars on GitHub and test it. And here it is - the `perform_dry_run` problem.

// perform dry run

For some reason, when I tried to test three completely different projects - two for INI parsing and one for TOML file parsing - I got crashes for ***valid*** test cases before the fuzzer even started. And this is really weird, because after running the same input from main manually, I did not encounter any exception at all. I repeated this for three different libraries; they are completely unrelated.

The most interesting part is that these exceptions occurred even when I wrapped the whole code in the callback for the fuzzer in a `try/catch` block:

```c#
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
```

Even in this case the test crashed with the same message. I spent over 10 hours on this problem and I cannot really find a root cause, because there is no direct way to track where the exception occurs - this is a prerun of the entire corpus, and AFL does not report it as a crash. The only guess I have is that there is some internal problem with the binary - aka, something went wrong during instrumentation or whatever.

## Conclusion

The problem above is probably the price we - developers in high- level languages - have to pay for using such tools. Libraries do the whole magic behind the scenes, but sometimes because of this magic we cannot track what is happening in reality.

However, the idea seems veeery interesting to me. I'll definitely ask my team lead about it - maybe we can test something in our project with this magic.

Now I know how to quickly set up C# fuzz testing and even what dictionaries are in the context of fuzz testing :) I can also be proud of my word count project - no crashes is already a win...