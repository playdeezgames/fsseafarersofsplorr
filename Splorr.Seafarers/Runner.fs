namespace Splorr.Seafarers

open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Services
open System

type RunnerRunContext =
    inherit AtSeaRunContext
    abstract member avatarMetricSource              : AvatarMetricSource
    abstract member islandFeatureGeneratorSource    : IslandFeatureGeneratorSource
    abstract member islandJobPurger                 : IslandJobPurger
    abstract member islandSingleJobSource           : IslandSingleJobSource
    abstract member islandSingleMarketSink          : IslandSingleMarketSink 
    abstract member islandSingleMarketSource        : IslandSingleMarketSource
    abstract member islandSingleNameSink            : IslandSingleNameSink 
    abstract member islandSingleStatisticSink       : IslandSingleStatisticSink
    abstract member islandStatisticTemplateSource   : IslandStatisticTemplateSource
    abstract member rationItemSource                : RationItemSource
    abstract member shipmateRationItemSink          : ShipmateRationItemSink
    abstract member shipmateStatisticTemplateSource : ShipmateStatisticTemplateSource
    abstract member switchSource                    : SwitchSource
    abstract member termNameSource                  : TermSource
    abstract member vesselStatisticSink             : VesselStatisticSink
    abstract member vesselStatisticTemplateSource   : VesselStatisticTemplateSource

module Runner =
    let rec private Loop 
            (context       : RunnerRunContext)
            (random        : Random) 
            (commandSource : CommandSource) 
            (messageSink   : MessageSink) 
            (gamestate     : Gamestate) 
            : unit =

        let nextGamestate : Gamestate option = 
            match gamestate with
            | Gamestate.AtSea world -> 
                AtSea.Run 
                    context
                    random 
                    commandSource 
                    messageSink 
                    world

            | Gamestate.Careened (side, world) -> 
                Careened.Run 
                    context.avatarMessagePurger
                    context.avatarMessageSource
                    context.avatarShipmateSource
                    context.avatarSingleMetricSink
                    context.avatarSingleMetricSource
                    context.shipmateSingleStatisticSink
                    context.shipmateSingleStatisticSource
                    context.vesselSingleStatisticSink
                    context.vesselSingleStatisticSource
                    commandSource 
                    messageSink 
                    side 
                    world

            | Gamestate.Chart (chartName, world) -> 
                Chart.Run 
                    context.avatarIslandSingleMetricSource
                    context.islandSingleNameSource
                    context.islandSource 
                    context.vesselSingleStatisticSource
                    context.worldSingleStatisticSource
                    messageSink 
                    chartName 
                    world

            | Gamestate.ConfirmQuit state -> 
                ConfirmQuit.Run 
                    context.switchSource 
                    commandSource 
                    messageSink 
                    state

            | Gamestate.Docked (Dock, location, world) -> 
                Docked.Run 
                    context.avatarInventorySink
                    context.avatarInventorySource
                    context.avatarIslandSingleMetricSink
                    context.avatarIslandSingleMetricSource
                    context.avatarJobSink
                    context.avatarJobSource
                    context.avatarMessagePurger
                    context.avatarMessageSink
                    context.avatarMessageSource
                    context.avatarSingleMetricSink
                    context.avatarSingleMetricSource
                    context.commoditySource 
                    context.islandJobPurger
                    context.islandMarketSource 
                    context.islandSingleJobSource
                    context.islandSingleMarketSink 
                    context.islandSingleMarketSource
                    context.islandSingleNameSource
                    context.islandSource
                    context.itemSource 
                    context.shipmateSingleStatisticSink
                    context.shipmateSingleStatisticSource
                    context.vesselSingleStatisticSource
                    commandSource 
                    messageSink 
                    location 
                    world

            | Gamestate.Docked (ItemList, location, world) -> 
                ItemList.Run 
                    context.avatarMessageSource
                    context.commoditySource 
                    context.islandItemSource 
                    context.islandMarketSource 
                    context.islandSource
                    context.itemSource 
                    context.shipmateSingleStatisticSource
                    messageSink 
                    location 
                    world

            | Gamestate.Docked (Jobs, location, world) -> 
                Jobs.Run 
                    context.islandJobSource
                    context.islandSingleNameSource
                    context.islandSource
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
                    context.avatarInventorySource
                    context.itemSource 
                    context.vesselSingleStatisticSource
                    messageSink 
                    gameState

            | Gamestate.IslandList (page, state) -> 
                IslandList.Run 
                    context.avatarIslandSingleMetricSource
                    context.islandSingleNameSource
                    context.islandSource
                    context.vesselSingleStatisticSource
                    messageSink 
                    page 
                    state

            | Gamestate.MainMenu world -> 
                MainMenu.Run 
                    context.avatarIslandSingleMetricSink
                    context.avatarJobSink
                    context.islandFeatureGeneratorSource
                    context.islandSingleNameSink
                    context.islandSingleStatisticSink
                    context.islandSource
                    context.islandStatisticTemplateSource
                    context.rationItemSource
                    context.shipmateRationItemSink
                    context.shipmateSingleStatisticSink
                    context.shipmateStatisticTemplateSource
                    context.termNameSource
                    context.vesselSingleStatisticSource
                    context.vesselStatisticSink
                    context.vesselStatisticTemplateSource
                    context.worldSingleStatisticSource
                    commandSource 
                    messageSink 
                    world

            | Gamestate.Metrics state -> 
                Metrics.Run 
                    context.avatarMetricSource
                    messageSink 
                    state

            | Gamestate.Status state -> 
                Status.Run 
                    context.avatarJobSource
                    context.islandSingleNameSource
                    context.shipmateSingleStatisticSource
                    context.vesselSingleStatisticSource
                    messageSink 
                    state

            |> Gamestate.CheckForAvatarDeath 
                context.avatarMessageSource
                context.shipmateSingleStatisticSource

        match nextGamestate with
        | Some state ->
            Loop 
                context
                random 
                commandSource 
                messageSink 
                state

        | None ->
            ()
    
    let Run 
            (context : RunnerRunContext)
            : unit =

        Console.Title <- "Seafarers of SPLORR!!"
        let old = Console.ForegroundColor
        Console.ForegroundColor <- ConsoleColor.Gray

        None
        |> Gamestate.MainMenu
        |> Loop 
            context
            (Random()) 
            (fun () -> CommandSource.Read Console.ReadLine) 
            MessageSink.Write

        Console.ForegroundColor <- old
