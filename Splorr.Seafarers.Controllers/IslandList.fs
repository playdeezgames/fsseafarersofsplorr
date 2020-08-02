namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module IslandList =
    let private RunWorld 
            (messageSink : MessageSink) 
            (pageSize    : uint32) 
            (page        : uint32) 
            (world       : World) 
            : unit = 
        [
            "" |> Line
            (Hue.Heading, "Known Islands:" |> Line) |> Hued
        ]
        |> List.iter messageSink
        let knownIslands =
            world.Islands
            |> Map.toList
            |> List.filter
                (fun (_,island) -> 
                    island.AvatarVisits.TryFind world.AvatarId 
                    |> Option.map (fun i -> i.VisitCount.IsSome)
                    |> Option.defaultValue false)
            |> List.sortBy(fun (_,x)->x.Name)
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
            knownIslands
            |> List.skip (skippedItems |> int)
            |> List.take ((Utility.Lesser pageSize (totalItems-skippedItems)) |> int)
            |> List.iter (fun (location, island) -> 
                let distance =
                    Location.DistanceTo world.Avatars.[world.AvatarId].Position location
                let bearing =
                    Location.HeadingTo world.Avatars.[world.AvatarId].Position location
                    |> Angle.ToDegrees
                    |> Angle.ToString
                [
                    (Hue.Value, island.Name |> sprintf "%s" |> Text) |> Hued
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
            (messageSink : MessageSink) 
            (page        : uint32) 
            (gamestate   : Gamestate) 
            : Gamestate option =
        gamestate 
        |> Gamestate.GetWorld
        |> Option.iter (RunWorld messageSink pageSize page )
        gamestate
        |> Some