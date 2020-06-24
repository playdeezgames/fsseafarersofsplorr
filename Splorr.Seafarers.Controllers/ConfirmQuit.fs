namespace Splorr.Seafarers.Controllers

module ConfirmQuit = 
    let Run (source:CommandSource) (sink:MessageSink) (state:Gamestate) : Gamestate option =
        "" |> sink
        "Are you sure you want to quit?" |> sink

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
            "Maybe try 'help'?" |> sink
            state 
            |> Gamestate.ConfirmQuit 
            |> Some

