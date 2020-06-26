namespace ConstrainedTypes

open System.IO
open System.Reflection
open FSharp.Quotations
open FSharp.Core.CompilerServices
open ProviderImplementation
open ProviderImplementation.ProvidedTypes
open FSharp.Quotations
open Microsoft.FSharp.Quotations
open FSharp.Compiler.SourceCodeServices

module internal BoundedStringUtilities =
    let ensureBoundedString length value =
        if length < String.length value then
            sprintf "provided value exceeded the bounds: '%s' > %d"
                value length
            |> invalidArg "value"

[<TypeProvider>]
type ConstrainedStringProvider (config : TypeProviderConfig) as this =
    inherit TypeProviderForNamespaces (config, addDefaultProbingLocation=true)

    let rootNs = "ConstrainedTypes"
    let thisAssembly = Assembly.GetExecutingAssembly()

    let boundedStringProvider = ProvidedTypeDefinition(thisAssembly, rootNs, "BoundedString", Some typeof<string>)

    do
        boundedStringProvider.DefineStaticParameters([ProvidedStaticParameter("Length", typeof<int>)], fun name args ->
            let length = args.[0] :?> int
            let provided = ProvidedTypeDefinition(thisAssembly, rootNs, name, Some typeof<string>)

            ProvidedConstructor(
                [ProvidedParameter("value", typeof<string>)],
                fun args ->
                    <@@
                        BoundedStringUtilities.ensureBoundedString length %%args.[0]
                    @@>
            )
            |> provided.AddMember

            provided
        )

        this.AddNamespace(rootNs, [boundedStringProvider])

[<TypeProviderAssembly>]
do ()