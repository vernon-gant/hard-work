package com.example.topic02;

import org.junit.jupiter.api.Test;

import java.util.List;

import static org.assertj.core.api.BDDAssertions.then;

public class GradeCalculatorTests {

    @Test
    public void GivenNullList_WhenCalculatingAverage_ThenEmptyStatusReturned() {
        // Given
        // When
        var result = GradeCalculator.calculateAverage(null);

        // Then
        then(result.isT1()).isTrue();
    }

    @Test
    public void GivenEmptyList_WhenCalculatingAverage_ThenEmptyStatusReturned() {
        // Given
        // When
        var result = GradeCalculator.calculateAverage(List.of());

        // Then
        then(result.isT1()).isTrue();
    }

    @Test
    public void GivenListWithNegativeValues_WhenCalculatingAverage_ThenNegativeStatusReturned() {
        // Given
        List<Integer> grades = List.of(5, -1, 7);

        // When
        var result = GradeCalculator.calculateAverage(grades);

        // Then
        then(result.isT2()).isTrue();
    }

    @Test
    public void GivenListWithNullValues_WhenCalculatingAverage_ThenNullStatusReturned() {
        // Given
        List<Integer> grades = new java.util.ArrayList<>(java.util.Arrays.asList(1, null, 3));

        // When
        var result = GradeCalculator.calculateAverage(grades);

        // Then
        then(result.isT3()).isTrue();
    }

    @Test
    public void GivenValidGrades_WhenCalculatingAverage_ThenCorrectMeanReturned() {
        // Given
        List<Integer> grades = List.of(4, 6, 10);

        // When
        var result = GradeCalculator.calculateAverage(grades);

        // Then
        then(result.isT0()).isTrue();
        then(result.asT0()).isEqualTo(6.666666666666667);
    }
}
