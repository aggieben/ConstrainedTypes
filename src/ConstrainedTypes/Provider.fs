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

[<TypeProvider>]
type ConstrainedStringProvider (config : TypeProviderConfig) as this =
    inherit TypeProviderForNamespaces (config, addDefaultProbingLocation=true)

    let rootNs = "ConstrainedTypes"
    let thisAssembly = Assembly.GetExecutingAssembly()

    let provider = ProvidedTypeDefinition(thisAssembly, rootNs, "ConstrainedString", Some typeof<string>)

    let compile expression =
        let fsc = FSharpChecker.Create()
        let srcf = Path.ChangeExtension(Path.GetTempFileName(), ".fsx")
        let dllf = Path.ChangeExtension(srcf, ".dll")

        let src =
            sprintf
                """
                module ConstrainedStringExpression
                let expression = %s
                """
                expression

        File.WriteAllText(srcf, src)

        let errors, exitCode, dynAssembly =
            fsc.CompileToDynamicAssembly([|"-o"; dllf; "-a"; srcf|], execute=None)
            |> Async.RunSynchronously

        match dynAssembly with
        | None ->
            failwithf "failed to compile expression '%s': %s" expression errors.[0].Message
        | Some assembly ->
            let funcType =
                assembly.DefinedTypes
                |> Seq.head
                |> (fun m ->
                        m.DeclaredMethods
                        |> Seq.head)

            fun (s:string) -> funcType.Invoke(null, [| s |]) :?> bool

    do
        provider.DefineStaticParameters([ProvidedStaticParameter("Expression", typeof<string>)], fun name args ->
            let expression = args.[0] :?> string
            let constraintFunc = compile expression
            let provided = ProvidedTypeDefinition(thisAssembly, rootNs, name, Some typeof<string>)

            printfn "creating type named: %s.%s" rootNs name

            ProvidedConstructor(
                [ProvidedParameter("value", typeof<string>)],
                fun args ->
                    <@@
                        printfn "evaluating constraint '%s'" expression
                        if not (constraintFunc %%(args.[0])) then
                            sprintf "provided value did not satisfy the constraint '%s'" expression
                            |> invalidArg "value"
                    @@>
            )
            |> provided.AddMember

            provided
        )

        this.AddNamespace(rootNs, [provider])

[<TypeProviderAssembly>]
do ()