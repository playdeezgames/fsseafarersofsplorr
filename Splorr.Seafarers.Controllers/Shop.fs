namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module Shop = 
    let Run (source:CommandSource) (sink:MessageSink) (location:Location) (world: World) : Gamestate option =
        [
            ""
            "You are at the shop."
        ]
        |> List.iter sink
        match source() with
        | Some Command.Dock ->
            (location, world) 
            |> Gamestate.Docked
            |> Some

        | Some Command.Status ->
            (location, world) 
            |> Gamestate.Shop
            |> Gamestate.Status
            |> Some

        | Some Command.Quit ->
            (location, world) 
            |> Gamestate.Shop
            |> Gamestate.ConfirmQuit
            |> Some

        | Some Command.Help ->
            (location, world) 
            |> Gamestate.Shop
            |> Gamestate.Help
            |> Some

        | _ -> 
            (location, world |> World.AddMessages ["Maybe try 'help'?"]) 
            |> Gamestate.Shop 
            |> Some

