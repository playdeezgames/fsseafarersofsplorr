﻿namespace Splorr.Seafarers

open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Services
open System

module Runner =


    let rec private Loop 
            (avatarInventorySink             : AvatarInventorySink)
            (avatarInventorySource           : AvatarInventorySource)
            (avatarJobSink                   : AvatarJobSink)
            (avatarJobSource                 : AvatarJobSource)
            (avatarMessagePurger             : AvatarMessagePurger)
            (avatarMessageSink               : AvatarMessageSink)
            (avatarMessageSource             : AvatarMessageSource)
            (avatarMetricSource              : AvatarMetricSource)
            (avatarShipmateSource            : AvatarShipmateSource)
            (avatarSingleMetricSink          : AvatarSingleMetricSink)
            (avatarSingleMetricSource        : AvatarSingleMetricSource)
            (commoditySource                 : CommoditySource)
            (islandItemSink                  : IslandItemSink) 
            (islandItemSource                : IslandItemSource) 
            (islandMarketSink                : IslandMarketSink) 
            (islandMarketSource              : IslandMarketSource) 
            (islandSingleMarketSink          : IslandSingleMarketSink) 
            (islandSingleMarketSource        : IslandSingleMarketSource)
            (itemSource                      : ItemSource)
            (rationItemSource                : RationItemSource)
            (shipmateRationItemSink          : ShipmateRationItemSink)
            (shipmateRationItemSource        : ShipmateRationItemSource)
            (shipmateSingleStatisticSink     : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource   : ShipmateSingleStatisticSource)
            (shipmateStatisticTemplateSource : ShipmateStatisticTemplateSource)
            (switchSource                    : SwitchSource)
            (termNameSource                  : TermSource)
            (termSources                     : TermSources)
            (vesselSingleStatisticSink       : VesselSingleStatisticSink)
            (vesselStatisticSink             : VesselStatisticSink)
            (vesselSingleStatisticSource     : VesselSingleStatisticSource)
            (vesselStatisticTemplateSource   : VesselStatisticTemplateSource)
            (worldSingleStatisticSource      : WorldSingleStatisticSource)
            (random                          : Random) 
            (commandSource                   : CommandSource) 
            (messageSink                     : MessageSink) 
            (gamestate                       : Gamestate) 
            : unit =

        let nextGamestate : Gamestate option = 
            match gamestate with
            | Gamestate.AtSea world -> 
                AtSea.Run 
                    avatarInventorySink
                    avatarInventorySource
                    avatarJobSink
                    avatarJobSource
                    avatarMessagePurger
                    avatarMessageSink
                    avatarMessageSource
                    avatarShipmateSource
                    avatarSingleMetricSink
                    avatarSingleMetricSource
                    commoditySource 
                    islandItemSink 
                    islandItemSource 
                    islandMarketSink 
                    islandMarketSource
                    itemSource 
                    shipmateRationItemSource
                    shipmateSingleStatisticSink
                    shipmateSingleStatisticSource
                    termSources
                    vesselSingleStatisticSink
                    vesselSingleStatisticSource
                    worldSingleStatisticSource
                    random 
                    commandSource 
                    messageSink 
                    world

            | Gamestate.Careened (side, world) -> 
                Careened.Run 
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
                    world

            | Gamestate.Chart (chartName, world) -> 
                Chart.Run 
                    vesselSingleStatisticSource
                    worldSingleStatisticSource
                    messageSink 
                    chartName 
                    world

            | Gamestate.ConfirmQuit state -> 
                ConfirmQuit.Run 
                    switchSource 
                    commandSource 
                    messageSink 
                    state

            | Gamestate.Docked (Dock, location, world) -> 
                Docked.Run 
                    avatarInventorySink
                    avatarInventorySource
                    avatarJobSink
                    avatarJobSource
                    avatarMessagePurger
                    avatarMessageSink
                    avatarMessageSource
                    avatarSingleMetricSink
                    avatarSingleMetricSource
                    commoditySource 
                    islandMarketSource 
                    islandSingleMarketSink 
                    islandSingleMarketSource
                    itemSource 
                    shipmateSingleStatisticSink
                    shipmateSingleStatisticSource
                    vesselSingleStatisticSource
                    commandSource 
                    messageSink 
                    location 
                    world

            | Gamestate.Docked (ItemList, location, world) -> 
                ItemList.Run 
                    avatarMessageSource
                    commoditySource 
                    islandItemSource 
                    islandMarketSource 
                    itemSource 
                    shipmateSingleStatisticSource
                    messageSink 
                    location 
                    world

            | Gamestate.Docked (Jobs, location, world) -> 
                Jobs.Run 
                    messageSink 
                    location
                    world

            | Gamestate.GameOver messages -> 
                GameOver.Run 
                    messageSink 
                    messages

            | Gamestate.Help state -> 
                Help.Run 
                    messageSink 
                    state

            | Gamestate.ErrorMessage (message, state) ->
                ErrorMessage.Run
                    messageSink
                    message
                    state

            | Gamestate.Inventory gameState -> 
                Inventory.Run 
                    avatarInventorySource
                    itemSource 
                    vesselSingleStatisticSource
                    messageSink 
                    gameState

            | Gamestate.IslandList (page, state) -> 
                IslandList.Run 
                    vesselSingleStatisticSource
                    messageSink 
                    page 
                    state

            | Gamestate.MainMenu world -> 
                MainMenu.Run 
                    avatarJobSink
                    rationItemSource
                    shipmateRationItemSink
                    shipmateSingleStatisticSink
                    shipmateStatisticTemplateSource
                    termNameSource
                    vesselSingleStatisticSource
                    vesselStatisticSink
                    vesselStatisticTemplateSource
                    worldSingleStatisticSource
                    commandSource 
                    messageSink 
                    world

            | Gamestate.Metrics state -> 
                Metrics.Run 
                    avatarMetricSource
                    messageSink 
                    state

            | Gamestate.Status state -> 
                Status.Run 
                    avatarJobSource
                    shipmateSingleStatisticSource
                    vesselSingleStatisticSource
                    messageSink 
                    state

            |> Gamestate.CheckForAvatarDeath 
                avatarMessageSource
                shipmateSingleStatisticSource

        match nextGamestate with
        | Some state ->
            Loop 
                avatarInventorySink
                avatarInventorySource
                avatarJobSink
                avatarJobSource
                avatarMessagePurger
                avatarMessageSink
                avatarMessageSource
                avatarMetricSource
                avatarShipmateSource
                avatarSingleMetricSink
                avatarSingleMetricSource
                commoditySource 
                islandItemSink 
                islandItemSource 
                islandMarketSink 
                islandMarketSource 
                islandSingleMarketSink 
                islandSingleMarketSource
                itemSource 
                rationItemSource
                shipmateRationItemSink
                shipmateRationItemSource
                shipmateSingleStatisticSink
                shipmateSingleStatisticSource
                shipmateStatisticTemplateSource
                switchSource 
                termNameSource
                termSources
                vesselSingleStatisticSink
                vesselStatisticSink
                vesselSingleStatisticSource
                vesselStatisticTemplateSource
                worldSingleStatisticSource
                random 
                commandSource 
                messageSink 
                state

        | None ->
            ()
    
    let Run 
            (avatarInventorySink             : AvatarInventorySink)
            (avatarInventorySource           : AvatarInventorySource)
            (avatarJobSink                   : AvatarJobSink)
            (avatarJobSource                 : AvatarJobSource)
            (avatarMessagePurger             : AvatarMessagePurger)
            (avatarMessageSink               : AvatarMessageSink)
            (avatarMessageSource             : AvatarMessageSource)
            (avatarMetricSource              : AvatarMetricSource)
            (avatarShipmateSource            : AvatarShipmateSource)
            (avatarSingleMetricSink          : AvatarSingleMetricSink)
            (avatarSingleMetricSource        : AvatarSingleMetricSource)
            (commoditySource                 : CommoditySource) 
            (islandItemSink                  : IslandItemSink) 
            (islandItemSource                : IslandItemSource) 
            (islandMarketSink                : IslandMarketSink) 
            (islandMarketSource              : IslandMarketSource) 
            (islandSingleMarketSink          : IslandSingleMarketSink) 
            (islandSingleMarketSource        : IslandSingleMarketSource) 
            (itemSource                      : ItemSource) 
            (rationItemSource                : RationItemSource)
            (shipmateRationItemSink          : ShipmateRationItemSink)
            (shipmateRationItemSource        : ShipmateRationItemSource)
            (shipmateSingleStatisticSink     : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource   : ShipmateSingleStatisticSource)
            (shipmateStatisticTemplateSource : ShipmateStatisticTemplateSource)
            (switchSource                    : SwitchSource) 
            (termNameSource                  : TermSource)
            (termSources                     : TermSources)
            (vesselSingleStatisticSink       : VesselSingleStatisticSink)
            (vesselSingleStatisticSource     : VesselSingleStatisticSource)
            (vesselStatisticSink             : VesselStatisticSink)
            (vesselStatisticTemplateSource   : VesselStatisticTemplateSource)
            (worldSingleStatisticSource      : WorldSingleStatisticSource)
            : unit =

        Console.Title <- "Seafarers of SPLORR!!"
        let old = Console.ForegroundColor
        Console.ForegroundColor <- ConsoleColor.Gray

        None
        |> Gamestate.MainMenu
        |> Loop 
            avatarInventorySink
            avatarInventorySource
            avatarJobSink
            avatarJobSource
            avatarMessagePurger
            avatarMessageSink
            avatarMessageSource
            avatarMetricSource
            avatarShipmateSource
            avatarSingleMetricSink
            avatarSingleMetricSource
            commoditySource 
            islandItemSink 
            islandItemSource 
            islandMarketSink 
            islandMarketSource 
            islandSingleMarketSink 
            islandSingleMarketSource
            itemSource 
            rationItemSource
            shipmateRationItemSink
            shipmateRationItemSource
            shipmateSingleStatisticSink
            shipmateSingleStatisticSource
            shipmateStatisticTemplateSource
            switchSource 
            termNameSource
            termSources
            vesselSingleStatisticSink
            vesselStatisticSink
            vesselSingleStatisticSource
            vesselStatisticTemplateSource
            worldSingleStatisticSource
            (Random()) 
            (fun () -> CommandSource.Read Console.ReadLine) 
            MessageSink.Write

        Console.ForegroundColor <- old
