namespace Splorr.Seafarers

open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Services
open System
open Splorr.Seafarers.Models

type RunnerRunContext =
    inherit StatusRunContext
    inherit AtSeaRunContext
    inherit WorldCreateContext
    inherit DockedRunContext
    inherit IslandFeatureRunContext
    inherit HelpRunContext
    inherit ItemListRunContext
    inherit CareenedRunContext
    inherit GamestateCheckForAvatarDeathContext
    inherit ChartRunContext
    inherit IslandListRunContext
    inherit InventoryRunContext
    abstract member avatarMetricSource : AvatarMetricSource
    abstract member switchSource : SwitchSource

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
                    raise (NotImplementedException "InPlay state with unknown configuration")


            | Gamestate.Careened (side, avatarId) -> 
                Careened.Run 
                    context
                    context.avatarMessageSource
                    commandSource 
                    messageSink 
                    side 
                    avatarId

            | Gamestate.Chart (chartName, avatarId) -> 
                Chart.Run 
                    context
                    context.avatarIslandSingleMetricSource
                    context.islandSingleNameSource
                    context.islandSource 
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
                        context.islandItemSource 
                        context.islandSource
                        context.itemSource 
                        messageSink 
                        feature.location 
                        avatarId
                | _ ->
                    raise (NotImplementedException "Gamestate.ItemList with unexpected inner gamestate")

            | Gamestate.ItemList _ -> 
                raise (NotImplementedException "Gamestate.ItemList with unexpected inner gamestate")

            | Gamestate.Jobs (Gamestate.InPlay avatarId) -> 
                match context.avatarIslandFeatureSource avatarId with
                | Some feature ->
                    Jobs.Run 
                        context
                        messageSink 
                        feature.location
                        avatarId
                | _ ->
                    raise (NotImplementedException "Gamestate.Jobs with unexpected inner gamestate")
            | Gamestate.Jobs _ -> 
                raise (NotImplementedException "Gamestate.Jobs with unexpected inner gamestate")

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
                    context
                    context.avatarInventorySource
                    context.itemSource 
                    context.vesselSingleStatisticSource
                    messageSink 
                    gameState

            | Gamestate.IslandList (page, state) -> 
                IslandList.Run 
                    context
                    context.avatarIslandSingleMetricSource
                    context.islandSingleNameSource
                    context.islandSource
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
                    context
                    context.avatarJobSource
                    context.islandSingleNameSource
                    context.shipmateSingleStatisticSource
                    context.vesselSingleStatisticSource
                    messageSink 
                    state

            |> Gamestate.CheckForAvatarDeath 
                context
                context.avatarMessageSource

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
