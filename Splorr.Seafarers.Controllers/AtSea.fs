namespace Splorr.Seafarers.Controllers

open System
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open Splorr.Seafarers.Persistence

type AtSeaGetVisibleIslandsContext =
    inherit AvatarGetPositionContext
    inherit IslandGetDisplayNameContext

type AtSeaUpdateDisplayContext =
    inherit AtSeaGetVisibleIslandsContext
    inherit AvatarGetSpeedContext
    inherit AvatarGetHeadingContext
    inherit AvatarGetEffectiveSpeedContext
    inherit WorldDistanceToContext

type AtSeaCanCareenContext =
    inherit AvatarGetPositionContext

type AtSeaHandleCommandContext =
    inherit WorldDockContext
    inherit AtSeaGetVisibleIslandsContext
    inherit AtSeaUpdateDisplayContext
    inherit WorldMoveContext
    inherit WorldAbandonJobContext
    inherit WorldHeadForContext
    inherit WorldAddMessagesContext
    inherit WorldSetSpeedContext
    inherit AtSeaCanCareenContext
    abstract member avatarInventorySink            : AvatarInventorySink
    abstract member avatarInventorySource          : AvatarInventorySource
    abstract member avatarMessagePurger            : AvatarMessagePurger
    abstract member avatarShipmateSource           : AvatarShipmateSource
    abstract member islandLocationByNameSource     : IslandLocationByNameSource
    abstract member islandSingleNameSource         : IslandSingleNameSource
    abstract member islandSingleStatisticSource    : IslandSingleStatisticSource
    abstract member shipmateRationItemSource       : ShipmateRationItemSource
    abstract member vesselSingleStatisticSink      : VesselSingleStatisticSink
    abstract member vesselSingleStatisticSource    : VesselSingleStatisticSource

type AtSeaRunContext =
    inherit AtSeaHandleCommandContext
    abstract member avatarMessageSource             : AvatarMessageSource



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
            (context : AtSeaCanCareenContext)
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
            Avatar.GetPosition 
                context 
                avatarId
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
            (context : AtSeaGetVisibleIslandsContext)
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
            |> Avatar.GetPosition context
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
                                    context
                                    avatarId
                                    location)))
        |> List.sortBy (fun (_,_,d,_)->d)

    let private UpdateDisplay 
            (context : AtSeaUpdateDisplayContext)
            (avatarMessageSource            : AvatarMessageSource)
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
            |> Avatar.GetSpeed 
                context 
            |> Option.get
        let heading = 
            avatarId 
            |> Avatar.GetHeading 
                context 
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
            |> Avatar.GetEffectiveSpeed context 
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
            context
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
            (context                        : AtSeaHandleCommandContext)
            (random                         : Random) 
            (command                        : Command option) 
            (avatarId                       : string) 
            : Gamestate option =
        avatarId
        |> World.ClearMessages context.avatarMessagePurger

        let canCareen = 
            CanCareen 
                context
                context.islandSingleStatisticSource   
                context.islandSource
                context.vesselSingleStatisticSource 
                avatarId

        let dockDistance = 
            context.vesselSingleStatisticSource avatarId VesselStatisticIdentifier.DockDistance 
            |> Option.get 
            |> Statistic.GetCurrentValue
        let nearby = 
            avatarId
            |> GetVisibleIslands 
                context
                context.islandSource
                context.vesselSingleStatisticSource
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
            (avatarId)
            |> Gamestate.InPlay
            |> Gamestate.Status
            |> Some

        | Some (Command.HeadFor name) ->
            avatarId
            |> World.HeadFor
                context
                context.avatarIslandSingleMetricSource
                context.avatarMessageSink 
                context.islandLocationByNameSource
                context.vesselSingleStatisticSource 
                context.vesselSingleStatisticSink 
                name
            (avatarId)
            |> Gamestate.InPlay
            |> Some

        | Some (Command.DistanceTo name) ->
            avatarId
            |> World.DistanceTo 
                context
                context.avatarIslandSingleMetricSource
                context.avatarMessageSink 
                context.islandLocationByNameSource
                context.vesselSingleStatisticSource 
                name
            (avatarId)
            |> Gamestate.InPlay
            |> Some

        | Some (Command.Careen side) ->
            if canCareen then
                (side, avatarId)
                |> Gamestate.Careened
                |> Some
            else
                avatarId
                |> World.AddMessages 
                    context
                    context.avatarMessageSink 
                    [ "You cannot careen here." ]
                avatarId
                |> Gamestate.InPlay
                |> Some

        | Some Command.Dock ->
            match dockTarget with
            | Some location ->
                avatarId 
                |> World.Dock 
                    context
                    random 
                    location
                avatarId
                |> Gamestate.InPlay
                |> Some
            | None ->
                avatarId
                |> World.AddMessages 
                    context
                    context.avatarMessageSink 
                    [ "There is no place to dock." ]
                avatarId
                |> Gamestate.InPlay
                |> Some

        | Some (Command.Islands page) ->
            (page, avatarId |> Gamestate.InPlay)
            |> Gamestate.IslandList
            |> Some

        | Some (Command.Abandon Job) ->
            avatarId
            |> World.AbandonJob 
                context
                context.avatarJobSink
                context.avatarJobSource
                context.avatarMessageSink
                context.avatarSingleMetricSink
                context.avatarSingleMetricSource
                context.shipmateSingleStatisticSink
                context.shipmateSingleStatisticSource
            avatarId
            |> Gamestate.InPlay
            |> Some

        | Some Command.Metrics ->
            avatarId
            |> Gamestate.InPlay
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
            |> Gamestate.InPlay
            |> Gamestate.Help
            |> Some

        | Some (Command.Move distance)->
            avatarId
            |> World.Move 
                context
                context.avatarInventorySink
                context.avatarInventorySource
                context.avatarIslandSingleMetricSink
                context.avatarMessageSink 
                context.avatarShipmateSource
                context.avatarSingleMetricSink
                context.avatarSingleMetricSource
                context.islandSource
                context.shipmateRationItemSource 
                context.shipmateSingleStatisticSink
                context.shipmateSingleStatisticSource
                context.vesselSingleStatisticSink 
                context.vesselSingleStatisticSource 
                distance
            avatarId
            |> Gamestate.InPlay
            |> Some

        | Some (Command.Set (SetCommand.Heading heading)) ->
            avatarId
            |> World.SetHeading 
                context
                context.vesselSingleStatisticSource 
                context.vesselSingleStatisticSink 
                context.avatarMessageSink 
                heading
            avatarId
            |> Gamestate.InPlay
            |> Some

        | Some (Command.Set (Speed speed)) ->
            avatarId
            |> World.SetSpeed 
                context
                context.avatarMessageSink
                speed
            avatarId
            |> Gamestate.InPlay
            |> Some

        | Some Command.Quit -> 
            avatarId
            |> Gamestate.InPlay
            |> Gamestate.ConfirmQuit
            |> Some

        | Some Command.Inventory -> 
            avatarId
            |> Gamestate.InPlay
            |> Gamestate.Inventory
            |> Some

        | _ ->
            ("Maybe try 'help'?",avatarId
            |> Gamestate.InPlay)
            |> Gamestate.ErrorMessage
            |> Some

    let private RunAlive
            (context       : AtSeaRunContext)
            (random        : Random) 
            (commandSource : CommandSource) 
            (messageSink   : MessageSink) 
            (avatarId      : string) 
            : Gamestate option =
        UpdateDisplay 
            context
            context.avatarMessageSource
            context.islandSource
            context.shipmateSingleStatisticSource
            context.vesselSingleStatisticSource
            messageSink 
            avatarId
        HandleCommand
            context
            random
            (commandSource())
            avatarId

    let Run 
            (context       : AtSeaRunContext)
            (random        : Random) 
            (commandSource : CommandSource) 
            (messageSink   : MessageSink) 
            (avatarId      : string) 
            : Gamestate option =
        if avatarId |> World.IsAvatarAlive context then
            RunAlive
                context
                random 
                commandSource 
                messageSink 
                avatarId
        else
            avatarId
            |> context.avatarMessageSource
            |> Gamestate.GameOver
            |> Some
