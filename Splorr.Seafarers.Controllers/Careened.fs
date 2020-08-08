namespace Splorr.Seafarers.Controllers

open System
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module Careened = 
    let private UpdateDisplay 
            (vesselSingleStatisticSource : string -> VesselStatisticIdentifier -> Statistic option)
            (avatarMessageSource         : AvatarMessageSource)
            (messageSink                 : MessageSink) 
            (side                        : Side)
            (world                       : World) 
            : unit =
        let avatarId = world.AvatarId
        "" |> Line |> messageSink
        avatarId
        |> avatarMessageSource
        |> Utility.DumpMessages messageSink
        let sideName =
            match side with
            | Port -> "port"
            | Starboard -> "starboard"
        let currentValue =
            Avatar.GetCurrentFouling vesselSingleStatisticSource avatarId
        let maximumValue = 
            Avatar.GetMaximumFouling vesselSingleStatisticSource avatarId
        let foulage =
            100.0 * currentValue / maximumValue
        [
            (Hue.Heading, sideName |> sprintf "You are careened on the %s side." |> Line) |> Hued
            (Hue.Flavor, foulage |> sprintf "The hull is %.0f%% fouled." |> Line) |> Hued
        ]
        |> List.iter messageSink

    let private HandleCommand 
            (vesselSingleStatisticSource : string -> VesselStatisticIdentifier -> Statistic option)
            (vesselSingleStatisticSink   : string -> VesselStatisticIdentifier * Statistic -> unit)
            (avatarMessagePurger         : AvatarMessagePurger)
            (command                     : Command option) 
            (side                        : Side) 
            (world                       : World) 
            : Gamestate option =
        world
        |> World.ClearMessages avatarMessagePurger
        match command with
        | Some Command.Metrics ->
            (side, world)
            |> Gamestate.Careened
            |> Gamestate.Metrics
            |> Some
        | Some Command.Inventory ->
            (side, world)
            |> Gamestate.Careened
            |> Gamestate.Inventory
            |> Some
        | Some Command.Status ->
            (side, world)
            |> Gamestate.Careened
            |> Gamestate.Status
            |> Some
        | Some Command.Quit ->
            (side, world)
            |> Gamestate.Careened
            |> Gamestate.ConfirmQuit
            |> Some
        | Some Command.Help ->
            (side, world)
            |> Gamestate.Careened
            |> Gamestate.Help
            |> Some
        | Some Command.CleanHull ->
            (side, world 
            |> World.CleanHull 
                vesselSingleStatisticSource
                vesselSingleStatisticSink
                (if side=Port then Starboard else Port))
            |> Gamestate.Careened
            |> Some
        | Some Command.WeighAnchor ->
            world
            |> Gamestate.AtSea
            |> Some
        | _ ->
            ("Maybe try 'help'?",(side, world)
            |> Gamestate.Careened)
            |> Gamestate.ErrorMessage
            |> Some

    let private RunAlive 
            (vesselSingleStatisticSource : string -> VesselStatisticIdentifier -> Statistic option)
            (vesselSingleStatisticSink   : string -> VesselStatisticIdentifier*Statistic -> unit)
            (avatarMessageSource         : AvatarMessageSource)
            (avatarMessagePurger         : AvatarMessagePurger)
            (source:CommandSource) 
            (sink:MessageSink) 
            (side:Side) 
            (world:World) 
            : Gamestate option =
        UpdateDisplay 
            vesselSingleStatisticSource
            avatarMessageSource
            sink 
            side 
            world
        HandleCommand
            vesselSingleStatisticSource
            vesselSingleStatisticSink
            avatarMessagePurger
            (source())
            side
            world

    let Run 
            (vesselSingleStatisticSource : string -> VesselStatisticIdentifier -> Statistic option)
            (vesselSingleStatisticSink   : string -> VesselStatisticIdentifier*Statistic -> unit)
            (avatarMessageSource         : AvatarMessageSource)
            (avatarMessagePurger         : AvatarMessagePurger)
            (commandSource:CommandSource) 
            (messageSink:MessageSink) 
            (side:Side) 
            (world:World) 
            : Gamestate option =
        if world |> World.IsAvatarAlive then
            RunAlive 
                vesselSingleStatisticSource
                vesselSingleStatisticSink
                avatarMessageSource
                avatarMessagePurger
                commandSource 
                messageSink 
                side 
                world
        else
            world.AvatarId
            |> avatarMessageSource
            |> Gamestate.GameOver
            |> Some