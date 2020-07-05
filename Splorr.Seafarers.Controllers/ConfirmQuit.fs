namespace Splorr.Seafarers.Controllers

module ConfirmQuit = 
    let Run (source:CommandSource) (sink:MessageSink) (state:Gamestate) : Gamestate option =
        "" |> Line |> sink
        (Heading, "Are you sure you want to quit?" |> Line) |> Hued |> sink

        match source() with
        | Some Command.Help -> 
            state 
            |> Gamestate.ConfirmQuit 
            |> Gamestate.Help 
            |> Some

        | Some Command.Yes -> 
            None

        | Some Command.No -> 
            state 
            |> Some

        | _ -> 
            "Maybe try 'help'?" |> Line |> sink
            state 
            |> Gamestate.ConfirmQuit 
            |> Some

