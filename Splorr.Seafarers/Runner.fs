namespace Splorr.Seafarers

open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Models
open System

module Runner =
    let rec private Loop 
            (switches               : Set<string>) 
            (commoditySource        : unit     -> Map<uint64, CommodityDescriptor>)
            (itemSource             : unit     -> Map<uint64, ItemDescriptor>)
            (islandMarketSource     : Location -> Map<uint64, Market>) 
            (islandMarketSink       : Location -> Map<uint64, Market> -> unit) 
            (islandSingleMarketSink : Location -> uint64 * Market -> unit) 
            (islandListSource       : Location -> Set<uint64>) 
            (islandListSink         : Location -> Set<uint64> -> unit) 
            (random                 : Random) 
            (configurationSource    : unit -> WorldConfiguration) 
            (commandSource          : CommandSource) 
            (messageSink            : MessageSink) 
            (gamestate              : Gamestate) 
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
                    random 
                    (configurationSource()).RewardRange 
                    commandSource 
                    messageSink 
                    world

            | Gamestate.Careened (side, world) -> 
                Careened.Run 
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
                    islandMarketSource 
                    islandSingleMarketSink 
                    commoditySource 
                    itemSource 
                    commandSource 
                    messageSink 
                    location 
                    world

            | Gamestate.Docked (ItemList, location, world) -> 
                ItemList.Run 
                    islandMarketSource 
                    islandListSource 
                    commoditySource 
                    itemSource 
                    messageSink 
                    location 
                    world

            | Gamestate.Docked (Jobs, location, world) -> 
                Jobs.Run 
                    messageSink 
                    (location, world)

            | Gamestate.GameOver messages -> 
                GameOver.Run 
                    messageSink 
                    messages

            | Gamestate.Help state -> 
                Help.Run 
                    messageSink 
                    state

            | Gamestate.Inventory gameState -> 
                Inventory.Run 
                    itemSource 
                    messageSink 
                    gameState

            | Gamestate.IslandList (page, state) -> 
                IslandList.Run 
                    messageSink 
                    page 
                    state

            | Gamestate.MainMenu world -> 
                MainMenu.Run 
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
                    messageSink 
                    state

        match nextGamestate with
        | Some state ->
            Loop 
                switches 
                commoditySource 
                itemSource 
                islandMarketSource 
                islandMarketSink 
                islandSingleMarketSink 
                islandListSource 
                islandListSink 
                random 
                configurationSource 
                commandSource 
                messageSink 
                state

        | None ->
            ()
    
    let Run 
            (switches               : Set<string>) 
            (configurationSource    : unit     -> WorldConfiguration) 
            (commoditySource        : unit     -> Map<uint64, CommodityDescriptor>) 
            (itemSource             : unit     -> Map<uint64, ItemDescriptor>) 
            (islandMarketSource     : Location -> Map<uint64, Market>) 
            (islandMarketSink       : Location -> Map<uint64, Market> -> unit) 
            (islandSingleMarketSink : Location -> uint64 * Market     -> unit) 
            (islandListSource       : Location -> Set<uint64>) 
            (islandListSink         : Location -> Set<uint64>->unit) 
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
            islandMarketSink 
            islandSingleMarketSink 
            islandListSource 
            islandListSink 
            (Random()) 
            configurationSource
            (fun () -> CommandSource.Read Console.ReadLine) 
            MessageSink.Write

        Console.ForegroundColor <- old
