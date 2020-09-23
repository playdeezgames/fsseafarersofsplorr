namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Services

type SwitchSource = unit -> Set<string>

module ConfirmQuit = 
    let private onStreamSwitch = "on-stream"

    type RunContext =
        inherit OperatingContext
        abstract member switchSource : SwitchSource 
    let Run 
            (context : OperatingContext)
            (source       : CommandSource) 
            (sink         : MessageSink) 
            (state        : Gamestate) 
            : Gamestate option =
        let context = context :?> RunContext
        let switches = context.switchSource()
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

