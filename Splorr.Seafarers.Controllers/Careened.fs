namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

type CareenedHandleCommandContext =
    inherit World.ClearMessagesContext

type CareenedUpdateDisplayContext = 
    inherit Avatar.GetCurrentFoulingContext
    inherit Avatar.GetMaximumFoulingContext

type CareenedRunAliveContext =
    inherit CareenedHandleCommandContext
    inherit CareenedUpdateDisplayContext

type CareenedRunContext =
    inherit CareenedRunAliveContext
    abstract member avatarMessageSource           : AvatarMessageSource


module Careened = 
    let private UpdateDisplay 
            (context : CareenedUpdateDisplayContext)
            (avatarMessageSource         : AvatarMessageSource)
            (messageSink                 : MessageSink) 
            (side                        : Side)
            (avatarId                    : string) 
            : unit =
        let avatarId = avatarId
        "" |> Line |> messageSink
        avatarId
        |> avatarMessageSource
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
            (context : CareenedHandleCommandContext)
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
            (context : CareenedRunAliveContext)
            (avatarMessageSource           : AvatarMessageSource)
            (source                        : CommandSource) 
            (sink                          : MessageSink) 
            (side                          : Side) 
            (avatarId                      : string) 
            : Gamestate option =
        UpdateDisplay 
            context
            avatarMessageSource
            sink 
            side 
            avatarId
        HandleCommand
            context
            (source())
            side
            avatarId

    let Run 
            (context : OperatingContext)
            (commandSource                 : CommandSource) 
            (messageSink                   : MessageSink) 
            (side                          : Side) 
            (avatarId                      : string) 
            : Gamestate option =
        let context = context :?> CareenedRunContext
        if avatarId |> World.IsAvatarAlive context then
            RunAlive 
                context
                context.avatarMessageSource
                commandSource 
                messageSink 
                side 
                avatarId
        else
            avatarId
            |> context.avatarMessageSource
            |> Gamestate.GameOver
            |> Some