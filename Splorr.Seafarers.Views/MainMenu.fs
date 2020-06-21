namespace Splorr.Seafarers.Views

open Splorr.Seafarers.Models
open Splorr.Seafarers.Controllers

module MainMenu =
    let Run (source:CommandSource) (sink:MessageSink) (world:World option) : ViewState option =
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
        | Some w, Some Resume ->
            w
            |> AtSea
            |> Some
        | Some w, Some Abandon ->
            None
            |> MainMenu
            |> Some
        | None, Some Start ->
            World.Create {MinimumIslandDistance=10.0; WorldSize=(100.0,100.0); MaximumGenerationTries=500u} (System.Random())//TODO: still hard coded!
            |> AtSea
            |> Some
        | None, Some Quit ->
            world
            |> MainMenu
            |> ConfirmQuit
            |> Some
        | _ ->
            world
            |> MainMenu
            |> Some


