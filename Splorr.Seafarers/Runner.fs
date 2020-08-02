namespace Splorr.Seafarers

open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open System

module Runner =


    let rec private Loop 
            (switches                      : Set<string>) 
            (commoditySource               : unit     -> Map<uint64, CommodityDescriptor>)
            (itemSource                    : unit     -> Map<uint64, ItemDescriptor>)
            (islandMarketSource            : Location -> Map<uint64, Market>) 
            (islandSingleMarketSource      : Location -> uint64 -> Market option)
            (islandMarketSink              : Location -> Map<uint64, Market> -> unit) 
            (islandSingleMarketSink        : Location -> uint64 * Market -> unit) 
            (islandListSource              : Location -> Set<uint64>) 
            (islandListSink                : Location -> Set<uint64> -> unit) 
            (vesselStatisticTemplateSource : unit -> Map<VesselStatisticIdentifier, VesselStatisticTemplate>)
            (vesselStatisticSink           : string -> Map<VesselStatisticIdentifier, Statistic> -> unit)
            (vesselSingleStatisticSource   : string->VesselStatisticIdentifier->Statistic option)
            (vesselSingleStatisticSink     : string->VesselStatisticIdentifier*Statistic->unit)
            (random                        : Random) 
            (configurationSource           : unit -> WorldConfiguration) 
            (commandSource                 : CommandSource) 
            (messageSink                   : MessageSink) 
            (gamestate                     : Gamestate) 
            : unit =

        let nextGamestate : Gamestate option = 
            match gamestate with
            | Gamestate.AtSea world -> 
                AtSea.Run 
                    commoditySource 
                    itemSource 
                    islandMarketSource
                    islandMarketSink 
                    islandListSource 
                    islandListSink 
                    vesselSingleStatisticSource
                    vesselSingleStatisticSink
                    random 
                    (configurationSource()).RewardRange 
                    commandSource 
                    messageSink 
                    world

            | Gamestate.Careened (side, world) -> 
                Careened.Run 
                    vesselSingleStatisticSource
                    vesselSingleStatisticSink
                    commandSource 
                    messageSink 
                    side 
                    world

            | Gamestate.Chart (chartName, world) -> 
                Chart.Run 
                    (configurationSource()).WorldSize 
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
                    commandSource 
                    messageSink 
                    location 
                    world

            | Gamestate.Docked (ItemList, location, world) -> 
                ItemList.Run 
                    commoditySource 
                    itemSource 
                    islandMarketSource 
                    islandListSource 
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
                    messageSink 
                    page 
                    state

            | Gamestate.MainMenu world -> 
                MainMenu.Run 
                    vesselStatisticTemplateSource
                    vesselStatisticSink
                    (configurationSource()) 
                    commandSource 
                    messageSink 
                    world

            | Gamestate.Metrics state -> 
                Metrics.Run 
                    messageSink 
                    state

            | Gamestate.Status state -> 
                Status.Run 
                    vesselSingleStatisticSource
                    messageSink 
                    state

            |> Gamestate.CheckForAvatarDeath

        match nextGamestate with
        | Some state ->
            Loop 
                switches 
                commoditySource 
                itemSource 
                islandMarketSource 
                islandSingleMarketSource
                islandMarketSink 
                islandSingleMarketSink 
                islandListSource 
                islandListSink 
                vesselStatisticTemplateSource
                vesselStatisticSink
                vesselSingleStatisticSource
                vesselSingleStatisticSink
                random 
                configurationSource 
                commandSource 
                messageSink 
                state

        | None ->
            ()
    
    let Run 
            (switches                      : Set<string>) 
            (configurationSource           : unit     -> WorldConfiguration) 
            (commoditySource               : unit     -> Map<uint64, CommodityDescriptor>) 
            (itemSource                    : unit     -> Map<uint64, ItemDescriptor>) 
            (islandMarketSource            : Location -> Map<uint64, Market>) 
            (islandSingleMarketSource      : Location -> uint64 -> Market option) 
            (islandMarketSink              : Location -> Map<uint64, Market> -> unit) 
            (islandSingleMarketSink        : Location -> uint64 * Market     -> unit) 
            (islandListSource              : Location -> Set<uint64>) 
            (islandListSink                : Location -> Set<uint64>->unit) 
            (vesselStatisticTemplateSource : unit -> Map<VesselStatisticIdentifier, VesselStatisticTemplate>)
            (vesselStatisticSink           : string -> Map<VesselStatisticIdentifier, Statistic> -> unit)
            (vesselSingleStatisticSource   : string->VesselStatisticIdentifier->Statistic option)
            (vesselSingleStatisticSink     : string->VesselStatisticIdentifier*Statistic->unit)
            : unit =

        Console.Title <- "Seafarers of SPLORR!!"
        let old = Console.ForegroundColor
        Console.ForegroundColor <- ConsoleColor.Gray

        None
        |> Gamestate.MainMenu
        |> Loop 
            switches 
            commoditySource 
            itemSource 
            islandMarketSource 
            islandSingleMarketSource
            islandMarketSink 
            islandSingleMarketSink 
            islandListSource 
            islandListSink 
            vesselStatisticTemplateSource
            vesselStatisticSink
            vesselSingleStatisticSource
            vesselSingleStatisticSink
            (Random()) 
            configurationSource
            (fun () -> CommandSource.Read Console.ReadLine) 
            MessageSink.Write

        Console.ForegroundColor <- old
