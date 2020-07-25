﻿namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open Splorr.Seafarers.Persistence

module AtSea =
    let private RunAlive (commoditySource:unit -> Map<uint64, CommodityDescriptor>) (itemSource:unit -> Map<uint64, ItemDescriptor>) (islandMarketSource) (islandMarketSink) (islandItemSource) (islandItemSink) (random:System.Random) (rewardRange:float*float) (source:CommandSource) (sink:MessageSink) (avatarId:string) (world:World) : Gamestate option =

        "" |> Line |> sink
        world.Avatars.[avatarId].Messages
        |> Utility.DumpMessages sink
        let world =
            world
            |> World.ClearMessages avatarId
        let avatar = world.Avatars.[avatarId]
        let speedHue =
            if avatar.Speed >= 0.9 then
                Hue.Value
            elif avatar.Speed>=0.4 then
                Hue.Warning
            else
                Hue.Error
        [
            (Hue.Heading, "At Sea:" |> Line) |> Hued
            (Hue.Label, "Turn: " |> Text) |> Hued
            (Hue.Value, avatar.Shipmates.[0].Statistics.[AvatarStatisticIdentifier.Turn].CurrentValue |> sprintf "%.0f" |> Text) |> Hued
            (Hue.Value, avatar.Shipmates.[0].Statistics.[AvatarStatisticIdentifier.Turn].MaximumValue |> sprintf "/%.0f" |> Line) |> Hued
            (Hue.Label, "Heading: " |> Text) |> Hued
            (Hue.Value, avatar.Heading |> Dms.ToDms |> Dms.ToString |> sprintf "%s" |> Line) |> Hued
            (Hue.Label, "Speed: " |> Text) |> Hued
            (speedHue, (avatar.Speed * 100.0) |> sprintf "%.0f%%" |> Text) |> Hued
            avatar |> Avatar.GetEffectiveSpeed |> sprintf "(Effective rate: %.2f)" |> Line
            (Hue.Subheading, "Nearby:" |> Line) |> Hued
        ]
        |> List.iter sink

        let canCareen = 
            world
            |> World.GetNearbyLocations avatar.Position avatar.ViewDistance
            |> List.map (fun l -> (l,world.Islands.[l].CareenDistance))
            |> List.exists (fun (l,d) -> Location.DistanceTo l avatar.Position < d)

        let dockTarget = 
            world
            |> World.GetNearbyLocations avatar.Position avatar.ViewDistance
            |> List.map
                (fun location -> 
                    (location, Location.HeadingTo avatar.Position location |> Dms.ToDms |> Dms.ToString, Location.DistanceTo avatar.Position location, (world.Islands.[location] |> Island.GetDisplayName avatarId)))
            |> List.sortBy (fun (_,_,d,_)->d)
            |> List.fold
                (fun target (location, heading, distance, name) -> 
                    (Hue.Sublabel, "Name: " |> Text) |> Hued |> sink
                    (Hue.Value, sprintf "%s " (if name="" then "????" else name) |> Text) |> Hued |> sink
                    (Hue.Sublabel, "Bearing: " |> Text) |> Hued |> sink
                    (Hue.Value, sprintf "%s " heading |> Text) |> Hued |> sink
                    (Hue.Sublabel, "Distance: " |> Text) |> Hued |> sink
                    (Hue.Value, sprintf "%f" distance |> Text) |> Hued |> sink
                    (Hue.Flavor, sprintf "%s" (if distance<avatar.DockDistance then " (Can Dock)" else "") |> Line) |> Hued |> sink
                    (if distance<avatar.DockDistance then (Some location) else target)) None

        match source() with
        | Some Command.Status ->
            world 
            |> Gamestate.AtSea
            |> Gamestate.Status
            |> Some

        | Some (Command.HeadFor name) ->
            world
            |> World.HeadFor name
            |> Gamestate.AtSea
            |> Some

        | Some (Command.DistanceTo name) ->
            world
            |> World.DistanceTo name
            |> Gamestate.AtSea
            |> Some

        | Some (Command.Careen side) ->
            if canCareen then
                (side, world)
                |> Gamestate.Careened
                |> Some
            else
                world
                |> World.AddMessages avatarId [ "You cannot careen here." ]
                |> Gamestate.AtSea
                |> Some

        | Some Command.Dock ->
            match dockTarget with
            | Some location ->
                (Dock, location, world |> World.Dock islandMarketSource islandMarketSink islandItemSource islandItemSink random rewardRange (commoditySource()) (itemSource()) location avatarId)
                |> Gamestate.Docked
                |> Some
            | None ->
                world
                |> World.AddMessages avatarId [ "There is no place to dock." ]
                |> Gamestate.AtSea
                |> Some

        | Some (Command.Islands page) ->
            (page, world |> Gamestate.AtSea)
            |> Gamestate.IslandList
            |> Some

        | Some (Command.Abandon Job) ->
            world
            |> World.AbandonJob avatarId
            |> Gamestate.AtSea
            |> Some

        | Some Command.Metrics ->
            world 
            |> Gamestate.AtSea
            |> Gamestate.Metrics
            |> Some

        | Some (Command.Chart x) ->
            (x, world)
            |> Gamestate.Chart
            |> Some

        | Some Command.Menu ->
            world
            |> Some
            |> Gamestate.MainMenu
            |> Some

        | Some Command.Help ->
            world
            |> Gamestate.AtSea
            |> Gamestate.Help
            |> Some

        | Some (Command.Move distance)->
            world
            |> World.Move distance avatarId
            |> Gamestate.AtSea
            |> Some

        | Some (Command.Set (SetCommand.Heading heading)) ->
            world
            |> World.SetHeading heading avatarId
            |> Gamestate.AtSea
            |> Some

        | Some (Command.Set (Speed speed)) ->
            world
            |> World.SetSpeed speed avatarId
            |> Gamestate.AtSea
            |> Some

        | Some Command.Quit -> 
            world
            |> Gamestate.AtSea
            |> Gamestate.ConfirmQuit
            |> Some

        | Some Command.Inventory -> 
            world
            |> Gamestate.AtSea
            |> Gamestate.Inventory
            |> Some

        | _ ->
            (Hue.Error, "Maybe try 'help'?" |> Line) |> Hued |> sink
            world
            |> Gamestate.AtSea
            |> Some

    let Run 
            (commoditySource:unit -> Map<uint64, CommodityDescriptor>) 
            (itemSource:unit -> Map<uint64, ItemDescriptor>) 
            (islandMarketSource) 
            (islandMarketSink) 
            (islandItemSource) 
            (islandItemSink) 
            (random:System.Random) 
            (rewardRange:float*float) 
            (source:CommandSource) 
            (sink:MessageSink) 
            (world:World) 
            : Gamestate option =
        if world |> World.IsAvatarAlive world.AvatarId then
            RunAlive commoditySource itemSource islandMarketSource islandMarketSink islandItemSource islandItemSink random rewardRange source sink world.AvatarId world
        else
            world.Avatars.[world.AvatarId].Messages
            |> Gamestate.GameOver
            |> Some
