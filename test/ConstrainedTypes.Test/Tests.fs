module Tests

open System
open Xunit

type BoundedString10 = ConstrainedTypes.BoundedString<10>

[<Fact>]
let ``ConstrainedString Constructor`` () =
    let b10 = BoundedString10("test")
    printfn "b10: %A" b10
