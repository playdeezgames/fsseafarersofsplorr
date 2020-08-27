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
            (islandSingleStatisticSource : IslandSingleStatisticSource)
            (islandSource                : IslandSource)
            (vesselSingleStatisticSource : VesselSingleStatisticSource)
            (avatarId                    : string) 
            : bool =
        let viewDistance =
            vesselSingleStatisticSource avatarId VesselStatisticIdentifier.ViewDistance 
            |> Option.get 
            |> Statistic.GetCurrentValue
        let avatarPosition =
            Avatar.GetPosition vesselSingleStatisticSource avatarId
            |> Option.get
        World.GetNearbyLocations 
            islandSource
            avatarPosition 
            viewDistance
        |> List.map 
            (fun l -> 
                (l,
                    IslandStatisticIdentifier.CareenDistance
                    |> islandSingleStatisticSource l 
                    |> Option.get
                    |> Statistic.GetCurrentValue))
        |> List.exists (fun (l,d) -> Location.DistanceTo l avatarPosition < d)

    let private GetVisibleIslands 
            (avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource)
            (islandSingleNameSource         : IslandSingleNameSource)
            (islandSource                   : IslandSource)
            (vesselSingleStatisticSource    : VesselSingleStatisticSource)
            (avatarId                       : string) 
            : (Location * string * float * string) list =
        let viewDistance =
            vesselSingleStatisticSource avatarId VesselStatisticIdentifier.ViewDistance 
            |> Option.get 
            |> Statistic.GetCurrentValue
        let avatarPosition =
            avatarId
            |> Avatar.GetPosition vesselSingleStatisticSource
            |> Option.get
        World.GetNearbyLocations 
            islandSource
            avatarPosition 
            viewDistance
        |> List.map
            (fun location -> 
                (location, 
                    Location.HeadingTo 
                        avatarPosition 
                        location 
                    |> Angle.ToDegrees 
                    |> Angle.ToString, 
                        Location.DistanceTo 
                            avatarPosition 
                            location, 
                                (Island.GetDisplayName 
                                    avatarIslandSingleMetricSource
                                    islandSingleNameSource
                                    avatarId
                                    location)))
        |> List.sortBy (fun (_,_,d,_)->d)

    let private UpdateDisplay 
            (avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource)
            (avatarMessageSource            : AvatarMessageSource)
            (islandSingleNameSource         : IslandSingleNameSource)
            (islandSource                   : IslandSource)
            (shipmateSingleStatisticSource  : ShipmateSingleStatisticSource)
            (vesselSingleStatisticSource    : VesselSingleStatisticSource)
            (messageSink                    : MessageSink) 
            (avatarId                       : string) 
            : unit =
        "" |> Line |> messageSink
        avatarId
        |> avatarMessageSource
        |> Utility.DumpMessages messageSink

        let speed = 
            avatarId
            |> Avatar.GetSpeed vesselSingleStatisticSource 
            |> Option.get
        let heading = 
            avatarId 
            |> Avatar.GetHeading vesselSingleStatisticSource 
            |> Option.get
        let speedHue =DetermineSpeedHue speed
        let turn = shipmateSingleStatisticSource avatarId Primary ShipmateStatisticIdentifier.Turn |> Option.get
        [
            (Hue.Heading, "At Sea:" |> Line) |> Hued
            (Hue.Label, "Turn: " |> Text) |> Hued
            (Hue.Value, turn.CurrentValue |> sprintf "%.0f" |> Text) |> Hued
            (Hue.Value, turn.MaximumValue |> sprintf "/%.0f" |> Line) |> Hued
            (Hue.Label, "Heading: " |> Text) |> Hued
            (Hue.Value, heading |> Angle.ToDegrees |> Angle.ToString |> sprintf "%s" |> Line) |> Hued
            (Hue.Label, "Speed: " |> Text) |> Hued
            (speedHue, (speed * 100.0) |> sprintf "%.0f%%" |> Text) |> Hued
            avatarId 
            |> Avatar.GetEffectiveSpeed vesselSingleStatisticSource 
            |> sprintf "(Effective rate: %.2f)" |> Line
            (Hue.Subheading, "Nearby:" |> Line) |> Hued
        ]
        |> List.iter messageSink

        let dockDistance = 
            vesselSingleStatisticSource avatarId VesselStatisticIdentifier.DockDistance 
            |> Option.get 
            |> Statistic.GetCurrentValue
        avatarId
        |> GetVisibleIslands 
            avatarIslandSingleMetricSource
            islandSingleNameSource
            islandSource
            vesselSingleStatisticSource
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
            (avatarInventorySink            : AvatarInventorySink)
            (avatarInventorySource          : AvatarInventorySource)
            (avatarIslandSingleMetricSink   : AvatarIslandSingleMetricSink)
            (avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource)
            (avatarJobSink                  : AvatarJobSink)
            (avatarJobSource                : AvatarJobSource)
            (avatarMessagePurger            : AvatarMessagePurger)
            (avatarMessageSink              : AvatarMessageSink)
            (avatarShipmateSource           : AvatarShipmateSource)
            (avatarSingleMetricSink         : AvatarSingleMetricSink)
            (avatarSingleMetricSource       : AvatarSingleMetricSource)
            (commoditySource                : CommoditySource) 
            (islandItemSink                 : IslandItemSink) 
            (islandItemSource               : IslandItemSource) 
            (islandJobSink                  : IslandJobSink)
            (islandJobSource                : IslandJobSource)
            (islandLocationByNameSource     : IslandLocationByNameSource)
            (islandMarketSink               : IslandMarketSink) 
            (islandMarketSource             : IslandMarketSource) 
            (islandSource                   : IslandSource)
            (islandSingleNameSource         : IslandSingleNameSource)
            (islandSingleStatisticSource    : IslandSingleStatisticSource)
            (itemSource                     : ItemSource) 
            (shipmateRationItemSource       : ShipmateRationItemSource)
            (shipmateSingleStatisticSink    : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource  : ShipmateSingleStatisticSource)
            (termSources                    : TermSources)
            (vesselSingleStatisticSink      : VesselSingleStatisticSink)
            (vesselSingleStatisticSource    : VesselSingleStatisticSource)
            (worldSingleStatisticSource     : WorldSingleStatisticSource)
            (random                         : Random) 
            (command                        : Command option) 
            (avatarId                       : string) 
            : Gamestate option =
        avatarId
        |> World.ClearMessages avatarMessagePurger

        let canCareen = 
            CanCareen 
                islandSingleStatisticSource   
                islandSource
                vesselSingleStatisticSource 
                avatarId

        let dockDistance = 
            vesselSingleStatisticSource avatarId VesselStatisticIdentifier.DockDistance 
            |> Option.get 
            |> Statistic.GetCurrentValue
        let nearby = 
            avatarId
            |> GetVisibleIslands 
                avatarIslandSingleMetricSource
                islandSingleNameSource
                islandSource
                vesselSingleStatisticSource
        let dockTarget = 
            nearby
            |> List.fold
                (fun target (location, _, distance, _) -> 
                    (if distance<dockDistance then 
                        (Some location) 
                    else 
                        target)) None

        match command with
        | Some Command.Status ->
            avatarId 
            |> Gamestate.AtSea
            |> Gamestate.Status
            |> Some

        | Some (Command.HeadFor name) ->
            avatarId
            |> World.HeadFor
                avatarIslandSingleMetricSource
                avatarMessageSink 
                islandLocationByNameSource
                vesselSingleStatisticSource 
                vesselSingleStatisticSink 
                name
            avatarId
            |> Gamestate.AtSea
            |> Some

        | Some (Command.DistanceTo name) ->
            avatarId
            |> World.DistanceTo 
                avatarIslandSingleMetricSource
                avatarMessageSink 
                islandLocationByNameSource
                vesselSingleStatisticSource 
                name
            avatarId
            |> Gamestate.AtSea
            |> Some

        | Some (Command.Careen side) ->
            if canCareen then
                (side, avatarId)
                |> Gamestate.Careened
                |> Some
            else
                avatarId
                |> World.AddMessages avatarMessageSink [ "You cannot careen here." ]
                avatarId
                |> Gamestate.AtSea
                |> Some

        | Some Command.Dock ->
            match dockTarget with
            | Some location ->
                avatarId 
                |> World.Dock 
                    avatarIslandSingleMetricSink
                    avatarIslandSingleMetricSource
                    avatarJobSink
                    avatarJobSource
                    avatarMessageSink
                    avatarSingleMetricSink
                    avatarSingleMetricSource
                    commoditySource 
                    islandItemSink 
                    islandItemSource 
                    islandJobSink
                    islandJobSource
                    islandMarketSink 
                    islandMarketSource 
                    islandSource
                    itemSource
                    shipmateSingleStatisticSink
                    shipmateSingleStatisticSource
                    termSources
                    worldSingleStatisticSource
                    random 
                    location
                (Dock, 
                    location, 
                        avatarId)
                |> Gamestate.Docked
                |> Some
            | None ->
                avatarId
                |> World.AddMessages avatarMessageSink [ "There is no place to dock." ]
                avatarId
                |> Gamestate.AtSea
                |> Some

        | Some (Command.Islands page) ->
            (page, avatarId |> Gamestate.AtSea)
            |> Gamestate.IslandList
            |> Some

        | Some (Command.Abandon Job) ->
            avatarId
            |> World.AbandonJob 
                avatarJobSink
                avatarJobSource
                avatarMessageSink
                avatarSingleMetricSink
                avatarSingleMetricSource
                shipmateSingleStatisticSink
                shipmateSingleStatisticSource
            avatarId
            |> Gamestate.AtSea
            |> Some

        | Some Command.Metrics ->
            avatarId 
            |> Gamestate.AtSea
            |> Gamestate.Metrics
            |> Some

        | Some (Command.Chart x) ->
            (x, avatarId)
            |> Gamestate.Chart
            |> Some

        | Some Command.Menu ->
            avatarId
            |> Some
            |> Gamestate.MainMenu
            |> Some

        | Some Command.Help ->
            avatarId
            |> Gamestate.AtSea
            |> Gamestate.Help
            |> Some

        | Some (Command.Move distance)->
            avatarId
            |> World.Move 
                avatarInventorySink
                avatarInventorySource
                avatarIslandSingleMetricSink
                avatarMessageSink 
                avatarShipmateSource
                avatarSingleMetricSink
                avatarSingleMetricSource
                islandSource
                shipmateRationItemSource 
                shipmateSingleStatisticSink
                shipmateSingleStatisticSource
                vesselSingleStatisticSink 
                vesselSingleStatisticSource 
                distance
            avatarId
            |> Gamestate.AtSea
            |> Some

        | Some (Command.Set (SetCommand.Heading heading)) ->
            avatarId
            |> World.SetHeading vesselSingleStatisticSource vesselSingleStatisticSink avatarMessageSink heading
            avatarId
            |> Gamestate.AtSea
            |> Some

        | Some (Command.Set (Speed speed)) ->
            avatarId
            |> World.SetSpeed 
                vesselSingleStatisticSource
                vesselSingleStatisticSink
                avatarMessageSink
                speed
            avatarId
            |> Gamestate.AtSea
            |> Some

        | Some Command.Quit -> 
            avatarId
            |> Gamestate.AtSea
            |> Gamestate.ConfirmQuit
            |> Some

        | Some Command.Inventory -> 
            avatarId
            |> Gamestate.AtSea
            |> Gamestate.Inventory
            |> Some

        | _ ->
            ("Maybe try 'help'?",avatarId
            |> Gamestate.AtSea)
            |> Gamestate.ErrorMessage
            |> Some

    let private RunAlive
            (avatarInventorySink             : AvatarInventorySink)
            (avatarInventorySource           : AvatarInventorySource)
            (avatarIslandSingleMetricSink    : AvatarIslandSingleMetricSink)
            (avatarIslandSingleMetricSource  : AvatarIslandSingleMetricSource)
            (avatarJobSink                   : AvatarJobSink)
            (avatarJobSource                 : AvatarJobSource)
            (avatarMessagePurger             : AvatarMessagePurger)
            (avatarMessageSink               : AvatarMessageSink)
            (avatarMessageSource             : AvatarMessageSource)
            (avatarShipmateSource            : AvatarShipmateSource)
            (avatarSingleMetricSink          : AvatarSingleMetricSink)
            (avatarSingleMetricSource        : AvatarSingleMetricSource)
            (commoditySource                 : CommoditySource) 
            (islandItemSink                  : IslandItemSink) 
            (islandItemSource                : IslandItemSource) 
            (islandJobSink                   : IslandJobSink)
            (islandJobSource                 : IslandJobSource)
            (islandLocationByNameSource      : IslandLocationByNameSource)
            (islandMarketSink                : IslandMarketSink) 
            (islandMarketSource              : IslandMarketSource) 
            (islandSingleNameSource          : IslandSingleNameSource)
            (islandSingleStatisticSource     : IslandSingleStatisticSource)
            (islandSource                    : IslandSource)
            (itemSource                      : ItemSource) 
            (shipmateRationItemSource        : ShipmateRationItemSource)
            (termSources                     : TermSources)
            (vesselSingleStatisticSink       : VesselSingleStatisticSink)
            (vesselSingleStatisticSource     : VesselSingleStatisticSource)
            (worldSingleStatisticSource      : WorldSingleStatisticSource)
            (shipmateSingleStatisticSource   : ShipmateSingleStatisticSource)
            (shipmateSingleStatisticSink     : ShipmateSingleStatisticSink)
            (random                          : Random) 
            (commandSource                   : CommandSource) 
            (messageSink                     : MessageSink) 
            (avatarId                        : string) 
            : Gamestate option =
        UpdateDisplay 
            avatarIslandSingleMetricSource
            avatarMessageSource
            islandSingleNameSource
            islandSource
            shipmateSingleStatisticSource
            vesselSingleStatisticSource
            messageSink 
            avatarId
        HandleCommand
            avatarInventorySink
            avatarInventorySource
            avatarIslandSingleMetricSink
            avatarIslandSingleMetricSource
            avatarJobSink
            avatarJobSource
            avatarMessagePurger
            avatarMessageSink
            avatarShipmateSource
            avatarSingleMetricSink
            avatarSingleMetricSource
            commoditySource
            islandItemSink
            islandItemSource
            islandJobSink
            islandJobSource
            islandLocationByNameSource
            islandMarketSink
            islandMarketSource
            islandSource
            islandSingleNameSource
            islandSingleStatisticSource
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
            avatarId

    let Run 
            (avatarInventorySink             : AvatarInventorySink)
            (avatarInventorySource           : AvatarInventorySource)
            (avatarIslandSingleMetricSink    : AvatarIslandSingleMetricSink)
            (avatarIslandSingleMetricSource  : AvatarIslandSingleMetricSource)
            (avatarJobSink                   : AvatarJobSink)
            (avatarJobSource                 : AvatarJobSource)
            (avatarMessagePurger             : AvatarMessagePurger)
            (avatarMessageSink               : AvatarMessageSink)
            (avatarMessageSource             : AvatarMessageSource)
            (avatarShipmateSource            : AvatarShipmateSource)
            (avatarSingleMetricSink          : AvatarSingleMetricSink)
            (avatarSingleMetricSource        : AvatarSingleMetricSource)
            (commoditySource                 : CommoditySource) 
            (islandItemSink                  : IslandItemSink) 
            (islandItemSource                : IslandItemSource) 
            (islandJobSink                   : IslandJobSink)
            (islandJobSource                 : IslandJobSource)
            (islandLocationByNameSource      : IslandLocationByNameSource)
            (islandMarketSink                : IslandMarketSink) 
            (islandMarketSource              : IslandMarketSource) 
            (islandSingleNameSource          : IslandSingleNameSource)
            (islandSingleStatisticSource     : IslandSingleStatisticSource)
            (islandSource                    : IslandSource) 
            (itemSource                      : ItemSource) 
            (shipmateRationItemSource        : ShipmateRationItemSource)
            (shipmateSingleStatisticSink     : ShipmateSingleStatisticSink)
            (shipmateSingleStatisticSource   : ShipmateSingleStatisticSource)
            (termSources                     : TermSources)
            (vesselSingleStatisticSink       : VesselSingleStatisticSink)
            (vesselSingleStatisticSource     : VesselSingleStatisticSource)
            (worldSingleStatisticSource      : WorldSingleStatisticSource)
            (random                          : Random) 
            (commandSource                   : CommandSource) 
            (messageSink                     : MessageSink) 
            (avatarId                        : string) 
            : Gamestate option =
        if avatarId |> World.IsAvatarAlive shipmateSingleStatisticSource then
            RunAlive
                avatarInventorySink
                avatarInventorySource
                avatarIslandSingleMetricSink
                avatarIslandSingleMetricSource
                avatarJobSink
                avatarJobSource
                avatarMessagePurger
                avatarMessageSink
                avatarMessageSource
                avatarShipmateSource
                avatarSingleMetricSink
                avatarSingleMetricSource
                commoditySource 
                islandItemSink 
                islandItemSource 
                islandJobSink
                islandJobSource
                islandLocationByNameSource
                islandMarketSink 
                islandMarketSource 
                islandSingleNameSource
                islandSingleStatisticSource
                islandSource
                itemSource 
                shipmateRationItemSource
                termSources
                vesselSingleStatisticSink
                vesselSingleStatisticSource
                worldSingleStatisticSource
                shipmateSingleStatisticSource
                shipmateSingleStatisticSink
                random 
                commandSource 
                messageSink 
                avatarId
        else
            avatarId
            |> avatarMessageSource
            |> Gamestate.GameOver
            |> Some
