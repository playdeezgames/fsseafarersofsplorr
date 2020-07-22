namespace Splorr.Seafarers

open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Services
open System.Data.SQLite
open Splorr.Seafarers.Persistence
open Splorr.Seafarers.Models

module Runner =
    let rec private Loop (switches:Set<string>) (islandMarketSource) (islandMarketSink) (islandSingleMarketSink) (islandListSource) (islandListSink) (connection:SQLiteConnection) (random:System.Random) (configuration:WorldConfiguration) (source:CommandSource) (sink:MessageSink) (gamestate: Gamestate) : unit =
        let nextGamestate : Gamestate option = 
            match gamestate with
            | Gamestate.AtSea world -> AtSea.Run islandMarketSource islandMarketSink islandListSource islandListSink random configuration.RewardRange connection source sink world
            | Gamestate.Careened (side, world) -> Careened.Run source sink side world
            | Gamestate.ConfirmQuit state -> ConfirmQuit.Run switches source sink state
            | Gamestate.Docked (Dock, location, world) -> 
                match connection |> Commodity.GetList, connection |> Item.GetList with
                | Ok commodities, Ok items ->
                    Docked.Run islandMarketSource islandSingleMarketSink commodities items source sink location world
                | Result.Error message, _ ->
                    raise (System.InvalidOperationException message)
                | _, Result.Error message ->
                    raise (System.InvalidOperationException message)
            | Gamestate.Docked (ItemList, location, world) -> 
                match connection |> Commodity.GetList, connection |> Item.GetList with
                | Ok commodities, Ok items ->
                    ItemList.Run islandMarketSource islandListSource commodities items sink location world
                | Result.Error message, _ ->
                    raise (System.InvalidOperationException message)
                | _, Result.Error message ->
                    raise (System.InvalidOperationException message)
            | Gamestate.Docked (Jobs, location, world) -> Jobs.Run sink (location, world)
            | Gamestate.GameOver messages -> messages |> GameOver.Run sink
            | Gamestate.Help state -> Help.Run sink state
            | Gamestate.Inventory gameState -> 
                match connection |> Item.GetList with
                | Ok items ->
                    Inventory.Run items sink gameState
                | Result.Error message ->
                    raise (System.InvalidOperationException message)
            | Gamestate.IslandList (page, state) -> IslandList.Run sink page state
            | Gamestate.MainMenu world -> MainMenu.Run configuration source sink world
            | Gamestate.Metrics state -> Metrics.Run sink state
            | Gamestate.Status state -> Status.Run sink state
        match nextGamestate with
        | Some state ->
            Loop switches islandMarketSource islandMarketSink islandSingleMarketSink islandListSource islandListSink connection random configuration source sink state
        | None ->
            ()
    
    let Run (switches:Set<string>) (islandMarketSource) (islandMarketSink) (islandSingleMarketSink) (islandListSource) (islandListSink) (connection:SQLiteConnection) : unit =
        match connection |> WorldConfiguration.Get with
        | Ok configuration ->
            System.Console.Title <- "Seafarers of SPLORR!!"
            let old = System.Console.ForegroundColor
            System.Console.ForegroundColor <- System.ConsoleColor.Gray
            None
            |> Gamestate.MainMenu
            |> Loop switches islandMarketSource islandMarketSink islandSingleMarketSink islandListSource islandListSink connection (System.Random()) configuration (fun () -> CommandSource.Read System.Console.ReadLine) MessageSink.Write
            System.Console.ForegroundColor <- old
        | Result.Error message ->
            raise (System.InvalidOperationException message)
