#r "nuget: FSharp.Compiler.Service"

open System.IO
open FSharp.Compiler.SourceCodeServices
open FSharp.Reflection

let fsc = FSharpChecker.Create()
let srcfile = Path.ChangeExtension(Path.GetTempFileName(), ".fsx")
let dllfile = Path.ChangeExtension(srcfile, ".dll")

let src =
    sprintf """
        module ConstrainedStringExpression
        let expression = %s
        """ "fun (s:string) -> !string.IsNullOrWhitespace(s)"

File.WriteAllText(srcfile, src)

let errors, exitCode, assemblyOption =
    fsc.CompileToDynamicAssembly([|"-o"; dllfile; "-a"; srcfile|], execute=None)
    |> Async.RunSynchronously