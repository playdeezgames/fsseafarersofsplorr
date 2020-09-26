namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module IslandList =
    let private RunWorld 
            (context: ServiceContext)
            (messageSink                    : MessageSink) 
            (pageSize                       : uint32) 
            (page                           : uint32) 
            (avatarId                       : string) 
            : unit = 
        [
            "" |> Line
            (Hue.Heading, "Known Islands:" |> Line) |> Hued
        ]
        |> List.iter messageSink
        let knownIslands =
            context
            |> Island.GetList
            |> List.filter
                (fun location -> 
                    avatarId
                    |> Avatar.GetIslandMetric context location AvatarIslandMetricIdentifier.VisitCount 
                    |> Option.map (fun _ -> true)
                    |> Option.defaultValue false)
            |> List.sortBy(Island.GetName context >> Option.get)
        let totalItems = knownIslands |> List.length |> uint32
        let totalPages = (totalItems + (pageSize-1u)) / pageSize
        let skippedItems = page * pageSize
        [
            (Hue.Subheading, "Page " |> Text) |> Hued
            (Hue.Value, (page+1u) |> sprintf "%u" |> Text) |> Hued
            (Hue.Subheading, " of " |> Text) |> Hued
            (Hue.Value, totalPages |> sprintf "%u" |> Line) |> Hued
        ]
        |> List.iter messageSink
        if page < totalPages then
            let avatarPosition = 
                avatarId
                |> Avatar.GetPosition context
                |> Option.get
            knownIslands
            |> List.skip (skippedItems |> int)
            |> List.take ((Utility.Lesser pageSize (totalItems-skippedItems)) |> int)
            |> List.iter (fun location -> 
                let distance =
                    Location.DistanceTo avatarPosition location
                let bearing =
                    Location.HeadingTo avatarPosition location
                    |> Angle.ToDegrees
                    |> Angle.ToString
                [
                    (Hue.Value, location |> Island.GetName context |> Option.get |> sprintf "%s" |> Text) |> Hued
                    (Hue.Sublabel, " Bearing:" |> Text) |> Hued
                    (Hue.Value, bearing |> sprintf "%s" |> Text) |> Hued
                    (Hue.Sublabel, " Distance:" |> Text) |> Hued
                    (Hue.Value, distance |> sprintf "%f" |> Line) |> Hued
                ]
                |> List.iter messageSink)
        else
            (Hue.Usage ,"(end of list)" |> Line) |> Hued |> messageSink
    
    let private pageSize = 20u

    let Run 
            (context : ServiceContext)
            (messageSink                    : MessageSink) 
            (page                           : uint32) 
            (gamestate                      : Gamestate) 
            : Gamestate option =
        gamestate 
        |> Gamestate.GetWorld
        |> Option.iter 
            (RunWorld 
                context
                messageSink 
                pageSize 
                page)
        gamestate
        |> Some