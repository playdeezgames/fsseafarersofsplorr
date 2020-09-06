﻿namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

type IslandFeatureSource = Location -> IslandFeatureIdentifier list

type DockedUpdateDisplayContext =
    abstract member avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource
    abstract member avatarMessageSource            : AvatarMessageSource
    abstract member islandSingleNameSource         : IslandSingleNameSource
    abstract member islandFeatureSource            : IslandFeatureSource

type DockedHandleCommandContext = 
    inherit WorldUndockContext
    abstract member avatarInventorySink            : AvatarInventorySink
    abstract member avatarInventorySource          : AvatarInventorySource
    abstract member avatarIslandSingleMetricSink   : AvatarIslandSingleMetricSink
    abstract member avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource
    abstract member avatarJobSink                  : AvatarJobSink
    abstract member avatarJobSource                : AvatarJobSource
    abstract member avatarMessagePurger            : AvatarMessagePurger
    abstract member avatarMessageSink              : AvatarMessageSink
    abstract member avatarSingleMetricSink         : AvatarSingleMetricSink
    abstract member avatarSingleMetricSource       : AvatarSingleMetricSource
    abstract member commoditySource                : CommoditySource 
    abstract member islandJobPurger                : IslandJobPurger
    abstract member islandMarketSource             : IslandMarketSource 
    abstract member islandSingleJobSource          : IslandSingleJobSource
    abstract member islandSingleMarketSink         : IslandSingleMarketSink 
    abstract member islandSingleMarketSource       : IslandSingleMarketSource 
    abstract member islandSource                   : IslandSource
    abstract member itemSource                     : ItemSource 
    abstract member shipmateSingleStatisticSink    : ShipmateSingleStatisticSink
    abstract member shipmateSingleStatisticSource  : ShipmateSingleStatisticSource
    abstract member vesselSingleStatisticSource    : VesselSingleStatisticSource


type DockedRunContext =
    inherit DockedUpdateDisplayContext
    inherit DockedHandleCommandContext

module Docked = 
    let private getFeatureDisplayName (feature:IslandFeatureIdentifier) : Message =
        match feature with
        | IslandFeatureIdentifier.Dock ->
            "" |> Text
        | IslandFeatureIdentifier.DarkAlley ->
            (Hue.Flavor, "There is a dark alley here." |> Line) |> Hued
        | _ ->
            raise (System.NotImplementedException (sprintf "%s" (feature.ToString())))

    let private UpdateDisplay 
            (context     : DockedUpdateDisplayContext)
            (messageSink : MessageSink) 
            (location    : Location) 
            (avatarId    : string) 
            : unit =
        "" |> Line |> messageSink
        avatarId
        |> context.avatarMessageSource
        |> Utility.DumpMessages messageSink
        context.islandFeatureSource location
        |> List.map
            getFeatureDisplayName
        |> List.append
            [
                (Hue.Heading, sprintf "You are docked at '%s':" (context.islandSingleNameSource location |> Option.get) |> Line) |> Hued
                (Hue.Flavor, sprintf "You have visited %u times." (context.avatarIslandSingleMetricSource avatarId location AvatarIslandMetricIdentifier.VisitCount |> Option.defaultValue 0UL) |> Line) |> Hued
            ]
        |> List.iter messageSink

    let private HandleCommand
            (context : DockedHandleCommandContext)
            (command                        : Command option) 
            (location                       : Location) 
            (avatarId                       : string) 
            : Gamestate option =
        avatarId
        |> World.ClearMessages context.avatarMessagePurger

        match command with
        | Some (Command.GoTo feature) ->
            (Some(feature, 
                location), 
                avatarId)
            |> Gamestate.InPlay
            |> Some

        | Some (Command.AcceptJob index) ->
            avatarId 
            |> World.AcceptJob
                context.avatarIslandSingleMetricSink
                context.avatarIslandSingleMetricSource
                context.avatarJobSink
                context.avatarJobSource
                context.avatarMessageSink 
                context.avatarSingleMetricSink
                context.avatarSingleMetricSource
                context.islandJobPurger
                context.islandSingleJobSource
                context.islandSource
                index 
                location
            (Some(IslandFeatureIdentifier.Dock, 
                location), 
                avatarId)
            |> Gamestate.InPlay
            |> Some

        | Some (Command.Buy (quantity, itemName))->
            avatarId 
            |> World.BuyItems 
                context.avatarInventorySink
                context.avatarInventorySource
                context.avatarMessageSink 
                context.commoditySource 
                context.islandMarketSource 
                context.islandSingleMarketSink 
                context.islandSingleMarketSource 
                context.islandSource
                context.itemSource 
                context.shipmateSingleStatisticSink
                context.shipmateSingleStatisticSource
                context.vesselSingleStatisticSource 
                location 
                quantity 
                itemName
            (Some(IslandFeatureIdentifier.Dock, 
                location), 
                    avatarId) 
            |> Gamestate.InPlay
            |> Some            

        | Some (Command.Sell (quantity, itemName))->
            avatarId 
            |> World.SellItems 
                context.avatarInventorySink
                context.avatarInventorySource
                context.avatarMessageSink
                context.commoditySource 
                context.islandMarketSource 
                context.islandSingleMarketSink 
                context.islandSingleMarketSource 
                context.islandSource
                context.itemSource 
                context.shipmateSingleStatisticSink
                context.shipmateSingleStatisticSource
                location 
                quantity 
                itemName
            (Some(IslandFeatureIdentifier.Dock, 
                location), 
                    avatarId) 
            |> Gamestate.InPlay
            |> Some            

        | Some Command.Items ->
            (Some(IslandFeatureIdentifier.Dock, location), avatarId) 
            |> Gamestate.InPlay
            |> Gamestate.ItemList
            |> Some

        | Some Command.Jobs ->
            (Some(IslandFeatureIdentifier.Dock, location), avatarId)
            |> Gamestate.InPlay
            |> Gamestate.Jobs
            |> Some

        | Some Command.Status ->
            (Some(IslandFeatureIdentifier.Dock, location), avatarId)
            |> Gamestate.InPlay
            |> Gamestate.Status
            |> Some

        | Some (Command.Abandon Job) ->
            avatarId 
            |> World.AbandonJob 
                context.avatarJobSink
                context.avatarJobSource
                context.avatarMessageSink
                context.avatarSingleMetricSink
                context.avatarSingleMetricSource
                context.shipmateSingleStatisticSink
                context.shipmateSingleStatisticSource
            (Some(IslandFeatureIdentifier.Dock, 
                location), 
                    avatarId)
            |> Gamestate.InPlay
            |> Some

        | Some Command.Undock ->
            avatarId 
            |> World.Undock context
            (None, avatarId)
            |> Gamestate.InPlay 
            |> Some

        | Some Command.Quit ->
            (Some(IslandFeatureIdentifier.Dock, location), avatarId) 
            |> Gamestate.InPlay 
            |> Gamestate.ConfirmQuit 
            |> Some

        | Some Command.Inventory ->
            (Some(IslandFeatureIdentifier.Dock, location), avatarId) 
            |> Gamestate.InPlay 
            |> Gamestate.Inventory 
            |> Some

        | Some Command.Help ->
            (Some(IslandFeatureIdentifier.Dock, location), avatarId) 
            |> Gamestate.InPlay 
            |> Gamestate.Help 
            |> Some

        | Some Command.Metrics ->
            (Some(IslandFeatureIdentifier.Dock, location), avatarId) 
            |> Gamestate.InPlay 
            |> Gamestate.Metrics 
            |> Some

        | _ -> 
            ("Maybe try 'help'?",(Some(IslandFeatureIdentifier.Dock, location), avatarId) 
            |> Gamestate.InPlay)
            |> Gamestate.ErrorMessage
            |> Some

    let private RunWithIsland 
            (context                        : DockedRunContext)
            (commandSource                  : CommandSource) 
            (messageSink                    : MessageSink) 
            (location                       : Location) 
            (avatarId                       : string) 
            : Gamestate option =
        avatarId
        |> UpdateDisplay 
            context
            messageSink 
            location 
        
        avatarId   
        |> HandleCommand 
            context
            (commandSource()) 
            location 

    let internal RunBoilerplate 
            (avatarMessageSource           : AvatarMessageSource)
            (islandSource                  : IslandSource)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (func                          : Location -> string -> Gamestate option) 
            (location                      : Location) 
            (avatarId                      : string) 
            : Gamestate option =
        if avatarId |> World.IsAvatarAlive shipmateSingleStatisticSource then
            if islandSource() |> List.exists (fun x->x= location) then
                func location avatarId
            else
                (None, avatarId)
                |> Gamestate.InPlay
                |> Some
        else
            avatarId
            |> avatarMessageSource
            |> Gamestate.GameOver
            |> Some

    let Run 
            (context       : DockedRunContext)
            (commandSource : CommandSource) 
            (messageSink   : MessageSink) =
        RunBoilerplate 
            context.avatarMessageSource
            context.islandSource
            context.shipmateSingleStatisticSource
            (RunWithIsland 
                context
                commandSource                
                messageSink)                 
