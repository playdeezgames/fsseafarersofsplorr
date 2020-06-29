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
            | Gamestate.Docked (location, world) -> Docked.Run source sink location world
            | Gamestate.IslandList (page, state) -> IslandList.Run sink page state
            | Gamestate.Status state -> Status.Run sink state
            | Gamestate.Jobs (location, world) -> Jobs.Run sink (location, world)
            | Gamestate.PriceList (location, world) -> PriceList.Run sink location world
            | Gamestate.Shop (location, world) -> Shop.Run source sink location world
        match nextGamestate with
        | Some state ->
            Loop random configuration source sink state
        | None ->
            ()
    
    let Run (configuration:WorldGenerationConfiguration) : unit =
        None
        |> Gamestate.MainMenu
        |> Loop (System.Random()) configuration CommandSource.Read System.Console.WriteLine
