﻿namespace Splorr.Seafarers.Controllers

module Utility =
    let DumpMessages (sink:MessageSink) (messages:string list) : unit =
        messages
        |> List.iter sink
