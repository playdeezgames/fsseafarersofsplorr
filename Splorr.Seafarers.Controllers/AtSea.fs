namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open Splorr.Seafarers.Persistence

module AtSea =
    let private DetermineSpeedHue (speed:float) : Hue =
        if speed >= 0.9 then
            Hue.Value
        elif speed>=0.4 then
            Hue.Warning
        else
            Hue.Error

    let private CanCareen (world: World) : bool =
        let avatar = world.Avatars.[world.AvatarId]
        world
        |> World.GetNearbyLocations avatar.Position avatar.ViewDistance
        |> List.map (fun l -> (l,world.Islands.[l].CareenDistance))
        |> List.exists (fun (l,d) -> Location.DistanceTo l avatar.Position < d)

    let private GetVisibleIslands (world:World) : (Location * string * float * string) list =
        let avatar = world.Avatars.[world.AvatarId]
        world
        |> World.GetNearbyLocations avatar.Position avatar.ViewDistance
        |> List.map
            (fun location -> 
                (location, Location.HeadingTo avatar.Position location |> Dms.ToDegrees |> Dms.ToString, Location.DistanceTo avatar.Position location, (world.Islands.[location] |> Island.GetDisplayName world.AvatarId)))
        |> List.sortBy (fun (_,_,d,_)->d)

    let private OutputState 
            (messageSink:MessageSink) 
            (world:World) : unit =
        "" |> Line |> messageSink
        world.Avatars.[world.AvatarId].Messages
        |> Utility.DumpMessages messageSink

        let avatar = world.Avatars.[world.AvatarId]
        let speedHue =DetermineSpeedHue avatar.Speed
        let shipmateZero = avatar.Shipmates.[0]
        [
            (Hue.Heading, "At Sea:" |> Line) |> Hued
            (Hue.Label, "Turn: " |> Text) |> Hued
            (Hue.Value, shipmateZero.Statistics.[AvatarStatisticIdentifier.Turn].CurrentValue |> sprintf "%.0f" |> Text) |> Hued
            (Hue.Value, shipmateZero.Statistics.[AvatarStatisticIdentifier.Turn].MaximumValue |> sprintf "/%.0f" |> Line) |> Hued
            (Hue.Label, "Heading: " |> Text) |> Hued
            (Hue.Value, avatar.Heading |> Dms.ToDegrees |> Dms.ToString |> sprintf "%s" |> Line) |> Hued
            (Hue.Label, "Speed: " |> Text) |> Hued
            (speedHue, (avatar.Speed * 100.0) |> sprintf "%.0f%%" |> Text) |> Hued
            avatar |> Avatar.GetEffectiveSpeed |> sprintf "(Effective rate: %.2f)" |> Line
            (Hue.Subheading, "Nearby:" |> Line) |> Hued
        ]
        |> List.iter messageSink

        world
        |> GetVisibleIslands
        |> List.iter
            (fun (_, heading, distance, name) -> 
                [
                    (Hue.Sublabel, "Name: " |> Text) |> Hued
                    (Hue.Value, sprintf "%s " (if name="" then "????" else name) |> Text) |> Hued
                    (Hue.Sublabel, "Bearing: " |> Text) |> Hued
                    (Hue.Value, sprintf "%s " heading |> Text) |> Hued
                    (Hue.Sublabel, "Distance: " |> Text) |> Hued
                    (Hue.Value, sprintf "%f" distance |> Text) |> Hued
                    (Hue.Flavor, sprintf "%s" (if distance<avatar.DockDistance then " (Can Dock)" else "") |> Line) |> Hued
                ]
                |> List.iter messageSink)

    let private UpdateState 
            (commoditySource:unit -> Map<uint64, CommodityDescriptor>) 
            (itemSource:unit -> Map<uint64, ItemDescriptor>) 
            (islandMarketSource:Location->Map<uint64, Market>) 
            (islandMarketSink:Location->Map<uint64, Market>->unit) 
            (islandItemSource:Location->Set<uint64>) 
            (islandItemSink:Location->Set<uint64>->unit) 
            (random:System.Random) 
            (rewardRange:float*float) 
            (command:Command option) 
            (world:World) 
            : bool * (Gamestate option) =
        let world =
            world
            |> World.ClearMessages world.AvatarId

        let avatar = world.Avatars.[world.AvatarId]

        let canCareen = 
            CanCareen world

        let dockTarget = 
            world
            |> GetVisibleIslands
            |> List.fold
                (fun target (location, _, distance, _) -> 
                    (if distance<avatar.DockDistance then (Some location) else target)) None

        match command with
        | Some Command.Status ->
            (true, world 
            |> Gamestate.AtSea
            |> Gamestate.Status
            |> Some)

        | Some (Command.HeadFor name) ->
            (true, world
            |> World.HeadFor name
            |> Gamestate.AtSea
            |> Some)

        | Some (Command.DistanceTo name) ->
            (true, world
            |> World.DistanceTo name
            |> Gamestate.AtSea
            |> Some)

        | Some (Command.Careen side) ->
            if canCareen then
                (true, (side, world)
                |> Gamestate.Careened
                |> Some)
            else
                (true, world
                |> World.AddMessages world.AvatarId [ "You cannot careen here." ]
                |> Gamestate.AtSea
                |> Some)

        | Some Command.Dock ->
            match dockTarget with
            | Some location ->
                (true, 
                    (Dock, 
                        location, 
                            world 
                            |> World.Dock 
                                commoditySource 
                                itemSource
                                islandMarketSource 
                                islandMarketSink 
                                islandItemSource 
                                islandItemSink 
                                random 
                                rewardRange 
                                location)
                |> Gamestate.Docked
                |> Some)
            | None ->
                (true, world
                |> World.AddMessages world.AvatarId [ "There is no place to dock." ]
                |> Gamestate.AtSea
                |> Some)

        | Some (Command.Islands page) ->
            (true, (page, world |> Gamestate.AtSea)
            |> Gamestate.IslandList
            |> Some)

        | Some (Command.Abandon Job) ->
            (true, world
            |> World.AbandonJob world.AvatarId
            |> Gamestate.AtSea
            |> Some)

        | Some Command.Metrics ->
            (true, world 
            |> Gamestate.AtSea
            |> Gamestate.Metrics
            |> Some)

        | Some (Command.Chart x) ->
            (true, (x, world)
            |> Gamestate.Chart
            |> Some)

        | Some Command.Menu ->
            (true, world
            |> Some
            |> Gamestate.MainMenu
            |> Some)

        | Some Command.Help ->
            (true, world
            |> Gamestate.AtSea
            |> Gamestate.Help
            |> Some)

        | Some (Command.Move distance)->
            (true, world
            |> World.Move distance world.AvatarId
            |> Gamestate.AtSea
            |> Some)

        | Some (Command.Set (SetCommand.Heading heading)) ->
            (true, world
            |> World.SetHeading heading world.AvatarId
            |> Gamestate.AtSea
            |> Some)

        | Some (Command.Set (Speed speed)) ->
            (true, world
            |> World.SetSpeed speed world.AvatarId
            |> Gamestate.AtSea
            |> Some)

        | Some Command.Quit -> 
            (true, world
            |> Gamestate.AtSea
            |> Gamestate.ConfirmQuit
            |> Some)

        | Some Command.Inventory -> 
            (true, world
            |> Gamestate.AtSea
            |> Gamestate.Inventory
            |> Some)

        | _ ->
            (false, world
            |> Gamestate.AtSea
            |> Some)

    let private RunAlive 
            (commoditySource:unit -> Map<uint64, CommodityDescriptor>) 
            (itemSource:unit -> Map<uint64, ItemDescriptor>) 
            (islandMarketSource:Location->Map<uint64, Market>) 
            (islandMarketSink:Location->Map<uint64, Market>->unit) 
            (islandItemSource:Location->Set<uint64>) 
            (islandItemSink:Location->Set<uint64>->unit) 
            (random:System.Random) 
            (rewardRange:float*float) 
            (source:CommandSource) 
            (sink:MessageSink) 
            (world:World) 
            : Gamestate option =
        OutputState sink world
        let handled, next = 
            UpdateState
                commoditySource
                itemSource
                islandMarketSource
                islandMarketSink
                islandItemSource
                islandItemSink
                random
                rewardRange
                (source())
                world
        if not handled then
            (Hue.Error, "Maybe try 'help'?" |> Line) |> Hued |> sink
        next


    let Run 
            (commoditySource:unit -> Map<uint64, CommodityDescriptor>) 
            (itemSource:unit -> Map<uint64, ItemDescriptor>) 
            (islandMarketSource:Location->Map<uint64, Market>) 
            (islandMarketSink:Location->Map<uint64, Market>->unit) 
            (islandItemSource:Location->Set<uint64>) 
            (islandItemSink:Location->Set<uint64>->unit) 
            (random:System.Random) 
            (rewardRange:float*float) 
            (source:CommandSource) 
            (sink:MessageSink) 
            (world:World) 
            : Gamestate option =
        if world |> World.IsAvatarAlive world.AvatarId then
            RunAlive commoditySource itemSource islandMarketSource islandMarketSink islandItemSource islandItemSink random rewardRange source sink world
        else
            world.Avatars.[world.AvatarId].Messages
            |> Gamestate.GameOver
            |> Some
