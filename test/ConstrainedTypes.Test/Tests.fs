module Tests

open System
open Xunit

type BoundedString10 = ConstrainedTypes.BoundedString<10>

[<Fact>]
let ``ConstrainedString Constructor`` () =
    BoundedString10("test")
    Assert.True(true)
