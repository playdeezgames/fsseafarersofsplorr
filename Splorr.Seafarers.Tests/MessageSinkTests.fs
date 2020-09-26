module MessageSinkTests

open NUnit.Framework
open Splorr.Seafarers
open Splorr.Seafarers.Controllers

[<Test>]
let ``MessageSink.It handles all of the hue values without error`` () =
    (System.Enum.GetValues(typedefof<Hue>)) :?> (Hue array)
    |> List.ofArray
    |> List.map
        (fun hue ->
            (hue, "" |> Text) |> Hued)
    |> List.iter MessageSink.Write
