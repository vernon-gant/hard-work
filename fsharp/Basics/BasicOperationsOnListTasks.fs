module BasicOperationsOnListTasks

// 39.1
let rec rmodd x =
    let indices = [ 1 .. 2 .. (List.length x) - 1 ]
    List.map (fun i -> List.item i x) indices

// 39.2
let rec del_even x =
    List.filter (fun i -> i % 2 <> 0) x

// 39.3
let rec multiplicity x xs =
    List.fold (fun acc i -> if i = x then acc + 1 else acc) 0 xs

// 39.4
let rec split x =
    let odd = [ 0 .. 2 .. (List.length x) - 1 ]
    let even = [ 1 .. 2 .. (List.length x) - 1 ]
    (List.map (fun i -> List.item i x) odd, List.map (fun i -> List.item i x) even)

// 39.5
let rec zip (xs1,xs2) =
    let rec zipRec (xs1,xs2) =
        match (xs1, xs2) with
        | [], [] -> []
        | h1 :: t1, h2 :: t2 -> (h1, h2) :: zipRec (t1, t2)
        | _ -> []

    if List.length xs1 <> List.length xs2 then
        failwith "Lists must have the same length"
    else
        zipRec (xs1, xs2)