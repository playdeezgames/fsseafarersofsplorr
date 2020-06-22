﻿namespace Splorr.Seafarers

open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Services

module Runner =
    let rec private Loop (source:CommandSource) (sink:MessageSink) (Gamestate: Gamestate) : unit =
        let nextGamestate : Gamestate option = 
            match Gamestate with
            | AtSea world -> AtSea.Run source sink world
            | ConfirmQuit state -> ConfirmQuit.Run source sink state
            | Help state -> Help.Run sink state
            | MainMenu world -> MainMenu.Run source sink world
            | Docked (location, world) -> Docked.Run source sink location world
        match nextGamestate with
        | Some state ->
            Loop source sink state
        | None ->
            ()
    
    let Run () : unit =
        None
        |> MainMenu
        |> Loop CommandSource.Read System.Console.WriteLine
