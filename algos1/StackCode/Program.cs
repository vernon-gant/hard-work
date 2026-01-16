using System;
using AlgorithmsDataStructures;

namespace StackCode
{
    class Program
    {

        static void Main(string[] args)
        {
            var equation = new Stack<string>();
            equation.Push("=");
            equation.Push("9");
            equation.Push("9");
            equation.Push("9");
            equation.Push("+");
            equation.Push("9");
            equation.Push("*");
            equation.Push("5");
            equation.Push("/");
            equation.Push("22");
            equation.Push("88");
            Console.WriteLine(SpecialTasks.PostfixEquation(equation));
        }

    }
}