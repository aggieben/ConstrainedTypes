module Tests

open System
open Xunit

open ComboProvider

type T = MyType

[<Fact>]
let ``My test`` () =
    Assert.True(true)
