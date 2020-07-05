namespace Splorr.Seafarers.Controllers

module Utility =
    let DumpMessages (sink:MessageSink) (messages:string list) : unit =
        messages
        |> List.map (fun x-> (Flavor, x|> Line) |> Hued)
        |> List.iter sink

