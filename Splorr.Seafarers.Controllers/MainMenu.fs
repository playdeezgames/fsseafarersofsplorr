namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module MainMenu =
    let Run (configuration:WorldGenerationConfiguration) (source:CommandSource) (sink:MessageSink) (world:World option) : Gamestate option =
        [
            "" |> Line
            (Heading, "Main Menu Commands:" |> Line) |> Hued
        ]
        |> List.iter sink

        if world.IsSome then
            [
                (Label, "resume" |> Text) |> Hued
                (Usage, " - resume game" |> Line) |> Hued
                (Label, "save (save name)" |> Text) |> Hued
                (Usage, " - save game" |> Line) |> Hued
                (Label, "abandon game" |> Text) |> Hued
                (Usage, " - abandon game" |> Line) |> Hued
            ]
        else
            [
                (Label, "start" |> Text) |> Hued
                (Usage, " - starts a new world" |> Line) |> Hued
                (Label, "quit" |> Text) |> Hued
                (Usage, " - quits the game" |> Line) |> Hued
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
                configuration
                (System.Random())//TODO: still hard coded!
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


