package com.example.topic03;

import java.util.*;
import java.util.concurrent.*;
import java.util.concurrent.locks.*;

// We were programming a game on a C++ course where multiptle robots are mining a field simultaneously - some examples using Java primitives
public class MiningController {

    // 1. synchronized — atomically mark a cell as mined
    static class SynchronizedField {
        private final boolean[][] mined;
        public SynchronizedField(int rows, int cols) {
            mined = new boolean[rows][cols];
        }

        public synchronized boolean claimCell(int r, int c, String robotId) {
            if (mined[r][c]) return false;
            mined[r][c] = true;
            return true;
        }
    }

    // 2. ReentrantLock — reserve a cell with rollback and timeout
    static class LockingField {
        private final ReentrantLock[][] locks;
        private final boolean[][] mined;
        public LockingField(int rows, int cols) {
            locks = new ReentrantLock[rows][cols];
            mined = new boolean[rows][cols];
            for (int i = 0; i < rows; i++) for (int j = 0; j < cols; j++)
                locks[i][j] = new ReentrantLock();
        }

        public boolean reserveCell(int r, int c, long timeout, TimeUnit unit)
                throws InterruptedException, TimeoutException {
            if (!locks[r][c].tryLock(timeout, unit)) {
                throw new TimeoutException("Lock timeout for cell " + r + "," + c);
            }
            try {
                if (mined[r][c]) return false;
                mined[r][c] = true;

                if (Math.random() < 0.01) {
                    mined[r][c] = false;
                    return false;
                }
                return true;
            } finally {
                locks[r][c].unlock();
            }
        }
    }


    // Limit the amount of robots with semaphore
    static class DangerZoneController {
        private final Semaphore capacity;
        public DangerZoneController(int maxRobots) {
            capacity = new Semaphore(maxRobots, true);
        }

        public boolean enter(long timeout, TimeUnit unit) throws InterruptedException {
            return capacity.tryAcquire(timeout, unit);
        }

        public void exit() {
            capacity.release();
        }
    }

    // We can use the rwLock to allow parallel reads and only exclusive writing to avoid corruptions
    static class FieldMap {
        private final ReentrantReadWriteLock rwLock = new ReentrantReadWriteLock();
        private Map<String, Object> terrainData = new HashMap<>();

        public Object readTerrain(String key) {
            rwLock.readLock().lock();
            try {
                return terrainData.get(key);
            } finally {
                rwLock.readLock().unlock();
            }
        }


        public void reloadMap(Map<String, Object> newData, Consumer<Map<String,Object>> onUpdate) {
            rwLock.writeLock().lock();
            try {
                terrainData = new HashMap<>(newData);
            } finally {
                rwLock.writeLock().unlock();
            }
            onUpdate.accept(Collections.unmodifiableMap(terrainData));
        }
    }

}