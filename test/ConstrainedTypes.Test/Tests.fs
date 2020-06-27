module Tests

open System
open Swensen.Unquote
open Xunit

type BoundedString10 = ConstrainedTypes.BoundedString<10>

[<Theory>]
[<Trait("ProvidedType", "BoundedString")>]
[<InlineData("test", false)>]
[<InlineData("this is a longer test", true)>]
let ``BoundedString Constructor correctly validates`` value throws =
    if throws then
        raises<ArgumentException> <@ BoundedString10(value) @>
    else
        BoundedString10(value) |> ignore


[<Fact>]
[<Trait("ProvidedType", "BoundedString")>]
let ``BoundedString conversion to string`` () =
    let bs = BoundedString10("test")
    test <@ string bs = "test" @>

[<Trait("ProvidedType", "BoundedString")>]
[<Fact>]
let ``BoundedString is a string`` () =
    let bs = BoundedString10("test")
    test <@ bs.GetType() = typeof<string> @>