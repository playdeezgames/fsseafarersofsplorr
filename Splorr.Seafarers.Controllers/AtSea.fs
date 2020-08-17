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
            (vesselSingleStatisticSource : string -> VesselStatisticIdentifier -> Statistic option)
            (world : World) 
            : bool =
        let avatar = world.Avatars.[world.AvatarId]
        let viewDistance =
            vesselSingleStatisticSource world.AvatarId VesselStatisticIdentifier.ViewDistance 
            |> Option.get 
            |> Statistic.GetCurrentValue
        let avatarPosition =
            Avatar.GetPosition vesselSingleStatisticSource world.AvatarId
            |> Option.get
        world
        |> World.GetNearbyLocations avatarPosition viewDistance
        |> List.map (fun l -> (l,world.Islands.[l].CareenDistance))
        |> List.exists (fun (l,d) -> Location.DistanceTo l avatarPosition < d)

    let private GetVisibleIslands 
            (vesselSingleStatisticSource : string -> VesselStatisticIdentifier -> Statistic option)
            (world : World) 
            : (Location * string * float * string) list =
        let avatar = world.Avatars.[world.AvatarId]
        let viewDistance =
            vesselSingleStatisticSource world.AvatarId VesselStatisticIdentifier.ViewDistance 
            |> Option.get 
            |> Statistic.GetCurrentValue
        let avatarPosition =
            world.AvatarId
            |> Avatar.GetPosition vesselSingleStatisticSource
            |> Option.get
        world
        |> World.GetNearbyLocations avatarPosition viewDistance
        |> List.map
            (fun location -> 
                (location, Location.HeadingTo avatarPosition location |> Angle.ToDegrees |> Angle.ToString, Location.DistanceTo avatarPosition location, (world.Islands.[location] |> Island.GetDisplayName world.AvatarId)))
        |> List.sortBy (fun (_,_,d,_)->d)

    let private UpdateDisplay 
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (vesselSingleStatisticSource   : VesselSingleStatisticSource)
            (avatarMessageSource           : AvatarMessageSource)
            (messageSink                   : MessageSink) 
            (world                         : World) 
            : unit =
        "" |> Line |> messageSink
        world.AvatarId
        |> avatarMessageSource
        |> Utility.DumpMessages messageSink

        let avatar = world.Avatars.[world.AvatarId]
        let speed = 
            world.AvatarId 
            |> Avatar.GetSpeed vesselSingleStatisticSource 
            |> Option.get
        let heading = 
            world.AvatarId 
            |> Avatar.GetHeading vesselSingleStatisticSource 
            |> Option.get
        let speedHue =DetermineSpeedHue speed
        let turn = shipmateSingleStatisticSource world.AvatarId Primary ShipmateStatisticIdentifier.Turn |> Option.get
        [
            (Hue.Heading, "At Sea:" |> Line) |> Hued
            (Hue.Label, "Turn: " |> Text) |> Hued
            (Hue.Value, turn.CurrentValue |> sprintf "%.0f" |> Text) |> Hued
            (Hue.Value, turn.MaximumValue |> sprintf "/%.0f" |> Line) |> Hued
            (Hue.Label, "Heading: " |> Text) |> Hued
            (Hue.Value, heading |> Angle.ToDegrees |> Angle.ToString |> sprintf "%s" |> Line) |> Hued
            (Hue.Label, "Speed: " |> Text) |> Hued
            (speedHue, (speed * 100.0) |> sprintf "%.0f%%" |> Text) |> Hued
            world.AvatarId 
            |> Avatar.GetEffectiveSpeed vesselSingleStatisticSource 
            |> sprintf "(Effective rate: %.2f)" |> Line
            (Hue.Subheading, "Nearby:" |> Line) |> Hued
        ]
        |> List.iter messageSink

        let dockDistance = 
            vesselSingleStatisticSource world.AvatarId VesselStatisticIdentifier.DockDistance 
            |> Option.get 
            |> Statistic.GetCurrentValue
        world
        |> GetVisibleIslands vesselSingleStatisticSource
        |> List.iter
            (fun (_, heading, distance, name) -> 
                [
                    (Hue.Sublabel, "Name: " |> Text) |> Hued
                    (Hue.Value, sprintf "%s " (if name="" then "????" else name) |> Text) |> Hued
                    (Hue.Sublabel, "Bearing: " |> Text) |> Hued
                    (Hue.Value, sprintf "%s " heading |> Text) |> Hued
                    (Hue.Sublabel, "Distance: " |> Text) |> Hued
                    (Hue.Value, sprintf "%f" distance |> Text) |> Hued
                    (Hue.Flavor, sprintf "%s" (if distance<dockDistance then " (Can Dock)" else "") |> Line) |> Hued
                ]
                |> List.iter messageSink)

    let private HandleCommand 
            (avatarInventorySink           : AvatarInventorySink)
            (avatarInventorySource         : AvatarInventorySource)
            (avatarMessagePurger           : AvatarMessagePurger)
            (avatarMessageSink             : AvatarMessageSink)
            (avatarShipmateSource          : AvatarShipmateSource)
            (commoditySource               : CommoditySource) 
            (islandItemSink                : IslandItemSink) 
            (islandItemSource              : IslandItemSource) 
            (islandMarketSink              : IslandMarketSink) 
            (islandMarketSource            : IslandMarketSource) 
            (itemSource                    : ItemSource) 
            (shipmateRationItemSource      : ShipmateRationItemSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (termSources                   : TermSources)
            (vesselSingleStatisticSink     : VesselSingleStatisticSink)
            (vesselSingleStatisticSource   : VesselSingleStatisticSource)
            (worldSingleStatisticSource    : WorldSingleStatisticSource)
            (random                        : Random) 
            (command                       : Command option) 
            (world                         : World) 
            : Gamestate option =
        world
        |> World.ClearMessages avatarMessagePurger

        let avatar = world.Avatars.[world.AvatarId]

        let canCareen = 
            CanCareen vesselSingleStatisticSource world

        let dockDistance = 
            vesselSingleStatisticSource world.AvatarId VesselStatisticIdentifier.ViewDistance 
            |> Option.get 
            |> Statistic.GetCurrentValue
        let dockTarget = 
            world
            |> GetVisibleIslands vesselSingleStatisticSource
            |> List.fold
                (fun target (location, _, distance, _) -> 
                    (if distance<dockDistance then (Some location) else target)) None

        match command with
        | Some Command.Status ->
            world 
            |> Gamestate.AtSea
            |> Gamestate.Status
            |> Some

        | Some (Command.HeadFor name) ->
            world
            |> World.HeadFor vesselSingleStatisticSource vesselSingleStatisticSink avatarMessageSink name
            world
            |> Gamestate.AtSea
            |> Some

        | Some (Command.DistanceTo name) ->
            world
            |> World.DistanceTo vesselSingleStatisticSource avatarMessageSink name
            world
            |> Gamestate.AtSea
            |> Some

        | Some (Command.Careen side) ->
            if canCareen then
                (side, world)
                |> Gamestate.Careened
                |> Some
            else
                world
                |> World.AddMessages avatarMessageSink [ "You cannot careen here." ]
                world
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
                            worldSingleStatisticSource
                            islandMarketSource 
                            islandMarketSink 
                            islandItemSource 
                            islandItemSink 
                            shipmateSingleStatisticSource
                            shipmateSingleStatisticSink
                            avatarMessageSink
                            random 
                            location)
                |> Gamestate.Docked
                |> Some
            | None ->
                world
                |> World.AddMessages avatarMessageSink [ "There is no place to dock." ]
                world
                |> Gamestate.AtSea
                |> Some

        | Some (Command.Islands page) ->
            (page, world |> Gamestate.AtSea)
            |> Gamestate.IslandList
            |> Some

        | Some (Command.Abandon Job) ->
            world
            |> World.AbandonJob 
                shipmateSingleStatisticSource
                shipmateSingleStatisticSink
                avatarMessageSink
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
            |> World.Move 
                avatarShipmateSource
                avatarInventorySource
                avatarInventorySink
                shipmateSingleStatisticSource
                shipmateSingleStatisticSink
                vesselSingleStatisticSource 
                vesselSingleStatisticSink 
                shipmateRationItemSource 
                avatarMessageSink 
                distance
            |> Gamestate.AtSea
            |> Some

        | Some (Command.Set (SetCommand.Heading heading)) ->
            world
            |> World.SetHeading vesselSingleStatisticSource vesselSingleStatisticSink avatarMessageSink heading
            world
            |> Gamestate.AtSea
            |> Some

        | Some (Command.Set (Speed speed)) ->
            world
            |> World.SetSpeed 
                vesselSingleStatisticSource
                vesselSingleStatisticSink
                avatarMessageSink
                speed
            world
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
            (termSources                   : TermSources)
            (commoditySource               : CommoditySource) 
            (itemSource                    : ItemSource) 
            (worldSingleStatisticSource    : WorldSingleStatisticSource)
            (islandMarketSource            : IslandMarketSource) 
            (islandMarketSink              : IslandMarketSink) 
            (islandItemSource              : IslandItemSource) 
            (islandItemSink                : IslandItemSink) 
            (vesselSingleStatisticSource   : VesselSingleStatisticSource)
            (vesselSingleStatisticSink     : VesselSingleStatisticSink)
            (shipmateRationItemSource      : ShipmateRationItemSource)
            (avatarShipmateSource          : AvatarShipmateSource)
            (avatarInventorySource         : AvatarInventorySource)
            (avatarInventorySink           : AvatarInventorySink)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (avatarMessageSource           : AvatarMessageSource)
            (avatarMessageSink             : AvatarMessageSink)
            (avatarMessagePurger           : AvatarMessagePurger)
            (random                        : Random) 
            (commandSource                 : CommandSource) 
            (messageSink                   : MessageSink) 
            (world                         : World) 
            : Gamestate option =
        UpdateDisplay 
            shipmateSingleStatisticSource
            vesselSingleStatisticSource
            avatarMessageSource
            messageSink 
            world
        HandleCommand
            avatarInventorySink
            avatarInventorySource
            avatarMessagePurger
            avatarMessageSink
            avatarShipmateSource
            commoditySource
            islandItemSink
            islandItemSource
            islandMarketSink
            islandMarketSource
            itemSource
            shipmateRationItemSource
            shipmateSingleStatisticSink
            shipmateSingleStatisticSource
            termSources
            vesselSingleStatisticSink
            vesselSingleStatisticSource
            worldSingleStatisticSource
            random
            (commandSource())
            world

    let Run 
            (avatarInventorySink           : AvatarInventorySink)
            (avatarInventorySource         : AvatarInventorySource)
            (avatarMessagePurger           : AvatarMessagePurger)
            (avatarMessageSink             : AvatarMessageSink)
            (avatarMessageSource           : AvatarMessageSource)
            (avatarShipmateSource          : AvatarShipmateSource)
            (commoditySource               : CommoditySource) 
            (islandItemSink                : IslandItemSink) 
            (islandItemSource              : IslandItemSource) 
            (islandMarketSink              : IslandMarketSink) 
            (islandMarketSource            : IslandMarketSource) 
            (itemSource                    : ItemSource) 
            (shipmateRationItemSource      : ShipmateRationItemSource)
            (shipmateSingleStatisticSink   : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (termSources                   : TermSources)
            (vesselSingleStatisticSink     : VesselSingleStatisticSink)
            (vesselSingleStatisticSource   : VesselSingleStatisticSource)
            (worldSingleStatisticSource    : WorldSingleStatisticSource)
            (random                        : Random) 
            (commandSource                 : CommandSource) 
            (messageSink                   : MessageSink) 
            (world                         : World) 
            : Gamestate option =
        if world |> World.IsAvatarAlive shipmateSingleStatisticSource then
            RunAlive 
                termSources
                commoditySource 
                itemSource 
                worldSingleStatisticSource
                islandMarketSource 
                islandMarketSink 
                islandItemSource 
                islandItemSink 
                vesselSingleStatisticSource
                vesselSingleStatisticSink
                shipmateRationItemSource
                avatarShipmateSource
                avatarInventorySource
                avatarInventorySink
                shipmateSingleStatisticSource
                shipmateSingleStatisticSink
                avatarMessageSource
                avatarMessageSink
                avatarMessagePurger
                random 
                commandSource 
                messageSink 
                world
        else
            world.AvatarId
            |> avatarMessageSource
            |> Gamestate.GameOver
            |> Some
