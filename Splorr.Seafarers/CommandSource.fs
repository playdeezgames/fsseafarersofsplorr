﻿namespace Splorr.Seafarers

open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module CommandSource=
    let private ParseSetSpeed(tokens: string list) : Command option =
        match tokens with
        | [ x ] ->
            match System.Double.TryParse x with
            | true, v ->
                v |> Speed |> Command.Set |> Some
            | _ -> None
        | _ -> None

    let private ParseDms(d:string, m: string, s:string) : Dms option =
        match System.Int32.TryParse(d), System.Int32.TryParse(m), System.Double.TryParse(s) with
        | (true, degrees), (true, minutes), (true, seconds) ->
            {
                Degrees = degrees
                Minutes = minutes
                Seconds = seconds
            }
            |> Some
        | _ -> None

    let private ParseSetHeading(tokens:string list) : Command option = 
        (match tokens with
        | [degrees; minutes; seconds] ->
            ParseDms(degrees,minutes,seconds)
        | [degrees; minutes] ->
            ParseDms(degrees,minutes,"0.0")
        | [degrees] ->
            ParseDms(degrees,"0","0.0")
        | _ -> None)
        |> Option.map(fun x -> x |> SetCommand.Heading |> Command.Set)

    let private ParseSet(tokens: string list) : Command option =
        match tokens with
        | "speed" :: tail ->
            tail
            |> ParseSetSpeed 
        | "heading" :: tail ->
            tail
            |> ParseSetHeading
        | _ -> None

    let private ParseIslands(tokens: string list) : Command option =
        match tokens with
        | [] -> 0u |> Command.Islands |> Some
        | [ token ] ->
            match System.UInt32.TryParse token with
            | true, page when page > 0u ->
                (page - 1u) |> Command.Islands |> Some
            | _ -> None
        | _ -> None

    let private ParseHead (tokens:string list) : Command option =
        match tokens with
        | "for" :: [ name ] ->
            name
            |> Command.HeadFor
            |> Some
        | _ -> None

    let private ParseDistance (tokens:string list) : Command option =
        match tokens with
        | "to" :: [ name ] ->
            name
            |> Command.DistanceTo
            |> Some
        | _ -> None

    let private ParseAccept (tokens:string list) : Command option =
        match tokens with
        | "job" :: [ number ] ->
            match System.UInt32.TryParse(number) with
            | true, value -> 
                value |> Command.AcceptJob |> Some
            | _ -> None
        | _ -> None

    let private ParseAbandon (tokens:string list) : Command option =
        match tokens with 
        | [ "game" ] -> 
            Game
            |> Command.Abandon
            |> Some
        | [ "job" ] -> 
            Job
            |> Command.Abandon
            |> Some
        | _ -> None

    let private ParseMove (tokens:string list) : Command option =
        match tokens with
        | [ "0" ] -> 
            None
        | [ number ] ->
            match System.UInt32.TryParse(number) with
            | true, distance ->
                distance
                |> Command.Move
                |> Some
            | _ -> None
        | _ -> 
            1u 
            |> Command.Move 
            |> Some

    let ParseBuy (tokens:string list) : Command option =
        match tokens with
        | [ _ ] ->
            None
        | "maximum" :: tail ->
            let itemName = System.String.Join(" ", tail)
            (Maximum, itemName)
            |> Command.Buy
            |> Some
        | "0" :: tail ->
            None
        | number :: tail ->
            match System.UInt32.TryParse(number) with
            | true, quantity ->
                let itemName = System.String.Join(" ", tail)
                (quantity |> Specific, itemName)
                |> Command.Buy
                |> Some
            | _ -> None
        | _ -> None

    let ParseSell (tokens:string list) : Command option =
        match tokens with
        | [ _ ] ->
            None
        | "0" :: tail ->
            None
        | "all" :: tail ->
            let itemName = System.String.Join(" ", tail)
            (Maximum, itemName)
            |> Command.Sell
            |> Some
        | number :: tail ->
            match System.UInt32.TryParse(number) with
            | true, quantity ->
                let itemName = System.String.Join(" ", tail)
                (Specific quantity, itemName)
                |> Command.Sell
                |> Some
            | _ -> None
        | _ -> None

    let private ParseCareen (tokens:string list) : Command option =
        match tokens with 
        | ["to";"port"] ->
            Port |> Command.Careen |> Some
        | ["to";"starboard"] ->
            Starboard |> Command.Careen |> Some
        | _ -> None

    let Parse(tokens:string list) : Command option =
        match tokens with
        | "chart" :: tail ->
            System.String.Join(" ", tail) |> Command.Chart |> Some

        | "careen" :: tail ->
            tail
            |> ParseCareen 

        | [ "resume" ] -> 
            Command.Resume 
            |> Some
        | [ "metrics" ] -> 
            Command.Metrics 
            |> Some
        | "abandon" :: tail -> 
            tail
            |> ParseAbandon
        | [ "quit" ] -> 
            Command.Quit 
            |> Some
        | [ "items" ] ->
            Command.Items
            |> Some
        | [ "yes" ] ->
            Command.Yes
            |> Some
        | [ "no" ] ->
            Command.No
            |> Some
        | "set" :: tail ->
            tail
            |> ParseSet
        | "move" :: tail ->
            tail
            |> ParseMove
        | [ "help" ] ->
            Command.Help
            |> Some
        | [ "start" ] ->
            System.Guid.NewGuid().ToString()
            |> Command.Start
            |> Some
        | [ "dock" ] ->
            Command.Dock
            |> Some
        | [ "jobs" ] ->
            Command.Jobs
            |> Some
        | [ "undock" ] ->
            Command.Undock
            |> Some
        | [ "status" ] ->
            Command.Status
            |> Some
        | [ "inventory" ] ->
            Command.Inventory
            |> Some
        | [ "menu" ] ->
            Command.Menu
            |> Some
        | "islands" :: tail ->
            tail
            |> ParseIslands
        | "head" :: tail ->
            tail
            |> ParseHead
        | "distance" :: tail ->
            tail
            |> ParseDistance
        | "accept" :: tail ->
            tail
            |> ParseAccept
        | "buy" :: tail ->
            tail
            |> ParseBuy
        | "sell" :: tail ->
            tail
            |> ParseSell
        | ["weigh";"anchor"] ->
            Command.WeighAnchor
            |> Some
        | ["clean";"the";"hull"]
        | ["clean";"hull"] ->
            Command.CleanHull
            |> Some
        | _ -> 
            None

    let Read (lineReader:unit->string) : Command option =
        System.Console.Write ">"
        lineReader().ToLower().Split([|' '|]) 
        |> List.ofArray
        |> Parse

