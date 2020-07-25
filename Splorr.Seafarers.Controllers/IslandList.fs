﻿namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module IslandList =
    let private lesser (a:uint32) (b:uint32) = if a<b then a else b

    let private RunWorld (sink:MessageSink) (page:uint32) (pageSize:uint32) (world:World) : unit = 
        [
            "" |> Line
            (Hue.Heading, "Known Islands:" |> Line) |> Hued
        ]
        |> List.iter sink
        let knownIslands =
            world.Islands
            |> Map.toList
            |> List.filter(fun (_,x) -> x.AvatarVisits.TryFind world.AvatarId |> Option.isSome)
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
        |> List.iter sink
        if page < totalPages then
            knownIslands
            |> List.skip (skippedItems |> int)
            |> List.take ((lesser pageSize (totalItems-skippedItems)) |> int)
            |> List.iter (fun (location, island) -> 
                let distance =
                    Location.DistanceTo world.Avatars.[world.AvatarId].Position location
                let bearing =
                    Location.HeadingTo world.Avatars.[world.AvatarId].Position location
                    |> Dms.ToDegrees
                    |> Dms.ToString
                [
                    (Hue.Value, island.Name |> sprintf "%s" |> Text) |> Hued
                    (Hue.Sublabel, " Bearing:" |> Text) |> Hued
                    (Hue.Value, bearing |> sprintf "%s" |> Text) |> Hued
                    (Hue.Sublabel, " Distance:" |> Text) |> Hued
                    (Hue.Value, distance |> sprintf "%f" |> Line) |> Hued
                ]
                |> List.iter sink)
        else
            (Hue.Usage ,"(end of list)" |> Line) |> Hued |> sink
    
    let private pageSize = 20u


    let Run (sink:MessageSink) (page:uint32) (gamestate: Gamestate) : Gamestate option =
        gamestate 
        |> Gamestate.GetWorld
        |> Option.iter (RunWorld sink page pageSize)
        gamestate
        |> Some