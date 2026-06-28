module List3Tasks

// 41.4.1
let list_filter f xs = List.foldBack (fun v acc -> if f v then v :: acc else acc) xs []

// 41.4.2
let sum (p, xs) = List.fold (fun acc v -> if p v then acc + v else acc) 0 xs

// 41.4.3
let revrev xs = List.map List.rev xs |> List.rev 