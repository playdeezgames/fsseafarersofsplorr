namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open Splorr.Seafarers.Persistence
open Splorr.Common

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
            (context : CommonContext)
            (avatarId                    : string) 
            : bool =
        let viewDistance =
            World.GetVesselStatistic context avatarId VesselStatisticIdentifier.ViewDistance 
            |> Option.get 
            |> Statistic.GetCurrentValue
        let avatarPosition =
            World.GetVesselPosition 
                context 
                avatarId
        World.GetNearbyLocations
            context
            avatarPosition 
            viewDistance
        |> List.map 
            (fun l -> 
                (l,
                    World.GetIslandStatistic context IslandStatisticIdentifier.CareenDistance l 
                    |> Option.get
                    |> Statistic.GetCurrentValue))
        |> List.exists (fun (l,d) -> Location.DistanceTo l avatarPosition < d)

    let private GetVisibleIslands 
            (context : CommonContext)
            (avatarId                       : string) 
            : (Location * string * float * string) list =
        let viewDistance =
            World.GetVesselStatistic context avatarId VesselStatisticIdentifier.ViewDistance 
            |> Option.get 
            |> Statistic.GetCurrentValue
        let avatarPosition =
            avatarId
            |> World.GetVesselPosition context
        World.GetNearbyLocations
            context
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
                                (World.GetIslandDisplayName 
                                    context
                                    avatarId
                                    location)))
        |> List.sortBy (fun (_,_,d,_)->d)

    let private UpdateDisplay 
            (context : CommonContext)
            (messageSink                    : MessageSink) 
            (avatarId                       : string) 
            : unit =
        "" |> Line |> messageSink
        avatarId
        |> World.GetAvatarMessages context
        |> Utility.DumpMessages messageSink

        let speed = 
            avatarId
            |> World.GetVesselSpeed 
                context 
            |> Option.get
        let heading = 
            avatarId 
            |> World.GetVesselHeading 
                context 
            |> Option.get
        let speedHue =DetermineSpeedHue speed
        let turn = World.GetShipmateStatistic context avatarId Primary ShipmateStatisticIdentifier.Turn |> Option.get
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
            |> World.GetVesselEffectiveSpeed context 
            |> sprintf "(Effective rate: %.2f)" |> Line
            (Hue.Subheading, "Nearby:" |> Line) |> Hued
        ]
        |> List.iter messageSink

        let dockDistance = 
            World.GetVesselStatistic context avatarId VesselStatisticIdentifier.DockDistance 
            |> Option.get 
            |> Statistic.GetCurrentValue
        avatarId
        |> GetVisibleIslands 
            context
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
            (context                        : CommonContext)
            (command                        : Command option) 
            (avatarId                       : string) 
            : Gamestate option =
        avatarId
        |> World.ClearMessages context

        let canCareen = 
            CanCareen 
                context
                avatarId

        let dockDistance = 
            World.GetVesselStatistic context avatarId VesselStatisticIdentifier.DockDistance 
            |> Option.get 
            |> Statistic.GetCurrentValue
        let nearby = 
            avatarId
            |> GetVisibleIslands 
                context
        let dockTarget = 
            nearby
            |> List.fold
                (fun target (location, _, distance, _) -> 
                    (if distance<dockDistance then 
                        (Some location) 
                    else 
                        target)) None

        match (BaseGameState.HandleCommand context command avatarId),command with
        | Some newState, _ ->
            newState
            |> Some

        | _, Some (Command.HeadFor name) ->
            avatarId
            |> World.HeadFor
                context
                name
            (avatarId)
            |> Gamestate.InPlay
            |> Some

        | _, Some (Command.DistanceTo name) ->
            avatarId
            |> World.DistanceTo 
                context
                name
            (avatarId)
            |> Gamestate.InPlay
            |> Some

        | _, Some (Command.Careen side) ->
            if canCareen then
                (side, avatarId)
                |> Gamestate.Careened
                |> Some
            else
                avatarId
                |> World.AddMessages 
                    context
                    [ "You cannot careen here." ]
                avatarId
                |> Gamestate.InPlay
                |> Some

        | _, Some Command.Dock ->
            match dockTarget with
            | Some location ->
                avatarId 
                |> World.Dock 
                    context
                    location
                avatarId
                |> Gamestate.InPlay
                |> Some
            | None ->
                avatarId
                |> World.AddMessages 
                    context
                    [ "There is no place to dock." ]
                avatarId
                |> Gamestate.InPlay
                |> Some

        | _, Some (Command.Chart x) ->
            (x, avatarId)
            |> Gamestate.Chart
            |> Some

        | _, Some (Command.Move distance)->
            avatarId
            |> World.Move 
                context
                distance
            avatarId
            |> Gamestate.InPlay
            |> Some

        | _, Some (Command.Set (SetCommand.Heading heading)) ->
            avatarId
            |> World.SetHeading 
                context
                heading
            avatarId
            |> Gamestate.InPlay
            |> Some

        | _, Some (Command.Set (Speed speed)) ->
            avatarId
            |> World.SetSpeed 
                context
                speed
            avatarId
            |> Gamestate.InPlay
            |> Some

        | _ ->
            BaseGameState.HandleCommand context None avatarId

    let private RunAlive
            (context       : CommonContext)
            (commandSource : CommandSource) 
            (messageSink   : MessageSink) 
            (avatarId      : string) 
            : Gamestate option =
        UpdateDisplay 
            context
            messageSink 
            avatarId
        HandleCommand
            context
            (commandSource())
            avatarId

    let Run 
            (context       : CommonContext)
            (commandSource : CommandSource) 
            (messageSink   : MessageSink) 
            (avatarId      : string) 
            : Gamestate option =
        if avatarId |> World.IsAvatarAlive context then
            RunAlive
                context
                commandSource 
                messageSink 
                avatarId
        else
            avatarId
            |> World.GetAvatarMessages context
            |> Gamestate.GameOver
            |> Some
