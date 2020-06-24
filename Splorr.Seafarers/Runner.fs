namespace Splorr.Seafarers

open Splorr.Seafarers.Controllers

module Runner =
    let rec private Loop (random:System.Random) (source:CommandSource) (sink:MessageSink) (gamestate: Gamestate) : unit =
        let nextGamestate : Gamestate option = 
            match gamestate with
            | AtSea world -> AtSea.Run random source sink world
            | ConfirmQuit state -> ConfirmQuit.Run source sink state
            | Help state -> Help.Run sink state
            | MainMenu world -> MainMenu.Run source sink world
            | Docked (location, world) -> Docked.Run source sink location world
            | IslandList (page, state) -> IslandList.Run sink page state
            | Status state -> Status.Run sink state
        match nextGamestate with
        | Some state ->
            Loop random source sink state
        | None ->
            ()
    
    let Run () : unit =
        None
        |> MainMenu
        |> Loop (System.Random()) CommandSource.Read System.Console.WriteLine
