namespace Splorr.Seafarers

open Splorr.Seafarers.Controllers

module Runner =
    let rec private Loop (random:System.Random) (source:CommandSource) (sink:MessageSink) (gamestate: Gamestate) : unit =
        let nextGamestate : Gamestate option = 
            match gamestate with
            | Gamestate.AtSea world -> AtSea.Run random source sink world
            | Gamestate.ConfirmQuit state -> ConfirmQuit.Run source sink state
            | Gamestate.Help state -> Help.Run sink state
            | Gamestate.MainMenu world -> MainMenu.Run source sink world
            | Gamestate.Docked (location, world) -> Docked.Run source sink location world
            | Gamestate.IslandList (page, state) -> IslandList.Run sink page state
            | Gamestate.Status state -> Status.Run sink state
        match nextGamestate with
        | Some state ->
            Loop random source sink state
        | None ->
            ()
    
    let Run () : unit =
        None
        |> Gamestate.MainMenu
        |> Loop (System.Random()) CommandSource.Read System.Console.WriteLine
