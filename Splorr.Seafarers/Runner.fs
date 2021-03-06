namespace Splorr.Seafarers

open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Services
open System
open Splorr.Seafarers.Models

module Runner =
    let rec private Loop 
            (context       : ServiceContext)
            (commandSource : CommandSource) 
            (messageSink   : MessageSink) 
            (gamestate     : Gamestate) 
            : unit =
        let nextGamestate : Gamestate option = 
            match gamestate with
            | Gamestate.InPlay avatarId -> 
                match AvatarIslandFeature.Get context avatarId with
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
                    commandSource 
                    messageSink 
                    side 
                    avatarId

            | Gamestate.Chart (chartName, avatarId) -> 
                Chart.Run 
                    context
                    messageSink 
                    chartName 
                    avatarId

            | Gamestate.ConfirmQuit state -> 
                ConfirmQuit.Run 
                    context 
                    commandSource 
                    messageSink 
                    state

            | Gamestate.ItemList (Gamestate.InPlay avatarId) -> 
                match AvatarIslandFeature.Get context avatarId with
                | Some feature ->
                    ItemList.Run 
                        context
                        messageSink 
                        feature.location 
                        avatarId
                | _ ->
                    raise (NotImplementedException "Gamestate.ItemList with unexpected inner gamestate")

            | Gamestate.ItemList _ -> 
                raise (NotImplementedException "Gamestate.ItemList with unexpected inner gamestate")

            | Gamestate.Jobs (Gamestate.InPlay avatarId) -> 
                match AvatarIslandFeature.Get context avatarId with
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
                    messageSink 
                    gameState

            | Gamestate.IslandList (page, state) -> 
                IslandList.Run 
                    context
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
                    context
                    messageSink 
                    state

            | Gamestate.Status state -> 
                Status.Run 
                    context
                    messageSink 
                    state

            |> Gamestate.CheckForAvatarDeath 
                context

        match nextGamestate with
        | Some state ->
            Loop 
                context
                commandSource 
                messageSink 
                state

        | None ->
            ()
    
    let Run 
            (context : ServiceContext)
            : unit =
        Console.Title <- "Seafarers of SPLORR!!"
        let old = Console.ForegroundColor
        Console.ForegroundColor <- ConsoleColor.Gray

        None
        |> Gamestate.MainMenu
        |> Loop 
            context
            (fun () -> CommandSource.Read Console.ReadLine) 
            MessageSink.Write

        Console.ForegroundColor <- old
