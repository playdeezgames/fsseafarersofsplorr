namespace Splorr.Seafarers.Controllers

module GameOver =
    let Run 
            (messageSink:MessageSink) 
            (messages:string list)
            : Gamestate option =
        "" |> Line |> messageSink
        messages |> Utility.DumpMessages messageSink
        None
        |> Gamestate.MainMenu
        |> Some

