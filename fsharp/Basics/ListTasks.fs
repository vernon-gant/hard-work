module ListTasks

// 34.1
let upto n =
    let rec loop i acc =
        if i <= 0 then acc
        else loop (i - 1) (i :: acc)

    loop n []

// 34.2
let rec dnto n =
    if n <= 0 then []
    else n :: dnto (n - 1)

// 34.3
let rec evenn n =
    let rec loop count current =
        if count <= 0 then []
        else current :: loop (count - 1) (current + 2)

    loop n 0

