namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

type IslandListRunWorldContext =
    inherit OperatingContext
    abstract member avatarIslandSingleMetricSource : AvatarIslandSingleMetricSource
    abstract member islandSingleNameSource         : IslandSingleNameSource
    abstract member islandSource                   : IslandSource

module IslandList =
    let private RunWorld 
            (context: OperatingContext)
            (messageSink                    : MessageSink) 
            (pageSize                       : uint32) 
            (page                           : uint32) 
            (avatarId                       : string) 
            : unit = 
        let context = context :?> IslandListRunWorldContext
        [
            "" |> Line
            (Hue.Heading, "Known Islands:" |> Line) |> Hued
        ]
        |> List.iter messageSink
        let knownIslands =
            context.islandSource()
            |> List.filter
                (fun location -> 
                    context.avatarIslandSingleMetricSource avatarId location AvatarIslandMetricIdentifier.VisitCount 
                    |> Option.map (fun _ -> true)
                    |> Option.defaultValue false)
            |> List.sortBy(fun l->context.islandSingleNameSource l |> Option.get)
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
                    (Hue.Value, context.islandSingleNameSource location |> Option.get |> sprintf "%s" |> Text) |> Hued
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
            (context : OperatingContext)
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