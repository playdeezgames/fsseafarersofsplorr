﻿namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module MainMenu =
    let private UpdateDisplayHeader 
            (messageSink : MessageSink) 
            : unit =
        [
            "" |> Line
            (Hue.Heading, "Main Menu Commands:" |> Line) |> Hued
        ]
        |> List.iter messageSink

    let private UpdateDisplayInGame 
            (messageSink : MessageSink) 
            : unit =
        UpdateDisplayHeader messageSink
        [
            (Hue.Label, "resume" |> Text) |> Hued
            (Hue.Usage, " - resume game" |> Line) |> Hued
            (Hue.Label, "abandon game" |> Text) |> Hued
            (Hue.Usage, " - abandon game" |> Line) |> Hued
        ]
        |> List.iter messageSink

    let private UpdateDisplayNoGame 
            (messageSink : MessageSink) 
            : unit =
        UpdateDisplayHeader messageSink
        [
            (Hue.Label, "start" |> Text) |> Hued
            (Hue.Usage, " - starts a new world" |> Line) |> Hued
            (Hue.Label, "quit" |> Text) |> Hued
            (Hue.Usage, " - quits the game" |> Line) |> Hued
        ]
        |> List.iter messageSink
    
    let private UpdateDisplay 
            (messageSink : MessageSink) 
            (inGame      : bool) 
            : unit =
        if inGame then
            UpdateDisplayInGame messageSink
        else
            UpdateDisplayNoGame messageSink

    let private HandleInvalidCommand 
            (avatarId : string option) 
            : Gamestate option =
        ("Invalid command.", avatarId
        |> Gamestate.MainMenu)
        |> Gamestate.ErrorMessage
        |> Some

    let private HandleCommandInGame 
            (avatarId : string) =
        function
        | Some Command.Resume ->
            avatarId
            |> Gamestate.AtSea
            |> Some
        | Some (Command.Abandon Game)->
            None
            |> Gamestate.MainMenu
            |> Some
        | _ ->
            avatarId
            |> Some
            |> HandleInvalidCommand

    let private HandleCommandNoGame
            (avatarIslandSingleMetricSink    : AvatarIslandSingleMetricSink)
            (avatarJobSink                   : AvatarJobSink)
            (islandFeatureGeneratorSource    : IslandFeatureGeneratorSource)
            (islandSingleNameSink            : IslandSingleNameSink)
            (islandSingleStatisticSink       : IslandSingleStatisticSink)
            (islandSource                    : IslandSource)
            (islandStatisticTemplateSource   : IslandStatisticTemplateSource)
            (nameSource                      : TermSource)
            (rationItemSource                : RationItemSource)
            (shipmateRationItemSink          : ShipmateRationItemSink)
            (shipmateSingleStatisticSink     : ShipmateSingleStatisticSink)
            (shipmateStatisticTemplateSource : ShipmateStatisticTemplateSource)
            (vesselSingleStatisticSource     : VesselSingleStatisticSource)
            (vesselStatisticSink             : VesselStatisticSink)
            (vesselStatisticTemplateSource   : VesselStatisticTemplateSource)
            (worldSingleStatisticSource      : WorldSingleStatisticSource)
            =
        function
        | Some (Command.Start avatarId)->
            World.Create 
                avatarIslandSingleMetricSink
                avatarJobSink
                islandFeatureGeneratorSource
                islandSingleNameSink
                islandSingleStatisticSink
                islandSource
                islandStatisticTemplateSource
                nameSource
                worldSingleStatisticSource
                shipmateStatisticTemplateSource
                shipmateSingleStatisticSink
                rationItemSource
                vesselStatisticTemplateSource
                vesselStatisticSink
                vesselSingleStatisticSource
                shipmateRationItemSink
                (System.Random())
                avatarId
            avatarId
            |> Gamestate.AtSea
            |> Some
        | Some Command.Quit ->
            None
            |> Gamestate.MainMenu
            |> Gamestate.ConfirmQuit
            |> Some
        | _ ->
            None
            |> HandleInvalidCommand


    let private HandleCommand
            (avatarIslandSingleMetricSink    : AvatarIslandSingleMetricSink)
            (avatarJobSink                   : AvatarJobSink)
            (islandFeatureGeneratorSource    : IslandFeatureGeneratorSource)
            (islandSingleNameSink            : IslandSingleNameSink)
            (islandSingleStatisticSink       : IslandSingleStatisticSink)
            (islandSource                    : IslandSource)
            (islandStatisticTemplateSource   : IslandStatisticTemplateSource)
            (nameSource                      : TermSource)
            (worldSingleStatisticSource      : WorldSingleStatisticSource)
            (rationItemSource                : RationItemSource)
            (shipmateStatisticTemplateSource : ShipmateStatisticTemplateSource)
            (shipmateSingleStatisticSink     : ShipmateSingleStatisticSink)
            (vesselStatisticTemplateSource   : VesselStatisticTemplateSource)
            (vesselStatisticSink             : VesselStatisticSink)
            (vesselSingleStatisticSource     : VesselSingleStatisticSource)
            (shipmateRationItemSink          : ShipmateRationItemSink)
            (avatarId                        : string option) 
            (command                         : Command option) 
            : Gamestate option =
        match avatarId with
        | Some w ->
            HandleCommandInGame w command
        | _ ->
            HandleCommandNoGame 
                avatarIslandSingleMetricSink
                avatarJobSink
                islandFeatureGeneratorSource
                islandSingleNameSink
                islandSingleStatisticSink
                islandSource
                islandStatisticTemplateSource
                nameSource
                rationItemSource
                shipmateRationItemSink
                shipmateSingleStatisticSink
                shipmateStatisticTemplateSource
                vesselSingleStatisticSource
                vesselStatisticSink
                vesselStatisticTemplateSource
                worldSingleStatisticSource
                command

    let Run 
            (avatarIslandSingleMetricSink    : AvatarIslandSingleMetricSink)
            (avatarJobSink                   : AvatarJobSink)
            (islandFeatureGeneratorSource    : IslandFeatureGeneratorSource)
            (islandSingleNameSink            : IslandSingleNameSink)
            (islandSingleStatisticSink       : IslandSingleStatisticSink)
            (islandSource                    : IslandSource)
            (islandStatisticTemplateSource   : IslandStatisticTemplateSource)
            (rationItemSource                : RationItemSource)
            (shipmateRationItemSink          : ShipmateRationItemSink)
            (shipmateSingleStatisticSink     : ShipmateSingleStatisticSink)
            (shipmateStatisticTemplateSource : ShipmateStatisticTemplateSource)
            (termNameSource                  : TermSource)
            (vesselSingleStatisticSource     : VesselSingleStatisticSource)
            (vesselStatisticSink             : VesselStatisticSink)
            (vesselStatisticTemplateSource   : VesselStatisticTemplateSource)
            (worldSingleStatisticSource      : WorldSingleStatisticSource)
            (commandSource                   : CommandSource) 
            (messageSink                     : MessageSink) 
            (avatarId                        : string option) 
            : Gamestate option =
        UpdateDisplay 
            messageSink 
            avatarId.IsSome
        HandleCommand
            avatarIslandSingleMetricSink
            avatarJobSink
            islandFeatureGeneratorSource
            islandSingleNameSink
            islandSingleStatisticSink
            islandSource
            islandStatisticTemplateSource
            termNameSource
            worldSingleStatisticSource
            rationItemSource
            shipmateStatisticTemplateSource
            shipmateSingleStatisticSink
            vesselStatisticTemplateSource
            vesselStatisticSink
            vesselSingleStatisticSource
            shipmateRationItemSink
            avatarId
            (commandSource())



