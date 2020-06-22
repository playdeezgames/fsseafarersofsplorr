namespace Splorr.Seafarers.Controllers

module ConfirmQuit = 
    let Run (source:CommandSource) (sink:MessageSink) (state:Gamestate) : Gamestate option =
        "" |> sink
        "Are you sure you want to quit?" |> sink

        match source() with
        | Some Help -> 
            state 
            |> ConfirmQuit 
            |> Gamestate.Help 
            |> Some

        | Some Yes -> 
            None

        | Some No -> 
            state 
            |> Some

        | _ -> 
            "Maybe try 'help'?" |> sink
            state 
            |> ConfirmQuit 
            |> Some

