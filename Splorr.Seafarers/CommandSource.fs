﻿namespace Splorr.Seafarers

open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Models

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
        |> Option.map(fun x -> x |> Heading |> Command.Set)

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

    let ParseHead (tokens:string list) : Command option =
        match tokens with
        | "for" :: [ name ] ->
            name
            |> Command.HeadFor
            |> Some
        | _ -> None

    let Parse(tokens:string list) : Command option =
        match tokens with
        | [ "resume" ] -> 
            Command.Resume 
            |> Some
        | [ "abandon" ] -> 
            Command.Abandon 
            |> Some
        | [ "quit" ] -> 
            Command.Quit 
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
        | [ "move" ] ->
            Command.Move
            |> Some
        | [ "help" ] ->
            Command.Help
            |> Some
        | [ "start" ] ->
            Command.Start
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
        | [ "menu" ] ->
            Command.Menu
            |> Some
        | "islands" :: tail ->
            tail
            |> ParseIslands
        | "head" :: tail ->
            tail
            |> ParseHead
        | _ -> 
            None

    let Read() : Command option =
        System.Console.Write ">"
        System.Console.ReadLine().ToLower().Split([|' '|]) 
        |> List.ofArray
        |> Parse

