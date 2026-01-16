package com.example.topic04;

import java.util.*;
import java.util.stream.IntStream;

public final class FunctionalGrid<T> {
    private final CellContent<T>[][] grid;

    @SuppressWarnings("unchecked")
    public FunctionalGrid(T[][] raw) {
        int rows = raw.length;
        int cols = raw[0].length;
        grid = new CellContent[rows][cols];

        for (int r = 0; r < rows; r++) {
            for (int c = 0; c < cols; c++) {
                grid[r][c] = raw[r][c] == null ? new EmptyCell<>() : new OccupiedCell<>(raw[r][c]);
            }
        }
    }

    private FunctionalGrid(CellContent<T>[][] grid) {
        this.grid = grid;
    }

    public int getRowCount() {
        return grid.length;
    }

    public int getColumnCount() {
        return grid[0].length;
    }

    public FunctionalGrid<T> delete(Cell cell) {
        if (grid[cell.row()][cell.col()].isEmpty()) {
            throw new IllegalStateException("Cell already empty");
        }

        var copy = cloneGrid();
        copy[cell.row()][cell.col()] = new EmptyCell<>();
        return new FunctionalGrid<>(copy);
    }

    public FunctionalGrid<T> addTop(int column, T value) {
        if (!isValidColumnIndex(column)) throw new IndexOutOfBoundsException();

        if (isColumnFull(column) || grid[0][column].isOccupied()) {
            throw new IllegalStateException("Cannot add to full column");
        }

        var copy = cloneGrid();
        copy[0][column] = new OccupiedCell<>(value);
        return new FunctionalGrid<>(copy);
    }

    public FunctionalGrid<T> shiftDown(int column) {
        if (!isValidColumnIndex(column)) throw new IndexOutOfBoundsException();
        if (isColumnFull(column) || !canShiftDown(column)) throw new IllegalStateException("Cannot shift down");

        var copy = cloneGrid();

        int lowestEmpty = -1;
        for (int r = getRowCount() - 1; r >= 0; r--) {
            if (copy[r][column].isEmpty()) {
                lowestEmpty = r;
                break;
            }
        }

        for (int r = lowestEmpty; r > 0; r--) {
            copy[r][column] = copy[r - 1][column];
        }

        copy[0][column] = new EmptyCell<>();
        return new FunctionalGrid<>(copy);
    }

    public boolean canShiftDown(int column) {
        return IntStream.range(0, getRowCount() - 1).anyMatch(r -> grid[r][column].isOccupied() && grid[r + 1][column].isEmpty());
    }

    public boolean isColumnFull(int column) {
        return IntStream.range(0, getRowCount())
                .allMatch(r -> grid[r][column].isOccupied());
    }

    private boolean isValidColumnIndex(int column) {
        return column >= 0 && column < getColumnCount();
    }

    private CellContent<T>[][] cloneGrid() {
        @SuppressWarnings("unchecked")
        var copy = new CellContent[getRowCount()][getColumnCount()];
        for (int r = 0; r < getRowCount(); r++) {
            System.arraycopy(grid[r], 0, copy[r], 0, getColumnCount());
        }
        return copy;
    }

    // Nested helper types
    public sealed interface CellContent<T> permits EmptyCell, OccupiedCell {
        boolean isEmpty();
        boolean isOccupied();
    }

    public static final class EmptyCell<T> implements CellContent<T> {
        @Override public boolean isEmpty() { return true; }
        @Override public boolean isOccupied() { return false; }
    }

    public static final class OccupiedCell<T> implements CellContent<T> {
        private final T value;
        public OccupiedCell(T value) { this.value = value; }
        public T value() { return value; }
        @Override public boolean isEmpty() { return false; }
        @Override public boolean isOccupied() { return true; }
    }

    public record Cell(int row, int col) {}
}
