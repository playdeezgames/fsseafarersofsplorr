namespace Splorr.Seafarers.Controllers

module ErrorMessage =
    let Run 
            (messageSink : MessageSink) 
            (message     : string) 
            (gamestate   : Gamestate) 
            : Gamestate option =
        (Hue.Error, message |> Line) |> Hued |> messageSink
        gamestate
        |> Some

