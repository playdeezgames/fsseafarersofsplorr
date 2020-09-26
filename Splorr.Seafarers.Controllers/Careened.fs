namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module Careened = 
    let private UpdateDisplay 
            (context : ServiceContext)
            (messageSink                 : MessageSink) 
            (side                        : Side)
            (avatarId                    : string) 
            : unit =
        let avatarId = avatarId
        "" |> Line |> messageSink
        avatarId
        |> Avatar.GetMessages context
        |> Utility.DumpMessages messageSink
        let sideName =
            match side with
            | Port -> "port"
            | Starboard -> "starboard"
        let currentValue =
            Avatar.GetCurrentFouling context avatarId
        let maximumValue = 
            Avatar.GetMaximumFouling context avatarId
        let foulage =
            100.0 * currentValue / maximumValue
        [
            (Hue.Heading, sideName |> sprintf "You are careened on the %s side." |> Line) |> Hued
            (Hue.Flavor, foulage |> sprintf "The hull is %.0f%% fouled." |> Line) |> Hued
        ]
        |> List.iter messageSink

    let private HandleCommand
            (context : ServiceContext)
            (command                       : Command option) 
            (side                          : Side) 
            (avatarId                      : string) 
            : Gamestate option =
        avatarId
        |> World.ClearMessages context
        match command with
        | Some Command.Metrics ->
            (side, avatarId)
            |> Gamestate.Careened
            |> Gamestate.Metrics
            |> Some
        | Some Command.Inventory ->
            (side, avatarId)
            |> Gamestate.Careened
            |> Gamestate.Inventory
            |> Some
        | Some Command.Status ->
            (side, avatarId)
            |> Gamestate.Careened
            |> Gamestate.Status
            |> Some
        | Some Command.Quit ->
            (side, avatarId)
            |> Gamestate.Careened
            |> Gamestate.ConfirmQuit
            |> Some
        | Some Command.Help ->
            (side, avatarId)
            |> Gamestate.Careened
            |> Gamestate.Help
            |> Some
        | Some Command.CleanHull ->
            avatarId 
            |> World.CleanHull
                context
                (if side=Port then Starboard else Port)
            (side, avatarId)
            |> Gamestate.Careened
            |> Some
        | Some Command.WeighAnchor ->
            avatarId
            |> Gamestate.InPlay
            |> Some
        | _ ->
            ("Maybe try 'help'?",(side, avatarId)
            |> Gamestate.Careened)
            |> Gamestate.ErrorMessage
            |> Some

    let private RunAlive
            (context : ServiceContext)
            (source                        : CommandSource) 
            (sink                          : MessageSink) 
            (side                          : Side) 
            (avatarId                      : string) 
            : Gamestate option =
        UpdateDisplay 
            context
            sink 
            side 
            avatarId
        HandleCommand
            context
            (source())
            side
            avatarId

    let Run 
            (context : ServiceContext)
            (commandSource                 : CommandSource) 
            (messageSink                   : MessageSink) 
            (side                          : Side) 
            (avatarId                      : string) 
            : Gamestate option =
        if avatarId |> World.IsAvatarAlive context then
            RunAlive 
                context
                commandSource 
                messageSink 
                side 
                avatarId
        else
            avatarId
            |> Avatar.GetMessages context
            |> Gamestate.GameOver
            |> Some