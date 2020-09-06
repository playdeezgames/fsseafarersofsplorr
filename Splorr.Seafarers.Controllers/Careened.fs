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
            (avatarMessagePurger           : AvatarMessagePurger)
            (avatarShipmateSource          : AvatarShipmateSource)
            (avatarSingleMetricSink        : AvatarSingleMetricSink)
            (avatarSingleMetricSource      : AvatarSingleMetricSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (vesselSingleStatisticSink     : VesselSingleStatisticSink)
            (vesselSingleStatisticSource   : VesselSingleStatisticSource)
            (command                       : Command option) 
            (side                          : Side) 
            (avatarId                      : string) 
            : Gamestate option =
        avatarId
        |> World.ClearMessages avatarMessagePurger
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
                avatarShipmateSource
                avatarSingleMetricSink
                avatarSingleMetricSource
                shipmateSingleStatisticSink
                shipmateSingleStatisticSource
                vesselSingleStatisticSink
                vesselSingleStatisticSource
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
            (avatarMessagePurger           : AvatarMessagePurger)
            (avatarMessageSource           : AvatarMessageSource)
            (avatarShipmateSource          : AvatarShipmateSource)
            (avatarSingleMetricSink        : AvatarSingleMetricSink)
            (avatarSingleMetricSource      : AvatarSingleMetricSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (vesselSingleStatisticSink     : VesselSingleStatisticSink)
            (vesselSingleStatisticSource   : VesselSingleStatisticSource)
            (source                        : CommandSource) 
            (sink                          : MessageSink) 
            (side                          : Side) 
            (avatarId                      : string) 
            : Gamestate option =
        UpdateDisplay 
            vesselSingleStatisticSource
            avatarMessageSource
            sink 
            side 
            avatarId
        HandleCommand
            avatarMessagePurger
            avatarShipmateSource
            avatarSingleMetricSink
            avatarSingleMetricSource
            shipmateSingleStatisticSink
            shipmateSingleStatisticSource
            vesselSingleStatisticSink
            vesselSingleStatisticSource
            (source())
            side
            avatarId

    let Run 
            (avatarMessagePurger           : AvatarMessagePurger)
            (avatarMessageSource           : AvatarMessageSource)
            (avatarShipmateSource          : AvatarShipmateSource)
            (avatarSingleMetricSink        : AvatarSingleMetricSink)
            (avatarSingleMetricSource      : AvatarSingleMetricSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (vesselSingleStatisticSink     : VesselSingleStatisticSink)
            (vesselSingleStatisticSource   : VesselSingleStatisticSource)
            (commandSource                 : CommandSource) 
            (messageSink                   : MessageSink) 
            (side                          : Side) 
            (avatarId                      : string) 
            : Gamestate option =
        if avatarId |> World.IsAvatarAlive shipmateSingleStatisticSource then
            RunAlive 
                avatarMessagePurger
                avatarMessageSource
                avatarShipmateSource
                avatarSingleMetricSink
                avatarSingleMetricSource
                shipmateSingleStatisticSink
                shipmateSingleStatisticSource
                vesselSingleStatisticSink
                vesselSingleStatisticSource
                commandSource 
                messageSink 
                side 
                avatarId
        else
            avatarId
            |> avatarMessageSource
            |> Gamestate.GameOver
            |> Some