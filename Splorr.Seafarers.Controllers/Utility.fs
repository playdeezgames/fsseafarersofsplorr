﻿namespace Splorr.Seafarers.Controllers

module Utility =
    let DumpMessages (messageSink:MessageSink) (messages:string list) : unit =
        messages
        |> List.map (fun x-> (Hue.Flavor, x|> Line) |> Hued)
        |> List.iter messageSink

