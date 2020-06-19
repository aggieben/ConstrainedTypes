module Tests

open System
open Xunit

type NonEmptyString = ConstrainedTypes.ConstrainedString<"fun (s:string) -> not (System.String.IsNullOrWhiteSpace(s))">

[<Fact>]
let ``ConstrainedString Constructor`` () =
    NonEmptyString("b")
    Assert.True(true)
