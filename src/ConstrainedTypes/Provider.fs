namespace ConstrainedTypes

open System.Reflection
open ProviderImplementation.ProvidedTypes
open FSharp.Core.CompilerServices

[<AutoOpen>]
module BoundedStringUtilities =
    let ensureBoundedString length value =
        if length < String.length value then
            sprintf "provided value exceeded the bounds: '%s' > %d"
                value length
            |> invalidArg "value"
        else value

[<TypeProvider>]
type ConstrainedStringProvider (config : TypeProviderConfig) as this =
    inherit TypeProviderForNamespaces (config, addDefaultProbingLocation=true)

    let ns = "ConstrainedTypes"
    let asm = Assembly.GetExecutingAssembly()

    let boundedStringProvider = ProvidedTypeDefinition(asm, ns, "BoundedString", Some typeof<string>)

    do
        boundedStringProvider.DefineStaticParameters([ProvidedStaticParameter("Length", typeof<int>)], fun name args ->
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

        this.AddNamespace(ns, [boundedStringProvider])

[<TypeProviderAssembly>]
do ()