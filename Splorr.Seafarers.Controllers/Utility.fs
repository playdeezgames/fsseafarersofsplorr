namespace Splorr.Seafarers.Controllers

module Utility =
    let Lesser 
            (a:uint32) 
            (b:uint32) 
            : uint32 = 
        if a<b then a else b

    let DumpMessages 
            (messageSink:MessageSink) 
            (messages:string list) 
            : unit =
        messages
        |> List.map (fun x-> (Hue.Flavor, x|> Line) |> Hued)
        |> List.iter messageSink

