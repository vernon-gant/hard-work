// 42.3
let rec allSubsets n k =
    if k = 0 then
        Set.singleton Set.empty
    elif k > n then
        Set.empty
    else
        let withoutN = allSubsets (n - 1) k
        let withN = allSubsets (n - 1) (k - 1) |> Set.map (Set.add n)
        Set.union withoutN withN