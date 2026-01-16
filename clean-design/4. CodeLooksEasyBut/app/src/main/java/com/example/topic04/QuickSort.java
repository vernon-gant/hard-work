package com.example.topic04;

public class QuickSort {

    // {P: arr.length > 0} quickSort(arr) {Q: arr is sorted}
    public static void quickSort(int[] arr) {
        if (arr.length == 0) {
            return;
        }
        quickSort(arr, 0, arr.length - 1);
    }

    /*
     * Precondition: P: arr.length > 0
     *     The array should not be empty.
     *
     * Function body:
     *     Calls quickSort(arr, 0, arr.length - 1) to sort the entire array.
     *
     * Postcondition: Q: arr is sorted
     *     The entire array is sorted in ascending order.
     *
     * Proof steps:
     *     1. quickSort(arr, 0, arr.length - 1) is invoked.
     *     2. The precondition is satisfied:
     *         - arr.lengt > 0 per function precondition
     *         - low = 0 is valid as array length > 0.
     *         - high = arr.length - 1 is valid because it is always less than arr.length.
     *     3. By correctness proof of quickSort(arr, low, high), the array is sorted.
     */

    // {P: arr.length > 0 and 0 <= low <= high < arr.length} quickSort(arr, low, high) {Q: arr[low..high] is sorted}
    private static void quickSort(int[] arr, int low, int high) {
        if (low < high) {
            int pi = partition(arr, low, high);
            quickSort(arr, low, pi - 1);
            quickSort(arr, pi + 1, high);
        }
    }

    /*
     * Precondition: P: arr.length > 0 and 0 <= low <= high < arr.length
     *
     * Function body:
     *     1. If low < high:
     *        - Partition the array around a pivot.
     *        - Recursively sort left and right partitions.
     *
     * Postcondition: Q: arr[low..high] is sorted
     *
     * Proof steps:
     *     1. Initialization:
     *        Base case low >= high to satisfy the postcondition.
     *
     *     2. Recursive step:
     *        - Correct partitioning ensures pivot is correctly placed.
     *        - Preconditions for recursive calls are satisfied.
     *        - Recursively sorting partitions ensures subarrays are sorted.
     *
     *     3. Termination:
     *        Each recursion reduces subarray size, thus termination is guaranteed.
     */

    // {P: arr.length > 0 and 0 <= low <= high < arr.length} partition(arr, low, high) {Q: arr[low..pivot-1] <= arr[pivot] <= arr[pivot+1..high]}
    private static int partition(int[] arr, int low, int high) {
        int pivot = arr[high];
        int i = low;

        for (int j = low; j < high; j++) {
            if (arr[j] <= pivot) {
                swap(arr, i, j);
                i++;
            }
        }
        swap(arr, i, high);
        return i;
    }

    /*
     * Precondition: P: arr.length > 0 and 0 <= low <= high < arr.length
     *
     * Function body:
     *     1. Select pivot = arr[high]. (valid by precondition)
     *     2. Initialize partition index i = low (valid as low <= high).
     *     3. Loop through the array from low to high - 1:
     *         a. If arr[j] <= pivot, swap arr[i] and arr[j] and increment i.
     *         b. Ensures elements <= pivot are on the left of i.
     *     4. Swap pivot element arr[high] with arr[i].
     *        Pivot placed correctly at index i.
     *     5. Return pivot index i.
     *
     * Postcondition: Q: arr[low..pivot-1] <= arr[pivot] <= arr[pivot+1..high]
     *
     * Loop invariant:
     *     At any point during the loop execution:
     *     - Elements from low to (i-1) <= pivot.
     *     - Elements from i to (j-1) > pivot.
     *     - Element at high is pivot.
     *
     * Proof:
     *     - Initialization: Initially, no elements have been checked, trivially holds.
     *     - Maintenance:
     *         - At each iteration, if arr[j] <= pivot, it is swapped into the lower partition,
     *           maintaining the invariant.
     *         - Elements > pivot naturally move to the right partition.
     *     - Termination:
     *         - The loop ends when all elements have been checked, ensuring pivot placement correctness.
     *         - Pivot element is placed exactly between elements <= pivot and elements > pivot.
     */

    private static void swap(int[] arr, int i, int j) {
        int temp = arr[i];
        arr[i] = arr[j];
        arr[j] = temp;
    }
}