module Basics.ListTasks2

// 40.1
let rec sum (p, xs) =
    match xs with
    | [] -> 0
    | h :: t -> if p h then h + sum (p, t) else sum (p, t)

// 40.2.1
let rec count (xs, n) =
    match xs with
    | [] -> 0
    | h :: t ->
        match h with
        | x when x < n -> count (t, n)
        | x when x = n -> 1 + count (t, n)
        | _ -> 0

// 40.2.2
let rec insert (xs, n) =
    match xs with
    | [] -> [ n ]
    | h :: t -> if h < n then h :: insert (t, n) else n :: h :: t

// 40.2.3
let rec intersect (xs1, xs2) =
    match xs1, xs2 with
    | [], _ -> []
    | _, [] -> []
    | h1 :: t1, h2 :: t2 ->
        if h1 < h2 then intersect (t1, xs2)
        elif h1 > h2 then intersect (xs1, t2)
        else h1 :: intersect (t1, t2)

// 40.2.4
let rec plus (xs1, xs2) =
    match xs1, xs2 with
    | [], ys -> ys
    | xs, [] -> xs
    | h1 :: t1, h2 :: t2 ->
        if h1 <= h2 then h1 :: plus (t1, xs2)
        else h2 :: plus (xs1, t2)

// 40.2.5
let rec minus (xs1, xs2) =
    match xs1, xs2 with
    | [], _ -> []
    | xs, [] -> xs
    | h1 :: t1, h2 :: t2 ->
        if h1 < h2 then h1 :: minus (t1, xs2)
        elif h1 > h2 then minus (xs1, t2)
        else minus (t1, t2)

// 40.3.1
let rec smallest xs =
    match xs with
    | [] -> None
    | [ x ] -> Some x
    | h :: t ->
        match smallest t with
        | None -> Some h
        | Some m -> if h < m then Some h else Some m

// 40.3.2
let rec delete (n, xs) =
    match xs with
    | [] -> []
    | h :: t -> if h = n then t else h :: delete (n, t)

// 40.3.3
let rec sort xs =
    match smallest xs with
    | None -> []
    | Some m -> m :: sort (delete (m, xs))

// 40.4
let rec revrev xss =
    match xss with
    | [] -> []
    | h :: t -> revrev t @ [ List.rev h ]