package com.example.topic02;

import org.junit.jupiter.api.Test;

import static org.assertj.core.api.BDDAssertions.then;

public class AverageCalculatorTests {
    @Test
    public void GiveEmptyArray_ThenZeroIsReturned() {
        // Given
        var input = new int[]{};

        // When
        var result = AverageCalculator.calculateAverage(input);

        // Then
        then(result).isEqualTo(0);
    }

    @Test
    public void GivenArrayWithPositiveNumbers_WhenCalculatingAverage_ThenReturnCorrectMean() {
        // Given
        int[] numbers = {1, 2, 3, 4, 5};

        // When
        double result = AverageCalculator.calculateAverage(numbers);

        // Then
        then(result).isEqualTo(3.0);
    }

    @Test
    public void GivenSingleElementArray_WhenCalculatingAverage_ThenReturnThatElement() {
        // Given
        int[] numbers = {42};

        // When
        double result = AverageCalculator.calculateAverage(numbers);

        // Then
        then(result).isEqualTo(42.0);
    }

    @Test
    public void GivenArrayWithNegativeNumbers_WhenCalculatingAverage_ThenReturnCorrectMean() {
        // Given
        int[] numbers = {-1, -2, -3, -4, -5};

        // When
        double result = AverageCalculator.calculateAverage(numbers);

        // Then
        then(result).isEqualTo(-3.0);
    }
}

/*
 * The general idea of checking the expected mean on valid inputs and also invalid one is asserted correctly. And the method seems to work fine. Every line of code is covered by the test.
 * The implementation seems correct and all tests pass but it still contains bugs because we did not cover our code with tests for extreme values. Yes we can have an array with millions of elements
 * and this will work with current implementation, however we can have a test case with three Integer.MAX_VALUE and everything breaks...

    @Test
    public void GivenArrayWithVeryLargeNumbers_ThenHandleOverflowCorrectly() {
        // Given
        int[] numbers = {Integer.MAX_VALUE, Integer.MAX_VALUE, Integer.MAX_VALUE};

        // When
        double result = AverageCalculator.calculateAverage(numbers);

        // Then
        then(result).isEqualTo((double) Integer.MAX_VALUE);
    }
    
 */