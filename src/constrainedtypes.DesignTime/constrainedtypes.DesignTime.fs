module constrainedtypesImplementation

open System
open System.Collections.Generic
open System.IO
open System.Reflection
open FSharp.Quotations
open FSharp.Core.CompilerServices
open ProviderImplementation
open ProviderImplementation.ProvidedTypes
open ConstrainedTypes

// Put any utility helpers here
[<AutoOpen>]
module internal Helpers =
    let x = 1

[<TypeProvider>]
type BasicErasingProvider (config : TypeProviderConfig) as this =
    inherit TypeProviderForNamespaces (config, assemblyReplacementMap=[("constrainedtypes.DesignTime", "constrainedtypes.Runtime")], addDefaultProbingLocation=true)

    let ns = "ConstrainedTypes"
    let asm = Assembly.GetExecutingAssembly()

    // check we contain a copy of runtime files, and are not referencing the runtime DLL
    do assert (typeof<DataSource>.Assembly.GetName().Name = asm.GetName().Name)

    let createTypes () =
        let boundedStringProvider = ProvidedTypeDefinition(asm, ns, "BoundedString", Some typeof<obj>)

        boundedStringProvider.DefineStaticParameters([ProvidedStaticParameter("Length", typeof<int>)], fun name args ->
            let length = args.[0] :?> int
            let boundedStringType = ProvidedTypeDefinition(asm, ns, name, Some typeof<string>)

            ProvidedConstructor(
                [ProvidedParameter("value", typeof<string>)],
                fun args ->
                    <@@
                        if (length < String.length %%(args.[0])) then
                            sprintf "provided value exceeds the bounds: '%s' > %d"
                                %%(args.[0]) length
                            |> invalidArg "value"
                    @@>
            )
            |> boundedStringType.AddMember

            boundedStringType
        )

    (*
        let ctor = ProvidedConstructor([], invokeCode = fun args -> <@@ "My internal state" :> obj @@>)
        myType.AddMember(ctor)

        let ctor2 = ProvidedConstructor([ProvidedParameter("InnerState", typeof<string>)], invokeCode = fun args -> <@@ (%%(args.[0]):string) :> obj @@>)
        myType.AddMember(ctor2)

        let innerState = ProvidedProperty("InnerState", typeof<string>, getterCode = fun args -> <@@ (%%(args.[0]) :> obj) :?> string @@>)
        myType.AddMember(innerState)

        let meth = ProvidedMethod("StaticMethod", [], typeof<DataSource>, isStatic=true, invokeCode = (fun args -> Expr.Value(null, typeof<DataSource>)))
        myType.AddMember(meth)

        let nameOf =
            let param = ProvidedParameter("p", typeof<Expr<int>>)
            param.AddCustomAttribute {
                new CustomAttributeData() with
                    member __.Constructor = typeof<ReflectedDefinitionAttribute>.GetConstructor([||])
                    member __.ConstructorArguments = [||] :> _
                    member __.NamedArguments = [||] :> _
            }
            ProvidedMethod("NameOf", [ param ], typeof<string>, isStatic = true, invokeCode = fun args ->
                <@@
                    match (%%args.[0]) : Expr<int> with
                    | Microsoft.FSharp.Quotations.Patterns.ValueWithName (_, _, n) -> n
                    | e -> failwithf "Invalid quotation argument (expected ValueWithName): %A" e
                @@>)
        myType.AddMember(nameOf)
    *)

        [boundedStringProvider]

    do
        this.AddNamespace(ns, createTypes())


[<TypeProviderAssembly>]
do ()
