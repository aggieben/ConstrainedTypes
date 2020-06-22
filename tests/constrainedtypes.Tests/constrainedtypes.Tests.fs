module constrainedtypesTests


open ConstrainedTypes
open NUnit.Framework

type StringOfLength10 = BoundedString<10>

[<Test>]
let ``Default constructor should create instance`` () =
    // Assert.AreEqual("My internal state", MyType().InnerState)
    let test = StringOfLength10("test")
    ()

// [<Test>]
// let ``Constructor with parameter should create instance`` () =
//     Assert.AreEqual("override", MyType("override").InnerState)

// [<Test>]
// let ``Method with ReflectedDefinition parameter should get its name`` () =
//     let myValue = 2
//     Assert.AreEqual("myValue", MyType.NameOf(myValue))

// type Generative2 = constrainedtypes.GenerativeProvider<2>
// type Generative4 = constrainedtypes.GenerativeProvider<4>

// [<Test>]
// let ``Can access properties of generative provider 2`` () =
//     let obj = Generative2()
//     Assert.AreEqual(obj.Property1, 1)
//     Assert.AreEqual(obj.Property2, 2)

// [<Test>]
// let ``Can access properties of generative provider 4`` () =
//     let obj = Generative4()
//     Assert.AreEqual(obj.Property1, 1)
//     Assert.AreEqual(obj.Property2, 2)
//     Assert.AreEqual(obj.Property3, 3)
//     Assert.AreEqual(obj.Property4, 4)

