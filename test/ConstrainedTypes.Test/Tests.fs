module Tests

open System
open Swensen.Unquote
open Xunit

type BoundedString10 = ConstrainedTypes.BoundedString<10>
type RangedInteger10 = ConstrainedTypes.RangedInt<0,10>

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


[<Trait("ProvidedType", "RangedInteger")>]
[<Theory>]
[<InlineData(7, false)>]
[<InlineData(-1, true)>]
[<InlineData(0, false)>]
[<InlineData(10, false)>]
[<InlineData(11, true)>]
let ``RangedInteger constructor correctly validates`` value throws =
    if throws then
        raises<ArgumentException> <@ RangedInteger10(value) @>
    else
        RangedInteger10(value) |> ignore

[<Trait("ProvidedType", "RangedInteger")>]
[<Fact>]
let ``RangedInteger is an int``() =
    let riType = RangedInteger10(5).GetType()
    test <@ riType = typeof<int> @>

[<Fact>]
let ``RangedInteger conversion to int``() =
    let ri = RangedInteger10(5)
    test <@ int ri = 5 @>