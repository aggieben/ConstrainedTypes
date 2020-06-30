namespace ConstrainedTypes

open System.Reflection
open ProviderImplementation.ProvidedTypes
open FSharp.Core.CompilerServices

[<AutoOpen>]
module Utilities =
    let ensureBoundedString length value =
        if length < String.length value then
            sprintf "provided value exceeded the bounds: '%s' > %d"
                value length
            |> invalidArg "value"
        else value

    let ensureRangedInteger min max value =
        if value < min || value > max then
            sprintf "provided value falls outside the range: '%d' ∉ [%d,%d]"
                value min max
            |> invalidArg "value"
        else value

[<TypeProvider>]
type ConstrainedTypesProvider (config : TypeProviderConfig) as this =
    inherit TypeProviderForNamespaces (config, addDefaultProbingLocation=true)

    let ns = "ConstrainedTypes"
    let asm = Assembly.GetExecutingAssembly()

    let boundedStringProvider = ProvidedTypeDefinition(asm, ns, "BoundedString", Some typeof<string>)
    let rangedIntegerProvider = ProvidedTypeDefinition(asm, ns, "RangedInt", Some typeof<int>)

    do
        boundedStringProvider.DefineStaticParameters(
            [ProvidedStaticParameter("Length", typeof<int>)],
            fun name args ->
                let length = args.[0] :?> int
                let provided = ProvidedTypeDefinition(asm, ns, name, Some typeof<string>)

                ProvidedConstructor(
                    [ProvidedParameter("value", typeof<string>)],
                    fun args ->
                        <@@
                            let value = %%args.[0]
                            ensureBoundedString length value
                        @@>
                )
                |> provided.AddMember

                provided
            )

        rangedIntegerProvider.DefineStaticParameters(
            [ProvidedStaticParameter("Min", typeof<int>); ProvidedStaticParameter("Max", typeof<int>)],
            fun name args ->
                let min = args.[0] :?> int
                let max = args.[1] :?> int
                let provided = ProvidedTypeDefinition(asm, ns, name, Some typeof<int>)

                ProvidedConstructor(
                    [ProvidedParameter("value", typeof<int>)],
                    fun args ->
                        <@@
                            let value = %%args.[0]
                            ensureRangedInteger min max value
                        @@>
                )
                |> provided.AddMember

                provided
            )

        this.AddNamespace(ns, [boundedStringProvider; rangedIntegerProvider])

[<TypeProviderAssembly>]
do ()