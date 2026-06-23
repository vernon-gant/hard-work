module CompositionTests

open CompositionTasks
open Xunit
open FsUnit.Xunit


module VatTests =

    [<Fact>]
    let ``0%: value is unchanged`` () =
        vat 0 100.0 |> should (equalWithin 1e-9) 100.0

    [<Fact>]
    let ``0%: zero value stays zero`` () =
        vat 0 0.0 |> should (equalWithin 1e-9) 0.0

    [<Fact>]
    let ``100%: value doubles`` () =
        vat 100 100.0 |> should (equalWithin 1e-9) 200.0

    [<Fact>]
    let ``100%: zero value stays zero`` () =
        vat 100 0.0 |> should (equalWithin 1e-9) 0.0

    [<Fact>]
    let ``any rate on zero value always returns zero`` () =
        vat 50 0.0 |> should (equalWithin 1e-9) 0.0

    [<Fact>]
    let ``20% of 100.0 equals 120.0`` () =
        vat 20 100.0 |> should (equalWithin 1e-9) 120.0

    [<Fact>]
    let ``50% of 200.0 equals 300.0`` () =
        vat 50 200.0 |> should (equalWithin 1e-9) 300.0

    [<Fact>]
    let ``5% of 1000.0 equals 1050.0`` () =
        vat 5 1000.0 |> should (equalWithin 1e-9) 1050.0

    [<Fact>]
    let ``1% is the minimum non-zero rate`` () =
        vat 1 100.0 |> should (equalWithin 1e-9) 101.0

    [<Fact>]
    let ``99% is just below the maximum rate`` () =
        vat 99 100.0 |> should (equalWithin 1e-9) 199.0

    [<Fact>]
    let ``10% of 50.0 equals 55.0`` () =
        vat 10 50.0 |> should (equalWithin 1e-6) 55.0

    [<Fact>]
    let ``33% of 99.0 equals approximately 131.67`` () =
        vat 33 99.0 |> should (equalWithin 1e-6) 131.67


module UnvatTests =

    [<Fact>]
    let ``round-trip: n=0 x=100.0`` () =
        unvat 0 (vat 0 100.0) |> should (equalWithin 1e-9) 100.0

    [<Fact>]
    let ``round-trip: n=20 x=100.0`` () =
        unvat 20 (vat 20 100.0) |> should (equalWithin 1e-9) 100.0

    [<Fact>]
    let ``round-trip: n=100 x=100.0`` () =
        unvat 100 (vat 100 100.0) |> should (equalWithin 1e-9) 100.0

    [<Fact>]
    let ``round-trip: n=50 x=200.0`` () =
        unvat 50 (vat 50 200.0) |> should (equalWithin 1e-9) 200.0

    [<Fact>]
    let ``round-trip: n=10 x=50.0 non-round`` () =
        unvat 10 (vat 10 50.0) |> should (equalWithin 1e-6) 50.0

    [<Fact>]
    let ``round-trip: n=1 x=999.0`` () =
        unvat 1 (vat 1 999.0) |> should (equalWithin 1e-6) 999.0

    [<Fact>]
    let ``round-trip: n=99 x=1.0`` () =
        unvat 99 (vat 99 1.0) |> should (equalWithin 1e-6) 1.0

    [<Fact>]
    let ``unvat 20 120.0 equals 100.0`` () =
        unvat 20 120.0 |> should (equalWithin 1e-9) 100.0

    [<Fact>]
    let ``unvat 100 200.0 equals 100.0`` () =
        unvat 100 200.0 |> should (equalWithin 1e-9) 100.0

    [<Fact>]
    let ``unvat on zero value always returns zero`` () =
        unvat 50 0.0 |> should (equalWithin 1e-9) 0.0


module MinTests =


    [<Fact>]
    let ``f always returns 0: answer is 1`` () = min (fun _ -> 0) |> should equal 1

    [<Fact>]
    let ``f(n) = n-1: answer is 1`` () = min (fun n -> n - 1) |> should equal 1

    [<Fact>]
    let ``f(n) = n-3: answer is 3`` () = min (fun n -> n - 3) |> should equal 3

    [<Fact>]
    let ``f(n) = n-5: answer is 5`` () = min (fun n -> n - 5) |> should equal 5

    [<Fact>]
    let ``f(n) = n-10: answer is 10`` () =
        min (fun n -> n - 10) |> should equal 10

    [<Fact>]
    let ``only n=7 gives 0: answer is 7`` () =
        min (fun n -> if n = 7 then 0 else 1) |> should equal 7

    [<Fact>]
    let ``first even positive integer: answer is 2`` () =
        min (fun n -> if n % 2 = 0 then 0 else 1) |> should equal 2

    [<Fact>]
    let ``first positive multiple of 3: answer is 3`` () =
        min (fun n -> if n % 3 = 0 then 0 else 1) |> should equal 3

    [<Fact>]
    let ``first positive multiple of 5: answer is 5`` () =
        min (fun n -> if n % 5 = 0 then 0 else 1) |> should equal 5

    [<Fact>]
    let ``first n greater than 1: answer is 2`` () =
        min (fun n -> if n > 1 then 0 else 1) |> should equal 2

    [<Fact>]
    let ``first n greater than 5: answer is 6`` () =
        min (fun n -> if n > 5 then 0 else 1) |> should equal 6
