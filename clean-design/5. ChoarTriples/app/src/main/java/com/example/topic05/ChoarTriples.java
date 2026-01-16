package com.example.topic05;

public class ChoarTriples {
    // {P: true} max(a, b) {Q: result = a, if a >= b, else result = b}
    public static int max(int a, int b) {
        return (a >= b) ? a : b;
    }
    /*
     * Precondition: P: true
           The precondition is always true, meaning there are no restrictions on the input values a and b before execution.

       Function body:
           Return statement: return (a >= b) ? a : b;
           The function returns the greater value between a and b.

       Postcondition: Q: result = a if a >= b, else result = b
           This means after the execution of the function, the result will be the maximum of the two values, a and b.

       Proof steps:

           1. Assume that the precondition P is true. Since it is always true, there are no restrictions on inputs.

           2. Execute the function body:

              return (a >= b) ? a : b;

             After this execution, the result of the function is:
                - a, if a >= b
                - b, if a < b

       Check the postcondition after execution:
          Postcondition Q: the result equals the greater of the two values a and b.
          This condition will indeed be true after the execution of return (a >= b) ? a : b.

       Therefore, the function max(a, b) correctly returns the maximum value of two numbers
     */

    // {P: true} abs(x) {Q: result = x if x >= 0, else result = -x}
    public static int abs(int x) {
        return (x >= 0) ? x : -x;
    }

    /*
     * Precondition: P: true
           The precondition is always true, meaning there are no restrictions on the input value x before execution.

       Function body:
           Return statement: return (x >= 0) ? x : -x;
           The function returns the absolute value of x.

       Postcondition: Q: result = x if x >= 0, else result = -x
           This means after the execution of the function, the result will be the absolute value of x.

       Proof steps:

           1. Assume that the precondition P is true. Since it is always true, there are no restrictions on inputs.

           2. Execute the function body:

              return (x >= 0) ? x : -x;

              After this execution, the result of the function is:
                  - x, if x >= 0
                  - -x, if x < 0

       Check the postcondition after execution:
           Postcondition Q: the result equals the absolute value of x.
           This condition will indeed be true after the execution of return (x >= 0) ? x : -x.

       Therefore, the function abs(x) correctly returns the absolute value
     */

    // {P: true} maxAbs(a, b) {Q: result = maximum(|a|, |b|)}
    public static int maxAbs(int a, int b) {
        int absA = abs(a);
        int absB = abs(b);
        return max(absA, absB);
    }

    /*\
     * Precondition: P: true
           The precondition is always true, meaning there are no restrictions on the input values a and b before execution.

       Function body:
           1. int absA = abs(a);
           This calculates the absolute value of a.

           2. int absB = abs(b);
           This calculates the absolute value of b.

           3. return max(absA, absB);
           This selects the maximum of the two absolute values absA and absB.

       Postcondition: Q: result = maximum(|a|, |b|)
           This means after the execution of the function, the result will be the greatest absolute value between a and b.

       Proof steps:

           1. Assume that the precondition P is true (it is always true).

           2. Execute the function body sequentially:

               absA = abs(a);
               absB = abs(b);

               After these steps, we have computed the absolute values |a| and |b|.

        3. Next:

            return max(absA, absB);

       By the proof of the max function, the result of this operation will be the greatest value between absA and absB, thus maximum(|a|, |b|).

       Therefore, the function maxAbs(a, b) correctly returns the maximum absolute value of two numbers
     */
}