namespace Splorr.Seafarers

open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Services
open System.Data.SQLite
open Splorr.Seafarers.Persistence
open Splorr.Seafarers.Models

module Runner =
    let rec private Loop (connection:SQLiteConnection) (random:System.Random) (configuration:WorldConfiguration) (source:CommandSource) (sink:MessageSink) (avatarId:string) (gamestate: Gamestate) : unit =
        let nextGamestate : Gamestate option = 
            match gamestate with
            | Gamestate.AtSea world -> AtSea.Run random connection source sink avatarId world
            | Gamestate.Careened (side, world) -> Careened.Run source sink side avatarId world
            | Gamestate.ConfirmQuit state -> ConfirmQuit.Run source sink state
            | Gamestate.Docked (Dock, location, world) -> 
                match connection |> Commodity.GetList, connection |> Item.GetList with
                | Ok commodities, Ok items ->
                    Docked.Run commodities items source sink location avatarId world
                | Result.Error message, _ ->
                    raise (System.InvalidOperationException message)
                | _, Result.Error message ->
                    raise (System.InvalidOperationException message)
            | Gamestate.Docked (ItemList, location, world) -> 
                match connection |> Commodity.GetList, connection |> Item.GetList with
                | Ok commodities, Ok items ->
                    ItemList.Run commodities items sink location avatarId world
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
                    Inventory.Run items sink avatarId gameState
                | Result.Error message ->
                    raise (System.InvalidOperationException message)
            | Gamestate.IslandList (page, state) -> IslandList.Run sink page avatarId state
            | Gamestate.MainMenu world -> MainMenu.Run configuration source sink world
            | Gamestate.Metrics state -> Metrics.Run sink avatarId state
            | Gamestate.Status state -> Status.Run sink avatarId state
        match nextGamestate with
        | Some state ->
            Loop connection random configuration source sink avatarId state
        | None ->
            ()
    
    let Run (connection:SQLiteConnection) (avatarId:string) : unit =
        match connection |> WorldConfiguration.Get with
        | Ok configuration ->
            System.Console.Title <- "Seafarers of Splorr!!"
            let old = System.Console.ForegroundColor
            System.Console.ForegroundColor <- System.ConsoleColor.Gray
            None
            |> Gamestate.MainMenu
            |> Loop connection (System.Random()) configuration CommandSource.Read MessageSink.Write avatarId
            System.Console.ForegroundColor <- old
        | Result.Error message ->
            raise (System.InvalidOperationException message)
