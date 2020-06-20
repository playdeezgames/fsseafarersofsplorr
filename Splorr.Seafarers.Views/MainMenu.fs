namespace Splorr.Seafarers.Views

open Splorr.Seafarers.Models

module MainMenu =
    let Run (source:CommandSource) (sink:MessageSink) (world:World option) : ViewState option =
        [
            ""
            "Main Menu Commands:"
        ]
        |> List.iter sink
        if world.IsSome then
            []
        else
            [
                "\tquit - quits the game"
            ]
        |> List.iter sink
        match world, source() with
        | None, Some Quit ->
            world
            |> MainMenu
            |> ConfirmQuit
            |> Some
        | _ ->
            world
            |> MainMenu
            |> Some


