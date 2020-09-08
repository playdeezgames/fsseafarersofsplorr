namespace Splorr.Seafarers

open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Services
open System
open Splorr.Seafarers.Models

type RunnerRunContext =
    inherit AtSeaRunContext
    inherit WorldCreateContext
    inherit DockedRunContext
    inherit IslandFeatureRunContext
    inherit HelpRunContext
    inherit ItemListRunContext
    inherit CareenedRunContext
    abstract member avatarMetricSource              : AvatarMetricSource
    abstract member switchSource                    : SwitchSource

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
            | Gamestate.InPlay avatarId -> 
                match context.avatarIslandFeatureSource avatarId with
                | None ->
                    AtSea.Run 
                        context
                        random 
                        commandSource 
                        messageSink 
                        avatarId

                | Some feature when feature.featureId = IslandFeatureIdentifier.Dock -> 
                    Docked.Run 
                        context
                        commandSource 
                        messageSink 
                        feature.location 
                        avatarId

                | Some feature -> 
                    IslandFeature.Run
                        context
                        commandSource
                        messageSink
                        feature.location
                        feature.featureId
                        avatarId

                | _ ->
                    raise (System.NotImplementedException "InPlay state with unknown configuration")


            | Gamestate.Careened (side, avatarId) -> 
                Careened.Run 
                    context
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
                    avatarId

            | Gamestate.Chart (chartName, avatarId) -> 
                Chart.Run 
                    context.avatarIslandSingleMetricSource
                    context.islandSingleNameSource
                    context.islandSource 
                    context.vesselSingleStatisticSource
                    context.worldSingleStatisticSource
                    messageSink 
                    chartName 
                    avatarId

            | Gamestate.ConfirmQuit state -> 
                ConfirmQuit.Run 
                    context.switchSource 
                    commandSource 
                    messageSink 
                    state
            

            | Gamestate.ItemList (Gamestate.InPlay avatarId) -> 
                match context.avatarIslandFeatureSource avatarId with
                | Some feature ->
                    ItemList.Run 
                        context
                        context.avatarMessageSource
                        context.commoditySource 
                        context.islandItemSource 
                        context.islandMarketSource 
                        context.islandSource
                        context.itemSource 
                        context.shipmateSingleStatisticSource
                        messageSink 
                        feature.location 
                        avatarId
                | _ ->
                    raise (System.NotImplementedException "Gamestate.ItemList with unexpected inner gamestate")

            | Gamestate.ItemList _ -> 
                raise (System.NotImplementedException "Gamestate.ItemList with unexpected inner gamestate")

            | Gamestate.Jobs (Gamestate.InPlay avatarId) -> 
                match context.avatarIslandFeatureSource avatarId with
                | Some feature ->
                    Jobs.Run 
                        context.islandJobSource
                        context.islandSingleNameSource
                        context.islandSource
                        messageSink 
                        feature.location
                        avatarId
                | _ ->
                    raise (System.NotImplementedException "Gamestate.Jobs with unexpected inner gamestate")
            | Gamestate.Jobs _ -> 
                raise (System.NotImplementedException "Gamestate.Jobs with unexpected inner gamestate")

            | Gamestate.GameOver messages -> 
                GameOver.Run 
                    messageSink 
                    messages

            | Gamestate.Help state -> 
                Help.Run 
                    context
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

            | Gamestate.MainMenu avatarId -> 
                MainMenu.Run 
                    context
                    commandSource 
                    messageSink 
                    avatarId

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
