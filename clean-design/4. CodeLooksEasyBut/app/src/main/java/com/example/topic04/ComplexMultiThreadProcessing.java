package com.example.topic04;

import java.util.List;
import java.util.OptionalInt;
import java.util.Random;
import java.util.concurrent.*;
import java.util.stream.IntStream;

public class ComplexMultiThreadProcessing {
    private static final int SIZE = 1000000;
    private static final int THREADS = 4;

    // For original method
    private static final Random seededRandom1 = new Random(42);
    private static final int[] data = new int[SIZE];
    private static volatile int sum = 0;

    public static void Original() {
        for (int i = 0; i < SIZE; i++) {
            data[i] = seededRandom1.nextInt(100);
        }

        Thread[] threads = new Thread[THREADS];
        int chunkSize = SIZE / THREADS;

        for (int i = 0; i < THREADS; i++) {
            final int start = i * chunkSize;
            final int end = (i + 1) * chunkSize;
            threads[i] = new Thread(() -> {
                int localSum = 0;
                for (int j = start; j < end; j++) {
                    localSum += data[j];
                }
                synchronized (ComplexMultiThreadProcessing.class) {
                    sum += localSum;
                }
            });
            threads[i].start();
        }

        for (int i = 0; i < THREADS; i++) {
            try {
                threads[i].join();
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }

        System.out.println("Original sum: " + sum);
    }

    public static void Simplified() {
        int chunk = SIZE / THREADS;
        Random seededRandom2 = new Random(42); // same seed as original
        try (ExecutorService pool = Executors.newFixedThreadPool(THREADS)) {
            List<Callable<Integer>> tasks = IntStream.range(0, THREADS)
                    .mapToObj(i -> (Callable<Integer>) () -> seededRandom2.ints(chunk, 0, 100).sum())
                    .toList();

            OptionalInt result = pool.invokeAll(tasks).stream()
                    .map(ComplexMultiThreadProcessing::executeToOptionalInt)
                    .reduce(ComplexMultiThreadProcessing::merge)
                    .orElse(OptionalInt.empty());

            System.out.println(result.isPresent() ? "Simplified sum: " + result.getAsInt() : "Simplified: Error occurred");
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
    }

    private static OptionalInt executeToOptionalInt(Future<Integer> future)
    {
        try {
            return OptionalInt.of(future.get());
        } catch (Exception e) {
            return OptionalInt.empty();
        }
    }

    private static OptionalInt merge(OptionalInt a, OptionalInt b)
    {
        return a.isPresent() && b.isPresent() ? OptionalInt.of(a.getAsInt() + b.getAsInt()) : OptionalInt.empty();
    }
}