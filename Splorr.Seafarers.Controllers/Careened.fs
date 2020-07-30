namespace Splorr.Seafarers.Controllers

open System
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module Careened = 
    let private UpdateDisplay 
            (messageSink:MessageSink) 
            (side:Side)
            (world:World) : unit =
        let avatarId = world.AvatarId
        "" |> Line |> messageSink
        world.Avatars.[avatarId].Messages
        |> Utility.DumpMessages messageSink
        let world =
            world
            |> World.ClearMessages avatarId
        let sideName =
            match side with
            | Port -> "port"
            | Starboard -> "starboard"
        let currentValue, maximumValue =
            world.Avatars.[avatarId].Vessel.Fouling
            |> Map.fold (fun (c,m) k v -> (c+v.CurrentValue,m+v.MaximumValue)) (0.0,0.0)
        let foulage =
            100.0 * currentValue / maximumValue
        [
            (Hue.Heading, sideName |> sprintf "You are careened on the %s side." |> Line) |> Hued
            (Hue.Flavor, foulage |> sprintf "The hull is %.0f%% fouled." |> Line) |> Hued
        ]
        |> List.iter messageSink

    let private HandleCommand (command:Command option) (sink:MessageSink) (side:Side) (world:World) : Gamestate option =
        match command with
        | Some Command.Metrics ->
            (side, world)
            |> Gamestate.Careened
            |> Gamestate.Metrics
            |> Some
        | Some Command.Inventory ->
            (side, world)
            |> Gamestate.Careened
            |> Gamestate.Inventory
            |> Some
        | Some Command.Status ->
            (side, world)
            |> Gamestate.Careened
            |> Gamestate.Status
            |> Some
        | Some Command.Quit ->
            (side, world)
            |> Gamestate.Careened
            |> Gamestate.ConfirmQuit
            |> Some
        | Some Command.Help ->
            (side, world)
            |> Gamestate.Careened
            |> Gamestate.Help
            |> Some
        | Some Command.CleanHull ->
            (side, world 
            |> World.CleanHull world.AvatarId (if side=Port then Starboard else Port))
            |> Gamestate.Careened
            |> Some
        | Some Command.WeighAnchor ->
            world
            |> Gamestate.AtSea
            |> Some
        | _ ->
            ("Maybe try 'help'?",(side, world)
            |> Gamestate.Careened)
            |> Gamestate.ErrorMessage
            |> Some

    let private RunAlive (source:CommandSource) (sink:MessageSink) (side:Side) (world:World) : Gamestate option =
        UpdateDisplay 
            sink 
            side 
            world
        HandleCommand
            (source())
            sink
            side
            world

    let Run (commandSource:CommandSource) (messageSink:MessageSink) (side:Side) (world:World) : Gamestate option =
        if world |> World.IsAvatarAlive world.AvatarId then
            RunAlive commandSource messageSink side world
        else
            world.Avatars.[world.AvatarId].Messages
            |> Gamestate.GameOver
            |> Some