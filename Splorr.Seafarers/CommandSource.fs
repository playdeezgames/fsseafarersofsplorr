﻿namespace Splorr.Seafarers

open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open System

module CommandSource=
    let private ParseSetSpeed:
            string list -> Command option = 
        function
        | [ x ] ->
            match Double.TryParse x with
            | true, v ->
                v |> Speed |> Command.Set |> Some
            | _ -> None
        | _ -> None

    let private ParseDms
            (d:string) 
            : float option =
        match Double.TryParse(d) with
        | (true, degrees) ->
            degrees
            |> Some
        | _ -> None

    let private ParseSetHeading
            (tokens:string list) 
            : Command option = 
        (match tokens with
        | [degrees] ->
            ParseDms(degrees)
        | _ -> None)
        |> Option.map(fun x -> x |> SetCommand.Heading |> Command.Set)

    let private ParseSet:
            string list -> Command option =
        function
        | "speed" :: tail ->
            tail
            |> ParseSetSpeed 
        | "heading" :: tail ->
            tail
            |> ParseSetHeading
        | _ -> None

    let private ParseIslands: 
            string list -> Command option =
        function
        | [] -> 0u |> Command.Islands |> Some
        | [ token ] ->
            match UInt32.TryParse token with
            | true, page when page > 0u ->
                (page - 1u) |> Command.Islands |> Some
            | _ -> None
        | _ -> None

    let private ParseHead: 
            string list -> Command option =
        function
        | "for" :: [ name ] ->
            name
            |> Command.HeadFor
            |> Some
        | _ -> None

    let private ParseDistance: 
            string list -> Command option =
        function
        | "to" :: [ name ] ->
            name
            |> Command.DistanceTo
            |> Some
        | _ -> None

    let private ParseAccept: 
            string list -> Command option =
        function
        | "job" :: [ number ] ->
            match UInt32.TryParse(number) with
            | true, value -> 
                value |> Command.AcceptJob |> Some
            | _ -> None
        | _ -> None

    let private ParseAbandon: 
            string list -> Command option =
        function 
        | [ "game" ] -> 
            Game
            |> Command.Abandon
            |> Some
        | [ "job" ] -> 
            Job
            |> Command.Abandon
            |> Some
        | _ -> None

    let private ParseMove: 
            string list -> Command option =
        function
        | [ "0" ] -> 
            None
        | [ number ] ->
            match UInt32.TryParse(number) with
            | true, distance ->
                distance
                |> Command.Move
                |> Some
            | _ -> None
        | _ -> 
            1u 
            |> Command.Move 
            |> Some

    let ParseBuy: 
            string list -> Command option =
        function
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
            match UInt64.TryParse(number) with
            | true, quantity ->
                let itemName = System.String.Join(" ", tail)
                (quantity |> Specific, itemName)
                |> Command.Buy
                |> Some
            | _ -> None
        | _ -> None

    let ParseSell: 
            string list -> Command option =
        function
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
            match UInt64.TryParse(number) with
            | true, quantity ->
                let itemName = System.String.Join(" ", tail)
                (Specific quantity, itemName)
                |> Command.Sell
                |> Some
            | _ -> None
        | _ -> None

    let private ParseCareen: 
            string list -> Command option =
        function 
        | ["to";"port"] ->
            Port |> Command.Careen |> Some
        | ["to";"starboard"] ->
            Starboard |> Command.Careen |> Some
        | _ -> None

    let Parse: 
            string list -> Command option =
        function
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
        | [ "weigh"; "anchor" ] ->
            Command.WeighAnchor
            |> Some
        | [ "clean"; "the"; "hull"]
        | [ "clean"; "hull"] ->
            Command.CleanHull
            |> Some
        | _ -> 
            None

    let Read 
            (lineReader: unit -> string) 
            : Command option =
        System.Console.Write ">"
        lineReader().ToLower().Split([|' '|]) 
        |> List.ofArray
        |> Parse

