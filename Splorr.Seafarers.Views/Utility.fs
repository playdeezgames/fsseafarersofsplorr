namespace Splorr.Seafarers.Views

module Utility =
    let DumpMessages (sink:MessageSink) (messages:string list) : unit =
        messages
        |> List.iter sink

