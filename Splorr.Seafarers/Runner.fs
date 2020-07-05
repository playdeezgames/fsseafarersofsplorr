namespace Splorr.Seafarers

open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Services

module Runner =
    let rec private Loop (random:System.Random) (configuration:WorldGenerationConfiguration) (source:CommandSource) (sink:MessageSink) (gamestate: Gamestate) : unit =
        let nextGamestate : Gamestate option = 
            match gamestate with
            | Gamestate.AtSea world -> AtSea.Run random source sink world
            | Gamestate.ConfirmQuit state -> ConfirmQuit.Run source sink state
            | Gamestate.Help state -> Help.Run sink state
            | Gamestate.MainMenu world -> MainMenu.Run configuration source sink world
            | Gamestate.Docked (Dock, location, world) -> Docked.Run source sink location world
            | Gamestate.IslandList (page, state) -> IslandList.Run sink page state
            | Gamestate.Status state -> Status.Run sink state
            | Gamestate.Docked (Jobs, location, world) -> Jobs.Run sink (location, world)
            | Gamestate.Docked (PriceList, location, world) -> PriceList.Run sink location world
            | Gamestate.Docked (Shop, location, world) -> Shop.Run source sink location world
            | Gamestate.Docked (ItemList, location, world) -> ItemList.Run sink location world
            | Gamestate.Inventory gameState -> Inventory.Run sink gameState
        match nextGamestate with
        | Some state ->
            Loop random configuration source sink state
        | None ->
            ()
    
    let Run (configuration:WorldGenerationConfiguration) : unit =
        let old = System.Console.ForegroundColor
        System.Console.ForegroundColor <- System.ConsoleColor.Gray
        None
        |> Gamestate.MainMenu
        |> Loop (System.Random()) configuration CommandSource.Read MessageSink.Write
        System.Console.ForegroundColor <- old
