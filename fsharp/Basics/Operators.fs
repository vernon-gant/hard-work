// 16.1
open System

let notDivisible (n, m) = m % n = 0

// 16.2
let prime n =
    if n < 2 then
        false
    else
        let limit = int (sqrt (float n))
        seq { 2..limit } |> Seq.forall (fun d -> n % d <> 0)

let test x =
    let a, b = x
    match x with
    | 0, 1 -> 1