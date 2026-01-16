package com.example.topic02;

public class AverageCalculator
{
    public static double calculateAverage(int[] numbers)
    {
        if (numbers.length == 0)
            return 0;

        int sum = 0;
        for (int number : numbers) {
            sum += number;
        }

        return (double) sum / numbers.length;
    }
}