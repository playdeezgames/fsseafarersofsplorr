namespace Splorr.Seafarers.Controllers

module ConfirmQuit = 
    let private onStreamSwitch = "on-stream"

    let Run 
            (switches : Set<string>) 
            (source   : CommandSource) 
            (sink     : MessageSink) 
            (state    : Gamestate) 
            : Gamestate option =
        if switches.Contains onStreamSwitch then
            [
                "" |> Line
                (Hue.Error, "(nice try!)" |> Line) |> Hued
            ]
            |> List.iter sink
            state
            |> Some
        else
            "" |> Line |> sink
            (Hue.Heading, "Are you sure you want to quit?" |> Line) |> Hued |> sink

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

