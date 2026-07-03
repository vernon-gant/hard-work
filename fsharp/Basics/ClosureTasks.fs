module ClosureTasks

let curry f = fun x y -> f (x, y)

let uncurry f = fun (x, y) -> f x y