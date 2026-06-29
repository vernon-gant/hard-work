// 43.3
open System

let try_find key m =
    let mapArray = Map.toArray m 
    match Array.BinarySearch(Array.map fst mapArray, key) with
    | n when n < 0 -> None
    | n -> Some (snd mapArray[n])