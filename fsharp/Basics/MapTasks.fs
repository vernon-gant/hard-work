// 43.3
let try_find key m =
    let entries = Map.toArray m

    let rec search low high =
        if low > high then
            None
        else
            let mid = (low + high) / 2
            let (entryKey, value) = entries.[mid]

            match compare key entryKey with
            | 0 -> Some value
            | c when c < 0 -> search low (mid - 1)
            | _ -> search (mid + 1) high

    search 0 (entries.Length - 1)