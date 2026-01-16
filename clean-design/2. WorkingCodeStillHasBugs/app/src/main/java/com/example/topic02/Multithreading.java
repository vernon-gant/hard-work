package com.example.topic02;

import java.util.concurrent.atomic.AtomicInteger;

public class Multithreading {
    private static AtomicInteger counter = new AtomicInteger();
    private static final Object lock1 = new Object();
    private static final Object lock2 = new Object();

    /* In the wrong example the problem was with the fact that the increment operation is not atomic by its nature. It consists of 3 operations : read the value, increase the value and write the value.
       When multiple threads try to do this it may come to a situation when two threads read 10 and increased the value to 11, however one thread will write the value before another one and the seond
       one will just overwrite it and the result will be 11 and not 12 as anticipated. To fix this we could use an atomic integer implementation which is a primitive for atomic work with int. */

    public static void RaceCondition() {
        int numberOfThreads = 10;
        Thread[] threads = new Thread[numberOfThreads];

        for (int i = 0; i < numberOfThreads; i++) {
            threads[i] = new Thread(() -> {
                for (int j = 0; j < 100000; j++) {
                    counter.incrementAndGet();
                }
            });
            threads[i].start();
        }

        for (int i = 0; i < numberOfThreads; i++) {
            try {
                threads[i].join();
            } catch (InterruptedException e) {
                e.printStackTrace();
            }
        }

        System.out.println("Final counter value: " + counter);
    }

    /* The problem in the wrong example happens because of the distinct order of resources locking. Thread one acquires lock1 and then waits what creates space for the thread 2 to for sure acquire the lock2.
       Then they both do not release the lock and wait for each other producing deadlocks. There are multiple solutions to this problem. 1) Make the order of acquiring locks the same to prevent
       such cases. 2) Have timeouts with modern lock objects. 3) Just restructure program and avoid multiple locks. */

    public static void Deadlock() {
        Thread thread1 = new Thread(() -> {
            synchronized (lock1) {
                System.out.println("Thread 1 acquired lock1");

                try {
                    Thread.sleep(50);
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }

                synchronized (lock2) {
                    System.out.println("Thread 1 acquired lock2");
                }
            }
        });

        Thread thread2 = new Thread(() -> {
            synchronized (lock1) {
                System.out.println("Thread 2 acquired lock2");

                try {
                    Thread.sleep(50);
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }

                synchronized (lock2) {
                    System.out.println("Thread 2 acquired lock1");
                }
            }
        });

        thread1.start();
        thread2.start();

        try {
            thread1.join();
            thread2.join();
        } catch (InterruptedException e) {
            e.printStackTrace();
        }

        System.out.println("Finished");
    }
}