module BasicOperationsOnListTests

open BasicOperationsOnListTasks
open Xunit
open FsUnit.Xunit


module RmoddTests =

    [<Fact>]
    let ``empty list returns empty list`` () =
        rmodd ([]: int list) |> should equal ([]: int list)

    [<Fact>]
    let ``single element list returns empty (index 0 is even)`` () =
        rmodd [ 1 ] |> should equal ([]: int list)

    [<Fact>]
    let ``two elements returns the second (index 1)`` () =
        rmodd [ 10; 20 ] |> should equal [ 20 ]

    [<Fact>]
    let ``three elements returns only index 1`` () =
        rmodd [ 1; 2; 3 ] |> should equal [ 2 ]

    [<Fact>]
    let ``five elements returns odd-indexed values`` () =
        rmodd [ 1; 2; 3; 4; 5 ] |> should equal [ 2; 4 ]

    [<Fact>]
    let ``six elements returns odd-indexed values`` () =
        rmodd [ 1; 2; 3; 4; 5; 6 ] |> should equal [ 2; 4; 6 ]

    [<Fact>]
    let ``works with negative numbers`` () =
        rmodd [ -1; -2; -3; -4 ] |> should equal [ -2; -4 ]

    [<Fact>]
    let ``works with duplicate values`` () =
        rmodd [ 7; 7; 7; 7; 7 ] |> should equal [ 7; 7 ]

    [<Fact>]
    let ``result length is n/2 (integer division)`` () =
        rmodd [ 1; 2; 3; 4; 5; 6; 7 ] |> List.length |> should equal 3


module DelEvenTests =

    [<Fact>]
    let ``empty list returns empty list`` () =
        del_even ([]: int list) |> should equal ([]: int list) 

    [<Fact>]
    let ``all odd values are kept`` () =
        del_even [ 1; 3; 5; 7 ] |> should equal [ 1; 3; 5; 7 ]

    [<Fact>]
    let ``all even values are removed`` () =
        del_even [ 2; 4; 6; 8 ] |> should equal ([]: int list)

    [<Fact>]
    let ``mixed values keeps only odd ones`` () =
        del_even [ 1; 2; 3; 4; 5; 6 ] |> should equal [ 1; 3; 5 ]

    [<Fact>]
    let ``zero is removed as an even value`` () =
        del_even [ 0; 1; 2; 3 ] |> should equal [ 1; 3 ]

    [<Fact>]
    let ``negative even values are removed`` () =
        del_even [ -4; -3; -2; -1 ] |> should equal [ -3; -1 ]

    [<Fact>]
    let ``preserves original order of remaining elements`` () =
        del_even [ 9; 2; 7; 4; 5 ] |> should equal [ 9; 7; 5 ]

    [<Fact>]
    let ``single odd element list returns itself`` () =
        del_even [ 11 ] |> should equal [ 11 ]

    [<Fact>]
    let ``single even element list returns empty`` () =
        del_even [ 12 ] |> should equal ([]: int list)


module MultiplicityTests =

    [<Fact>]
    let ``value not in empty list has multiplicity 0`` () =
        multiplicity 5 ([]: int list) |> should equal 0

    [<Fact>]
    let ``value not present in list has multiplicity 0`` () =
        multiplicity 9 [ 1; 2; 3 ] |> should equal 0

    [<Fact>]
    let ``value appearing once has multiplicity 1`` () =
        multiplicity 2 [ 1; 2; 3 ] |> should equal 1

    [<Fact>]
    let ``value appearing multiple times is counted correctly`` () =
        multiplicity 3 [ 3; 1; 3; 3; 2 ] |> should equal 3

    [<Fact>]
    let ``value appearing in all elements`` () =
        multiplicity 7 [ 7; 7; 7; 7 ] |> should equal 4

    [<Fact>]
    let ``works with negative numbers`` () =
        multiplicity -1 [ -1; 2; -1; -1; 3 ] |> should equal 3

    [<Fact>]
    let ``counts zero correctly`` () =
        multiplicity 0 [ 0; 1; 0; 2; 0 ] |> should equal 3

    [<Fact>]
    let ``single element list matching x`` () =
        multiplicity 4 [ 4 ] |> should equal 1

    [<Fact>]
    let ``single element list not matching x`` () =
        multiplicity 4 [ 5 ] |> should equal 0


module SplitTests =

    [<Fact>]
    let ``empty list splits into two empty lists`` () =
        split ([]: int list) |> should equal (([]: int list), ([]: int list))

    [<Fact>]
    let ``single element goes to first list`` () =
        split [ 1 ] |> should equal ([ 1 ], ([]: int list))

    [<Fact>]
    let ``two elements split one each`` () =
        split [ 1; 2 ] |> should equal ([ 1 ], [ 2 ])

    [<Fact>]
    let ``odd-length list, extra element goes to first list`` () =
        split [ 1; 2; 3; 4; 5 ] |> should equal ([ 1; 3; 5 ], [ 2; 4 ])

    [<Fact>]
    let ``even-length list splits evenly`` () =
        split [ 1; 2; 3; 4; 5; 6 ] |> should equal ([ 1; 3; 5 ], [ 2; 4; 6 ])

    [<Fact>]
    let ``works with duplicate values`` () =
        split [ 9; 9; 9; 9 ] |> should equal ([ 9; 9 ], [ 9; 9 ])

    [<Fact>]
    let ``recombining both lists covers all original elements`` () =
        let xs = [ 1; 2; 3; 4; 5; 6; 7 ]
        let (odds, evens) = split xs
        (List.length odds + List.length evens) |> should equal (List.length xs)


module ZipTests =

    [<Fact>]
    let ``zipping two empty lists returns empty list`` () =
        zip (([]: int list), ([]: string list)) |> should equal ([]: (int * string) list)

    [<Fact>]
    let ``zipping single-element lists`` () =
        zip ([ 1 ], [ "a" ]) |> should equal [ (1, "a") ]

    [<Fact>]
    let ``zipping equal-length lists pairs elements in order`` () =
        zip ([ 1; 2; 3 ], [ "a"; "b"; "c" ])
        |> should equal [ (1, "a"); (2, "b"); (3, "c") ]

    [<Fact>]
    let ``zipping lists of same type`` () =
        zip ([ 1; 2 ], [ 10; 20 ]) |> should equal [ (1, 10); (2, 20) ]

    [<Fact>]
    let ``result length equals input length when lengths match`` () =
        zip ([ 1; 2; 3; 4 ], [ 5; 6; 7; 8 ]) |> List.length |> should equal 4

    [<Fact>]
    let ``mismatched lengths raise an exception (first longer)`` () =
        (fun () -> zip ([ 1; 2; 3 ], [ 1; 2 ]) |> ignore)
        |> should throw typeof<System.Exception>

    [<Fact>]
    let ``mismatched lengths raise an exception (second longer)`` () =
        (fun () -> zip ([ 1; 2 ], [ 1; 2; 3 ]) |> ignore)
        |> should throw typeof<System.Exception>

    [<Fact>]
    let ``one empty one non-empty raises an exception`` () =
        (fun () -> zip (([]: int list), [ 1 ]) |> ignore)
        |> should throw typeof<System.Exception>

    [<Fact>]
    let ``equal-length lists do not raise`` () =
        (fun () -> zip ([ 1; 2 ], [ 3; 4 ]) |> ignore) |> should not' (throw typeof<System.Exception>)