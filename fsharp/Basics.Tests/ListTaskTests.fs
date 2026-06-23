module Basics.Tests.ListTaskTests

open Xunit
open FsUnit.Xunit
open ListTasks

module UptoTests =

    [<Fact>]
    let ``upto 0 returns empty list`` () =
        upto 0 |> should equal ([] : int list)

    [<Fact>]
    let ``upto 1 returns list with one element`` () =
        upto 1 |> should equal [1]

    [<Fact>]
    let ``upto 2 returns 1 to 2`` () =
        upto 2 |> should equal [1; 2]

    [<Fact>]
    let ``upto 5 returns 1 to 5`` () =
        upto 5 |> should equal [1; 2; 3; 4; 5]

    [<Fact>]
    let ``upto 10 returns 1 to 10`` () =
        upto 10 |> should equal [1; 2; 3; 4; 5; 6; 7; 8; 9; 10]

    [<Fact>]
    let ``upto 20 has length 20`` () =
        upto 20 |> should haveLength 20

    [<Fact>]
    let ``upto 20 starts with 1`` () =
        upto 20 |> List.head |> should equal 1

    [<Fact>]
    let ``upto 20 ends with 20`` () =
        upto 20 |> List.last |> should equal 20


module DntoTests =

    [<Fact>]
    let ``dnto 0 returns empty list`` () =
        dnto 0 |> should equal ([] : int list)

    [<Fact>]
    let ``dnto 1 returns list with one element`` () =
        dnto 1 |> should equal [1]

    [<Fact>]
    let ``dnto 2 returns 2 to 1`` () =
        dnto 2 |> should equal [2; 1]

    [<Fact>]
    let ``dnto 5 returns 5 to 1`` () =
        dnto 5 |> should equal [5; 4; 3; 2; 1]

    [<Fact>]
    let ``dnto 10 returns 10 to 1`` () =
        dnto 10 |> should equal [10; 9; 8; 7; 6; 5; 4; 3; 2; 1]

    [<Fact>]
    let ``dnto 20 has length 20`` () =
        dnto 20 |> should haveLength 20

    [<Fact>]
    let ``dnto 20 starts with 20`` () =
        dnto 20 |> List.head |> should equal 20

    [<Fact>]
    let ``dnto 20 ends with 1`` () =
        dnto 20 |> List.last |> should equal 1


module EvennTests =

    [<Fact>]
    let ``evenn 0 returns empty list`` () =
        evenn 0 |> should equal ([] : int list)

    [<Fact>]
    let ``evenn 1 returns first non-negative even number`` () =
        evenn 1 |> should equal [0]

    [<Fact>]
    let ``evenn 2 returns first two non-negative even numbers`` () =
        evenn 2 |> should equal [0; 2]

    [<Fact>]
    let ``evenn 3 returns first three non-negative even numbers`` () =
        evenn 3 |> should equal [0; 2; 4]

    [<Fact>]
    let ``evenn 5 returns first five non-negative even numbers`` () =
        evenn 5 |> should equal [0; 2; 4; 6; 8]

    [<Fact>]
    let ``evenn 10 returns first ten non-negative even numbers`` () =
        evenn 10 |> should equal [0; 2; 4; 6; 8; 10; 12; 14; 16; 18]

    [<Fact>]
    let ``evenn 20 has length 20`` () =
        evenn 20 |> should haveLength 20

    [<Fact>]
    let ``evenn 20 starts with 0`` () =
        evenn 20 |> List.head |> should equal 0

    [<Fact>]
    let ``evenn 20 ends with 38`` () =
        evenn 20 |> List.last |> should equal 38

    [<Fact>]
    let ``all numbers produced by evenn 20 are even`` () =
        evenn 20 |> List.forall (fun x -> x % 2 = 0) |> should equal true

    [<Fact>]
    let ``all numbers produced by evenn 20 are non-negative`` () =
        evenn 20 |> List.forall (fun x -> x >= 0) |> should equal true