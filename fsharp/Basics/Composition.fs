module CompositionTasks

// 20.3.1
let vat (n : int) (x : float) = x + (float(n) / 100.0 * x)

// 20.3.2
let unvat n x = x / (float(n) / 100.0 + 1.0)

// 20.3.3
let rec min f =
    let rec minRec x =
        match f x with
        | 0 -> x
        | _ -> minRec (x + 1)
    minRec 1