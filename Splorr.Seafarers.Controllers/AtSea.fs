namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open System.Data.SQLite
open Splorr.Seafarers.Persistence

module AtSea =
    let private RunAlive (islandItemSource) (islandItemSink) (random:System.Random) (rewardRange:float*float) (commodities:Map<uint64, CommodityDescriptor>) (items:Map<uint64, ItemDescriptor>) (source:CommandSource) (sink:MessageSink) (avatarId:string) (world:World) : Gamestate option =

        "" |> Line |> sink
        world.Avatars.[avatarId].Messages
        |> Utility.DumpMessages sink
        let world =
            world
            |> World.ClearMessages avatarId
        let avatar = world.Avatars.[avatarId]
        let speedHue =
            if avatar.Speed >= 0.9 then
                Value
            elif avatar.Speed>=0.4 then
                Warning
            else
                Error
        [
            (Heading, "At Sea:" |> Line) |> Hued
            (Label, "Turn: " |> Text) |> Hued
            (Value, avatar.Shipmates.[0].Statistics.[AvatarStatisticIdentifier.Turn].CurrentValue |> sprintf "%.0f" |> Text) |> Hued
            (Value, avatar.Shipmates.[0].Statistics.[AvatarStatisticIdentifier.Turn].MaximumValue |> sprintf "/%.0f" |> Line) |> Hued
            (Label, "Heading: " |> Text) |> Hued
            (Value, avatar.Heading |> Dms.ToDms |> Dms.ToString |> sprintf "%s" |> Line) |> Hued
            (Label, "Speed: " |> Text) |> Hued
            (speedHue, (avatar.Speed * 100.0) |> sprintf "%.0f%%" |> Text) |> Hued
            avatar |> Avatar.GetEffectiveSpeed |> sprintf "(Effective rate: %.2f)" |> Line
            (Subheading, "Nearby:" |> Line) |> Hued
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
                    (Sublabel, "Name: " |> Text) |> Hued |> sink
                    (Value, sprintf "%s " (if name="" then "????" else name) |> Text) |> Hued |> sink
                    (Sublabel, "Bearing: " |> Text) |> Hued |> sink
                    (Value, sprintf "%s " heading |> Text) |> Hued |> sink
                    (Sublabel, "Distance: " |> Text) |> Hued |> sink
                    (Value, sprintf "%f" distance |> Text) |> Hued |> sink
                    (Flavor, sprintf "%s" (if distance<avatar.DockDistance then " (Can Dock)" else "") |> Line) |> Hued |> sink
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
                (Dock, location, world |> World.Dock islandItemSource islandItemSink random rewardRange commodities items location avatarId)
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
            (Error, "Maybe try 'help'?" |> Line) |> Hued |> sink
            world
            |> Gamestate.AtSea
            |> Some

    let Run (islandItemSource) (islandItemSink) (random:System.Random) (rewardRange:float*float) (connection:SQLiteConnection) (source:CommandSource) (sink:MessageSink) (world:World) : Gamestate option =
        if world |> World.IsAvatarAlive world.AvatarId then
            match connection |> Commodity.GetList, connection |> Item.GetList with
            | Ok commodities, Ok items ->
                RunAlive islandItemSource islandItemSink random rewardRange commodities items source sink world.AvatarId world
            | Result.Error message, _ ->
                raise (System.InvalidOperationException message)
            | _, Result.Error message ->
                raise (System.InvalidOperationException message)
        else
            world.Avatars.[world.AvatarId].Messages
            |> Gamestate.GameOver
            |> Some
