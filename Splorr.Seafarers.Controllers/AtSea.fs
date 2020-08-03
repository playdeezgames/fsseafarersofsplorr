namespace Splorr.Seafarers.Controllers

open System
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open Splorr.Seafarers.Persistence

module AtSea =
    let private DetermineSpeedHue 
            (speed:float) 
            : Hue =
        if speed >= 0.9 then
            Hue.Value
        elif speed>=0.4 then
            Hue.Warning
        else
            Hue.Error

    let private CanCareen 
            (world : World) 
            : bool =
        let avatar = world.Avatars.[world.AvatarId]
        world
        |> World.GetNearbyLocations avatar.Position avatar.ViewDistance
        |> List.map (fun l -> (l,world.Islands.[l].CareenDistance))
        |> List.exists (fun (l,d) -> Location.DistanceTo l avatar.Position < d)

    let private GetVisibleIslands 
            (world : World) 
            : (Location * string * float * string) list =
        let avatar = world.Avatars.[world.AvatarId]
        world
        |> World.GetNearbyLocations avatar.Position avatar.ViewDistance
        |> List.map
            (fun location -> 
                (location, Location.HeadingTo avatar.Position location |> Angle.ToDegrees |> Angle.ToString, Location.DistanceTo avatar.Position location, (world.Islands.[location] |> Island.GetDisplayName world.AvatarId)))
        |> List.sortBy (fun (_,_,d,_)->d)

    let private UpdateDisplay 
            (vesselSingleStatisticSource : string->VesselStatisticIdentifier->Statistic option)
            (messageSink                 : MessageSink) 
            (world                       : World) 
            : unit =
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
            (Hue.Value, avatar.Heading |> Angle.ToDegrees |> Angle.ToString |> sprintf "%s" |> Line) |> Hued
            (Hue.Label, "Speed: " |> Text) |> Hued
            (speedHue, (avatar.Speed * 100.0) |> sprintf "%.0f%%" |> Text) |> Hued
            avatar |> Avatar.GetEffectiveSpeed vesselSingleStatisticSource world.AvatarId |> sprintf "(Effective rate: %.2f)" |> Line
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

    let private HandleCommand 
            (termSources                 : TermSource * TermSource * TermSource * TermSource * TermSource * TermSource)
            (commoditySource             : unit -> Map<uint64, CommodityDescriptor>) 
            (itemSource                  : unit -> Map<uint64, ItemDescriptor>) 
            (islandMarketSource          : Location -> Map<uint64, Market>) 
            (islandMarketSink            : Location -> Map<uint64, Market> -> unit) 
            (islandItemSource            : Location -> Set<uint64>) 
            (islandItemSink              : Location -> Set<uint64> -> unit) 
            (vesselSingleStatisticSource : string -> VesselStatisticIdentifier -> Statistic option)
            (vesselSingleStatisticSink   : string -> VesselStatisticIdentifier * Statistic -> unit)
            (random                      : Random) 
            (rewardRange                 : float * float) 
            (command                     : Command option) 
            (world                       : World) 
            : Gamestate option =
        let world =
            world
            |> World.ClearMessages

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
                |> World.AddMessages [ "You cannot careen here." ]
                |> Gamestate.AtSea
                |> Some

        | Some Command.Dock ->
            match dockTarget with
            | Some location ->
                (Dock, 
                    location, 
                        world 
                        |> World.Dock 
                            termSources
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
                |> Some
            | None ->
                world
                |> World.AddMessages [ "There is no place to dock." ]
                |> Gamestate.AtSea
                |> Some

        | Some (Command.Islands page) ->
            (page, world |> Gamestate.AtSea)
            |> Gamestate.IslandList
            |> Some

        | Some (Command.Abandon Job) ->
            world
            |> World.AbandonJob
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
            |> World.Move vesselSingleStatisticSource vesselSingleStatisticSink distance
            |> Gamestate.AtSea
            |> Some

        | Some (Command.Set (SetCommand.Heading heading)) ->
            world
            |> World.SetHeading heading
            |> Gamestate.AtSea
            |> Some

        | Some (Command.Set (Speed speed)) ->
            world
            |> World.SetSpeed speed
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
            ("Maybe try 'help'?",world
            |> Gamestate.AtSea)
            |> Gamestate.ErrorMessage
            |> Some

    let private RunAlive 
            (termSources                 : TermSource * TermSource * TermSource * TermSource * TermSource * TermSource)
            (commoditySource             : unit -> Map<uint64, CommodityDescriptor>) 
            (itemSource                  : unit -> Map<uint64, ItemDescriptor>) 
            (islandMarketSource          : Location -> Map<uint64, Market>) 
            (islandMarketSink            : Location -> Map<uint64, Market>->unit) 
            (islandItemSource            : Location -> Set<uint64>) 
            (islandItemSink              : Location -> Set<uint64>->unit) 
            (vesselSingleStatisticSource : string->VesselStatisticIdentifier->Statistic option)
            (vesselSingleStatisticSink   : string->VesselStatisticIdentifier*Statistic->unit)
            (random                      : Random) 
            (rewardRange                 : float*float) 
            (commandSource               : CommandSource) 
            (messageSink                 : MessageSink) 
            (world                       : World) 
            : Gamestate option =
        UpdateDisplay 
            vesselSingleStatisticSource
            messageSink 
            world
        HandleCommand
            termSources
            commoditySource
            itemSource
            islandMarketSource
            islandMarketSink
            islandItemSource
            islandItemSink
            vesselSingleStatisticSource
            vesselSingleStatisticSink
            random
            rewardRange
            (commandSource())
            world

    let Run 
            (termSources                 : TermSource * TermSource * TermSource * TermSource * TermSource * TermSource)
            (commoditySource             : unit -> Map<uint64, CommodityDescriptor>) 
            (itemSource                  : unit -> Map<uint64, ItemDescriptor>) 
            (islandMarketSource          : Location -> Map<uint64, Market>) 
            (islandMarketSink            : Location -> Map<uint64, Market>->unit) 
            (islandItemSource            : Location -> Set<uint64>) 
            (islandItemSink              : Location -> Set<uint64>->unit) 
            (vesselSingleStatisticSource : string->VesselStatisticIdentifier->Statistic option)
            (vesselSingleStatisticSink   : string->VesselStatisticIdentifier*Statistic->unit)
            (random                      : Random) 
            (rewardRange                 : float*float) 
            (commandSource               : CommandSource) 
            (messageSink                 : MessageSink) 
            (world                       : World) 
            : Gamestate option =
        if world |> World.IsAvatarAlive then
            RunAlive 
                termSources
                commoditySource 
                itemSource 
                islandMarketSource 
                islandMarketSink 
                islandItemSource 
                islandItemSink 
                vesselSingleStatisticSource
                vesselSingleStatisticSink
                random 
                rewardRange 
                commandSource 
                messageSink 
                world
        else
            world.Avatars.[world.AvatarId].Messages
            |> Gamestate.GameOver
            |> Some
