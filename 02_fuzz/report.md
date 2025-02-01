# Fuzz testing

Did not know at all that there exists such testing approach. Although primarily developed for testing programs written in unsafe languages like C and C++ we can easliy use it in our favourite managed languages to check for unhandled `OutOfBounds` or `FormatException`. I deciede to research how we can apply this art of testing for C# programs.

## Environment

First problem or even thought was that we need a way how to instrument our C# code, because after reading the AFL short docs I quickly understood that it needs access to native code as it instruments the paths inside of the code to manipulate the input later in order to explore new paths and increase coverage.

This problem has already been solved by smart people :) There is a library called `SharpFuzz` which offers a command line tool for instrumenting the C# dlls. As turned out later this might be also the root of problems.

To make things work best way is to use the WSL, install .NET 8 and then install the afl tool. Or we can also download the sources and build it ourselves with make.

Actualy that's it. Library also offers a ps script for fast run of afl, so I also installed powershell on wsl :) The rest is just to configure PUT projects.

One important point, which was not mentioned in the lib docs and confused me at the beginning,is that the project which will be started by afl must not contain any code which must be tested. In other words we must have a dedicated class library wich we reference from the running project. Because all referenced dlls are instrumnented by the command line tool before the afl call in the ps script.

## Projects

### 1. Word counting

Although we can try to test any method or function using fuzzing, in some cases it will be very hard to come up with meaningful strategy when we have a function which takes 2 ints and one custom data type with 1 double and one bool inside. Best projects to test are some string driven projects. I had only one such project - the WordCounting project which was our final assignment on the functional programming course.

I did not know what to expect and just composed the corpus using different patterns in text and started the fuzzing session. After 16 minutes and 150 covered paths I realized that I do not get here any crashes, simply because I did not find myself any places where I would have a `IndexOutOufBounds` trap or smth else. We just split and count unique. So no crashes found.

![1](https://github.com/user-attachments/assets/16bbec43-31b4-476d-921a-3107f1ab7be9)

Now I understand also why we need to execute these tests on stong machines - my dell was not ready for such load. I accidently run the same test 2 times and the CPU load went up to 60%...


### 2. HTML to MD

So I thought we need something highly data driven and format dependent so that our smart tool can challenge the logic. Best projects for this are parsers, converters and so on. But my mistake was that I took a project which has 1k stars on github, so people probably trust it and it is well tested in production(probably). After one hour of tests - no crashes found.

![2](https://github.com/user-attachments/assets/9f1f11af-3d26-488c-b654-a7791fc74409)


### 3. INI, TOML and instrumentation problems...

That's why I decided to take a project with 20-100 starts on github and test it. And here it is - `perform_dry_run` problem.

![3](https://github.com/user-attachments/assets/45f2e157-a0eb-40e0-9f9a-e80b326fd22c)

For some reason when I tried to test 3 completely different projects - 2 for INI parsing and 1 for TOML file parsing, I got crashes for ***valid*** test cases before start of the fuzzer. And this is really weird, because after running same input from main manually I did not encounter any exception at all. I repeated this for 3 different libraries, they are completely unrelated.

The most interesting is that these exceptions occured event when I set the whole code within the calback for fuzzer in `try/catch` block

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

Even in this case the test crashed with the same message. I spent with this problem 10+ hours and I can not really find a root cause, because there is no direct way to track where the exception occurs - this is prerun of the whole corpus body and afl does not report it as crash. The only guess which I have is some internal problem with the binary aka something went wrong during instrumentation or whatever.


## Conclusion

The problem above is probably the price which we - developers on high level languages - have to pay for using such tools. Libraries do the whole magic behind the scenes but sometimes because of this magic we can not track what is happenning in reality.

However the idea seems veeery interesting to me. Will definitely ask my team lead about it - maybe we can test smth in our project with this magic.

Now I know how to quickly setup the C# fuzz testing and even what are dictionaries in the context of fuzz testing :) Can be also proud of my word count project - no crashes is already a win...
