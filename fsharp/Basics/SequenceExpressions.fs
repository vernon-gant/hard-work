// 50.2.1
let fac_seq =
    seq {
        let mutable acc = 1
        yield acc
        for i in 1..System.Int32.MaxValue do
            acc <- acc * i
            yield acc
    }

// 50.2.2
let seq_seq =
    seq {
        yield 0
        for i in 1..System.Int32.MaxValue do
            yield -i
            yield i
    }