// 48.4.1
let rec fibo1 n n1 n2 =
    if n = 0 then n2
    else fibo1 (n - 1) (n1 + n2) n1

// 48.4.2
let rec fibo2 n c =
    if n = 0 then c 0
    elif n = 1 then c 1
    else fibo2 (n - 1) (fun result1 ->
         fibo2 (n - 2) (fun result2 ->
         c (result1 + result2)))

// 48.4.3
let bigList n k =
    let result = List.init n (fun _ -> 1)
    k result
