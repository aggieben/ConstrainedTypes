#r "nuget: FSharp.Compiler.Service"
#r "./bin/Debug/netstandard2.1/ConstrainedTypes.dll"


open ConstrainedTypes
type T = ConstrainedString<"fun (s:string) -> not (System.String.IsNullOrWhiteSpace(s))">

printfn ""

