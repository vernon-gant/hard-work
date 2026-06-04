// 16.1
let notDivisible (n, m) = m % n = 0

// 16.2
let prime n =
    if n < 2 then false
    else
        let limit = int (sqrt (float n))
        seq { 2 .. limit } |> Seq.forall (fun d -> n % d <> 0)