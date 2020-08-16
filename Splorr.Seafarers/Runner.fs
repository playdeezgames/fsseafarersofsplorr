namespace Splorr.Seafarers

open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open System

module Runner =


    let rec private Loop 
            (switches                        : Set<string>) 
            (nameSource                      : TermSource)
            (termSources                     : TermSource * TermSource * TermSource * TermSource * TermSource * TermSource)
            (commoditySource                 : CommoditySource)
            (itemSource                      : ItemSource)
            (worldSingleStatisticSource      : WorldSingleStatisticSource)
            (shipmateStatisticTemplateSource : ShipmateStatisticTemplateSource)
            (rationItemSource                : RationItemSource)
            (islandMarketSource              : IslandMarketSource) 
            (islandSingleMarketSource        : IslandSingleMarketSource)
            (islandMarketSink                : IslandMarketSink) 
            (islandSingleMarketSink          : IslandSingleMarketSink) 
            (islandItemSource                : IslandItemSource) 
            (islandItemSink                  : IslandItemSink) 
            (vesselStatisticTemplateSource   : VesselStatisticTemplateSource)
            (vesselStatisticSink             : VesselStatisticSink)
            (vesselSingleStatisticSource     : VesselSingleStatisticSource)
            (vesselSingleStatisticSink       : VesselSingleStatisticSink)
            (shipmateRationItemSource        : ShipmateRationItemSource)
            (avatarShipmateSource            : AvatarShipmateSource)
            (shipmateSingleStatisticSource   : ShipmateSingleStatisticSource)
            (shipmateSingleStatisticSink     : ShipmateSingleStatisticSink)
            (shipmateRationItemSink          : ShipmateRationItemSink)
            (avatarMessageSource             : AvatarMessageSource)
            (avatarMessageSink               : AvatarMessageSink)
            (avatarMessagePurger             : AvatarMessagePurger)
            (random                          : Random) 
            (commandSource                   : CommandSource) 
            (messageSink                     : MessageSink) 
            (gamestate                       : Gamestate) 
            : unit =

        let nextGamestate : Gamestate option = 
            match gamestate with
            | Gamestate.AtSea world -> 
                AtSea.Run 
                    termSources
                    commoditySource 
                    itemSource 
                    worldSingleStatisticSource
                    islandMarketSource
                    islandMarketSink 
                    islandItemSource 
                    islandItemSink 
                    vesselSingleStatisticSource
                    vesselSingleStatisticSink
                    shipmateRationItemSource
                    avatarShipmateSource
                    shipmateSingleStatisticSource
                    shipmateSingleStatisticSink
                    avatarMessageSource
                    avatarMessageSink
                    avatarMessagePurger
                    random 
                    commandSource 
                    messageSink 
                    world

            | Gamestate.Careened (side, world) -> 
                Careened.Run 
                    vesselSingleStatisticSource
                    vesselSingleStatisticSink
                    avatarShipmateSource
                    shipmateSingleStatisticSource
                    shipmateSingleStatisticSink
                    avatarMessageSource
                    avatarMessagePurger
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
                    switches 
                    commandSource 
                    messageSink 
                    state

            | Gamestate.Docked (Dock, location, world) -> 
                Docked.Run 
                    commoditySource 
                    itemSource 
                    islandMarketSource 
                    islandSingleMarketSource
                    islandSingleMarketSink 
                    vesselSingleStatisticSource
                    shipmateSingleStatisticSource
                    shipmateSingleStatisticSink
                    avatarMessageSource
                    avatarMessageSink
                    avatarMessagePurger
                    commandSource 
                    messageSink 
                    location 
                    world

            | Gamestate.Docked (ItemList, location, world) -> 
                ItemList.Run 
                    commoditySource 
                    itemSource 
                    islandMarketSource 
                    islandItemSource 
                    shipmateSingleStatisticSource
                    avatarMessageSource
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
                    nameSource
                    worldSingleStatisticSource
                    shipmateStatisticTemplateSource
                    shipmateSingleStatisticSink
                    rationItemSource
                    vesselStatisticTemplateSource
                    vesselStatisticSink
                    vesselSingleStatisticSource
                    shipmateRationItemSink
                    commandSource 
                    messageSink 
                    world

            | Gamestate.Metrics state -> 
                Metrics.Run 
                    messageSink 
                    state

            | Gamestate.Status state -> 
                Status.Run 
                    shipmateSingleStatisticSource
                    vesselSingleStatisticSource
                    messageSink 
                    state

            |> Gamestate.CheckForAvatarDeath 
                shipmateSingleStatisticSource
                avatarMessageSource

        match nextGamestate with
        | Some state ->
            Loop 
                switches 
                nameSource
                termSources
                commoditySource 
                itemSource 
                worldSingleStatisticSource
                shipmateStatisticTemplateSource
                rationItemSource
                islandMarketSource 
                islandSingleMarketSource
                islandMarketSink 
                islandSingleMarketSink 
                islandItemSource 
                islandItemSink 
                vesselStatisticTemplateSource
                vesselStatisticSink
                vesselSingleStatisticSource
                vesselSingleStatisticSink
                shipmateRationItemSource
                avatarShipmateSource
                shipmateSingleStatisticSource
                shipmateSingleStatisticSink
                shipmateRationItemSink
                avatarMessageSource
                avatarMessageSink
                avatarMessagePurger
                random 
                commandSource 
                messageSink 
                state

        | None ->
            ()
    
    let Run 
            (switches                        : Set<string>) 
            (nameSource                      : TermSource)
            (termSources                     : TermSource * TermSource * TermSource * TermSource * TermSource * TermSource)
            (commoditySource                 : CommoditySource) 
            (itemSource                      : ItemSource) 
            (worldSingleStatisticSource      : WorldSingleStatisticSource)
            (shipmateStatisticTemplateSource : ShipmateStatisticTemplateSource)
            (rationItemSource                : RationItemSource)
            (islandMarketSource              : IslandMarketSource) 
            (islandSingleMarketSource        : IslandSingleMarketSource) 
            (islandMarketSink                : IslandMarketSink) 
            (islandSingleMarketSink          : IslandSingleMarketSink) 
            (islandItemSource                : IslandItemSource) 
            (islandListSink                  : IslandItemSink) 
            (vesselStatisticTemplateSource   : VesselStatisticTemplateSource)
            (vesselStatisticSink             : VesselStatisticSink)
            (vesselSingleStatisticSource     : VesselSingleStatisticSource)
            (vesselSingleStatisticSink       : VesselSingleStatisticSink)
            (shipmateRationItemSource        : ShipmateRationItemSource)
            (shipmateRationItemSink          : ShipmateRationItemSink)
            (avatarShipmateSource            : AvatarShipmateSource)
            (shipmateSingleStatisticSource   : ShipmateSingleStatisticSource)
            (shipmateSingleStatisticSink     : ShipmateSingleStatisticSink)
            (avatarMessageSource             : AvatarMessageSource)
            (avatarMessageSink               : AvatarMessageSink)
            (avatarMessagePurger             : AvatarMessagePurger)
            : unit =

        Console.Title <- "Seafarers of SPLORR!!"
        let old = Console.ForegroundColor
        Console.ForegroundColor <- ConsoleColor.Gray

        None
        |> Gamestate.MainMenu
        |> Loop 
            switches 
            nameSource
            termSources
            commoditySource 
            itemSource 
            worldSingleStatisticSource
            shipmateStatisticTemplateSource
            rationItemSource
            islandMarketSource 
            islandSingleMarketSource
            islandMarketSink 
            islandSingleMarketSink 
            islandItemSource 
            islandListSink 
            vesselStatisticTemplateSource
            vesselStatisticSink
            vesselSingleStatisticSource
            vesselSingleStatisticSink
            shipmateRationItemSource
            avatarShipmateSource
            shipmateSingleStatisticSource
            shipmateSingleStatisticSink
            shipmateRationItemSink
            avatarMessageSource
            avatarMessageSink
            avatarMessagePurger
            (Random()) 
            (fun () -> CommandSource.Read Console.ReadLine) 
            MessageSink.Write

        Console.ForegroundColor <- old
