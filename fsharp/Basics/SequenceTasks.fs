// 49.5.1
let even_seq = Seq.initInfinite (fun i -> (i + 1) * 2)

// 49.5.2
let fac_seq = Seq.initInfinite (fun i ->
    let rec factorial n = if n = 0 then 1 else n * factorial (n - 1)
    factorial i)

// 49.5.3
let seq_seq = Seq.initInfinite (fun i ->
    if i = 0 then 0
    elif i % 2 = 1 then -(i / 2 + 1)
    else i / 2)