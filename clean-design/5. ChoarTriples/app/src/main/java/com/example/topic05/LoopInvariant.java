package com.example.topic05;

public class LoopInvariant {

    // {P: arr.length > 0} findMax(arr) {Q: result = max(arr)}
    public static int findMax(int[] arr) {
        int max = arr[0];
        for (int i = 1; i < arr.length; i++) {
            if (arr[i] > max) {
                max = arr[i];
            }
        }
        return max;
    }

    /*
     * Precondition: P: arr.length > 0
     *     The array must contain at least one element.
     *
     * Function body:
     *     1. Initialize max = arr[0];
     *        max initially holds the value of the first element.
     *
     *     2. Iterate through the array from index 1 to arr.length - 1:
     *        if (arr[i] > max), update max = arr[i].
     *
     *     3. Return max as the result.
     *
     * Postcondition: Q: result = max(arr)
     *     The function returns the maximum value present in the array arr.
     *
     * Loop invariant:
     *     At the beginning and end of each iteration (for each i),
     *     the following invariant holds:
     *
     *     I: max = maximum(arr[0], arr[1], ..., arr[i-1])
     *
     * Proof steps:
     *
     *     1. Initialization:
     *         Before entering the loop, max = arr[0].
     *         The invariant holds since max indeed equals the maximum of the single-element subarray [arr[0]].
     *
     *     2. Maintenance (Inductive step):
     *         Assume the invariant I holds at the start of iteration i:
     *         max = maximum(arr[0], ..., arr[i-1]).
     *
     *         During iteration i:
     *             If arr[i] > max, then max is updated to arr[i], thus ensuring:
     *                 max = maximum(arr[0], ..., arr[i-1], arr[i]).
     *
     *             If arr[i] <= max, max remains unchanged, thus still:
     *                 max = maximum(arr[0], ..., arr[i-1], arr[i]).
     *
     *         Therefore, the invariant remains true after each iteration.
     *
     *     3. Termination:
     *         The loop terminates when i = arr.length.
     *         By loop invariant, at this point, max = maximum(arr[0], ..., arr[arr.length-1]),
     *         which is the maximum of the entire array.
     *
     *     Thus, upon termination, the function returns max, which satisfies the postcondition:
     *     result = max(arr).
     *
     * Therefore, the function findMax(arr) correctly computes the maximum value in the array arr.
     */
}
