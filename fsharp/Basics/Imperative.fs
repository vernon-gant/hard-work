// 47.4.1
let f n =
    let mutable result = 1
    for i in 2..n do
        result <- result * i
    result

// 47.4.2
let fibo n =
    let mutable prev = 0
    let mutable curr = 1
    for _ in 1..n do
        let next = prev + curr
        prev <- curr
        curr <- next
    prev