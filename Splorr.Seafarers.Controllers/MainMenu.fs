namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module MainMenu =
    let Run (source:CommandSource) (sink:MessageSink) (world:World option) : Gamestate option =
        [
            ""
            "Main Menu Commands:"
        ]
        |> List.iter sink
        if world.IsSome then
            [
                "\tresume - resume game"
                "\tabandon - abandon game"
            ]
        else
            [
                "\tstart - starts a new world"
                "\tquit - quits the game"
            ]
        |> List.iter sink
        match world, source() with
        | Some w, Some Command.Resume ->
            w
            |> Gamestate.AtSea
            |> Some
        | Some w, Some Command.Abandon ->
            None
            |> Gamestate.MainMenu
            |> Some
        | None, Some Command.Start ->
            World.Create 
                {
                    MinimumIslandDistance=10.0
                    WorldSize=(100.0,100.0)
                    MaximumGenerationTries=500u
                    RewardRange=(0.0,0.0)
                } (System.Random())//TODO: still hard coded!
            |> Gamestate.AtSea
            |> Some
        | None, Some Command.Quit ->
            world
            |> Gamestate.MainMenu
            |> Gamestate.ConfirmQuit
            |> Some
        | _ ->
            world
            |> Gamestate.MainMenu
            |> Some


