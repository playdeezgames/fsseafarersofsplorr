namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module MainMenu =
    let Run (configuration:WorldConfiguration) (source:CommandSource) (sink:MessageSink) (world:World option) : Gamestate option =
        [
            "" |> Line
            (Hue.Heading, "Main Menu Commands:" |> Line) |> Hued
        ]
        |> List.iter sink

        if world.IsSome then
            [
                (Hue.Label, "resume" |> Text) |> Hued
                (Hue.Usage, " - resume game" |> Line) |> Hued
                (Hue.Label, "abandon game" |> Text) |> Hued
                (Hue.Usage, " - abandon game" |> Line) |> Hued
            ]
        else
            [
                (Hue.Label, "start" |> Text) |> Hued
                (Hue.Usage, " - starts a new world" |> Line) |> Hued
                (Hue.Label, "quit" |> Text) |> Hued
                (Hue.Usage, " - quits the game" |> Line) |> Hued
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

        | None, Some (Command.Start avatarId)->
            World.Create 
                configuration
                (System.Random())
                avatarId
            |> Gamestate.AtSea
            |> Some

        | None, Some Command.Quit ->
            world
            |> Gamestate.MainMenu
            |> Gamestate.ConfirmQuit
            |> Some

        | _ ->
            [
                (Hue.Error, "Invalid command." |> Line) |> Hued
            ]
            |> List.iter sink
            world
            |> Gamestate.MainMenu
            |> Some


