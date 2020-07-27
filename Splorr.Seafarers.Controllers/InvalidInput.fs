namespace Splorr.Seafarers.Controllers

module InvalidInput =
    let Run (sink:MessageSink) (gamestate:Gamestate) : Gamestate option =
        (Hue.Error, "Maybe try 'help'?" |> Line) |> Hued |> sink
        gamestate
        |> Some

