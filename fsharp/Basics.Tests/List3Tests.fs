module Basics.Tests.List3Tests

open List3Tasks
open Xunit
open FsUnit.Xunit

module ListFilterTests =

    [<Fact>]
    let ``empty list returns empty list`` () =
        list_filter (fun x -> x > 0) ([]: int list) |> should equal ([]: int list)

    [<Fact>]
    let ``all elements pass predicate returns same list`` () =
        list_filter (fun x -> x > 0) [ 1; 2; 3 ] |> should equal [ 1; 2; 3 ]

    [<Fact>]
    let ``no elements pass predicate returns empty list`` () =
        list_filter (fun x -> x > 100) [ 1; 2; 3 ] |> should equal ([]: int list)

    [<Fact>]
    let ``mixed predicate filters correctly`` () =
        list_filter (fun x -> x > 0) [ -1; 2; -3; 4; 5 ] |> should equal [ 2; 4; 5 ]

    [<Fact>]
    let ``preserves original order of remaining elements`` () =
        list_filter (fun x -> x % 2 = 0) [ 1; 2; 3; 4; 5; 6 ] |> should equal [ 2; 4; 6 ]

    [<Fact>]
    let ``filtering with always-true predicate keeps everything`` () =
        list_filter (fun _ -> true) [ 1; 2; 3 ] |> should equal [ 1; 2; 3 ]

    [<Fact>]
    let ``filtering with always-false predicate keeps nothing`` () =
        list_filter (fun _ -> false) [ 1; 2; 3 ] |> should equal ([]: int list)

    [<Fact>]
    let ``single element passing predicate`` () =
        list_filter (fun x -> x > 0) [ 5 ] |> should equal [ 5 ]

    [<Fact>]
    let ``single element failing predicate`` () =
        list_filter (fun x -> x > 0) [ -5 ] |> should equal ([]: int list)

    [<Fact>]
    let ``works with strings`` () =
        list_filter (fun (s: string) -> s.Length > 1) [ "a"; "bb"; "c"; "ddd" ]
        |> should equal [ "bb"; "ddd" ]


module SumTests =

    [<Fact>]
    let ``empty list returns 0`` () =
        sum ((fun x -> x > 0), ([]: int list)) |> should equal 0

    [<Fact>]
    let ``all elements satisfy predicate sums everything`` () =
        sum ((fun x -> x > 0), [ 1; 2; 3 ]) |> should equal 6

    [<Fact>]
    let ``no elements satisfy predicate sums to 0`` () =
        sum ((fun x -> x > 100), [ 1; 2; 3 ]) |> should equal 0

    [<Fact>]
    let ``mixed predicate sums only matching elements`` () =
        sum ((fun x -> x % 2 = 0), [ 1; 2; 3; 4; 5; 6 ]) |> should equal 12

    [<Fact>]
    let ``sums negative numbers correctly`` () =
        sum ((fun x -> x < 0), [ -1; 2; -3; 4; -5 ]) |> should equal -9

    [<Fact>]
    let ``single element satisfying predicate`` () =
        sum ((fun x -> x > 0), [ 7 ]) |> should equal 7

    [<Fact>]
    let ``single element not satisfying predicate`` () =
        sum ((fun x -> x > 0), [ -7 ]) |> should equal 0

    [<Fact>]
    let ``predicate matching zero values`` () =
        sum ((fun x -> x = 0), [ 0; 1; 0; 2; 0 ]) |> should equal 0

    [<Fact>]
    let ``always-true predicate equals total sum`` () =
        sum ((fun _ -> true), [ 10; 20; 30 ]) |> should equal 60


module RevrevTests =

    [<Fact>]
    let ``empty list of lists returns empty list`` () =
        revrev ([]: int list list) |> should equal ([]: int list list)

    [<Fact>]
    let ``single empty inner list`` () =
        revrev [ ([]: int list) ] |> should equal [ ([]: int list) ]

    [<Fact>]
    let ``single inner list reverses its elements only`` () =
        revrev [ [ 1; 2; 3 ] ] |> should equal [ [ 3; 2; 1 ] ]

    [<Fact>]
    let ``matches the example from the spec`` () =
        revrev [ [ 1; 2 ]; [ 3; 4; 5 ] ] |> should equal [ [ 5; 4; 3 ]; [ 2; 1 ] ]

    [<Fact>]
    let ``three inner lists reverses outer and inner order`` () =
        revrev [ [ 1; 2 ]; [ 3; 4 ]; [ 5; 6 ] ]
        |> should equal [ [ 6; 5 ]; [ 4; 3 ]; [ 2; 1 ] ]

    [<Fact>]
    let ``inner lists with single elements`` () =
        revrev [ [ 1 ]; [ 2 ]; [ 3 ] ] |> should equal [ [ 3 ]; [ 2 ]; [ 1 ] ]

    [<Fact>]
    let ``mix of empty and non-empty inner lists`` () =
        revrev [ [ 1; 2 ]; ([]: int list); [ 3 ] ]
        |> should equal [ [ 3 ]; ([]: int list); [ 2; 1 ] ]

    [<Fact>]
    let ``works with duplicate values`` () =
        revrev [ [ 1; 1 ]; [ 2; 2 ] ] |> should equal [ [ 2; 2 ]; [ 1; 1 ] ]

    [<Fact>]
    let ``applying revrev twice returns original list`` () =
        let xss = [ [ 1; 2; 3 ]; [ 4; 5 ] ]
        revrev (revrev xss) |> should equal xss