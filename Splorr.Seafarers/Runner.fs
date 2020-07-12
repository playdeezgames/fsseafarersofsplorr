namespace Splorr.Seafarers

open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Services
open System.Data.SQLite

module Runner =
    let rec private Loop (connection:SQLiteConnection) (random:System.Random) (configuration:WorldGenerationConfiguration) (source:CommandSource) (sink:MessageSink) (avatarId:string) (gamestate: Gamestate) : unit =
        let nextGamestate : Gamestate option = 
            match gamestate with
            | Gamestate.AtSea world -> AtSea.Run random source sink avatarId world
            | Gamestate.ConfirmQuit state -> ConfirmQuit.Run source sink state
            | Gamestate.Docked (Dock, location, world) -> Docked.Run source sink location avatarId world
            | Gamestate.Docked (ItemList, location, world) -> ItemList.Run sink location avatarId world
            | Gamestate.Docked (Jobs, location, world) -> Jobs.Run sink (location, world)
            | Gamestate.GameOver messages -> messages |> GameOver.Run sink
            | Gamestate.Help state -> Help.Run sink state
            | Gamestate.Inventory gameState -> Inventory.Run sink avatarId gameState
            | Gamestate.IslandList (page, state) -> IslandList.Run sink page avatarId state
            | Gamestate.MainMenu world -> MainMenu.Run configuration source sink world
            | Gamestate.Status state -> Status.Run sink avatarId state
            | Gamestate.SaveGame (name, world) -> SaveGame.Run connection sink name world
        match nextGamestate with
        | Some state ->
            Loop connection random configuration source sink avatarId state
        | None ->
            ()
    
    let Run (connection:SQLiteConnection) (configuration:WorldGenerationConfiguration) (avatarId:string) : unit =
        System.Console.Title <- "Seafarers of Splorr!!"
        let old = System.Console.ForegroundColor
        System.Console.ForegroundColor <- System.ConsoleColor.Gray
        None
        |> Gamestate.MainMenu
        |> Loop connection (System.Random()) configuration CommandSource.Read MessageSink.Write avatarId
        System.Console.ForegroundColor <- old
