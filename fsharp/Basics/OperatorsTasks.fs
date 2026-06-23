module OperatorsTasks

// 23.4.1
let toCopper (g, s, c) = g * 240 + s * 12 + c

let fromCopper copperTotal =
    let copper = copperTotal % 12
    let total = copperTotal / 12
    let silver = total % 20
    let gold = total / 20
    (gold, silver, copper)

let (.+.) x y = fromCopper (toCopper x + toCopper y)
let (.-.) x y = fromCopper (toCopper x - toCopper y)


// 23.4.2
let (.+) (a: float, b: float) (c: float, d: float) = (a + c, b + d)

let (.-) (a: float, b: float) (c: float, d: float) = (a - c, b - d)

let (.*) (a: float, b: float) (c: float, d: float) = (a * c - b * d, b * c + a * d)

let (./) (a: float, b: float) (c: float, d: float) =
    let denominator = c * c + d * d
    ((a * c + b * d) / denominator, (b * c - a * d) / denominator)
