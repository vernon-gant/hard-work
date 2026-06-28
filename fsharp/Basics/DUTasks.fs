module DU

type F =
    | AM
    | PM

type TimeOfDay = { hours: int; minutes: int; f: F }

let transform (x: TimeOfDay) =
    match x.f with
    | PM -> (x.hours + 12) * 60 + x.minutes
    | AM -> x.hours * 60 + x.minutes

let (.>.) x y = (transform x) > (transform y)