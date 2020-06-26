namespace ConstrainedTypes

open System

// Put any utilities here
[<AutoOpen>]
module internal Utilities =

    let ensureBoundedString length value =
        if length < String.length value then
            sprintf "provided value exceeds the bounds: '%s' > %d"
                value length
            |> invalidArg "value"

// Put any runtime constructs here
type DataSource(filename:string) =
    member this.FileName = filename


// Put the TypeProviderAssemblyAttribute in the runtime DLL, pointing to the design-time DLL
[<assembly:CompilerServices.TypeProviderAssembly("constrainedtypes.DesignTime.dll")>]
do ()
