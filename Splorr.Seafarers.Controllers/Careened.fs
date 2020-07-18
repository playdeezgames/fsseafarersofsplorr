namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module Careened = 
    let private RunAlive (source:CommandSource) (sink:MessageSink) (side:Side) (avatarId:string) (world:World) : Gamestate option =
        "" |> Line |> sink
        world.Avatars.[avatarId].Messages
        |> Utility.DumpMessages sink
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
            (Heading, sideName |> sprintf "You are careened on the %s side." |> Line) |> Hued
            (Flavor, foulage |> sprintf "The hull is %.0f%% fouled." |> Line) |> Hued
        ]
        |> List.iter sink
        match source() with
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
            (side, world |> World.CleanHull avatarId (if side=Port then Starboard else Port))
            |> Gamestate.Careened
            |> Some
        | Some Command.WeighAnchor ->
            world
            |> Gamestate.AtSea
            |> Some
        | _ ->
            (Error, "Maybe try 'help'?" |> Line) |> Hued |> sink
            (side, world)
            |> Gamestate.Careened
            |> Some

    let Run (source:CommandSource) (sink:MessageSink) (side:Side) (avatarId:string) (world:World) : Gamestate option =
        if world |> World.IsAvatarAlive avatarId then
            RunAlive source sink side avatarId world
        else
            world.Avatars.[avatarId].Messages
            |> Gamestate.GameOver
            |> Some