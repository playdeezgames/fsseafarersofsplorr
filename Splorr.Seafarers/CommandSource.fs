namespace Splorr.Seafarers

open Splorr.Seafarers.Controllers
open Splorr.Seafarers.Models

module CommandSource=
    let private ParseSetSpeed(tokens: string list) : Command option =
        match tokens with
        | [ x ] ->
            match System.Double.TryParse x with
            | true, v ->
                v |> Speed |> Set |> Some
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
        |> Option.map(fun x -> x |> Heading |> Set)

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
        | [] -> 0u |> Islands |> Some
        | [ token ] ->
            match System.UInt32.TryParse token with
            | true, page when page > 0u ->
                (page - 1u) |> Islands |> Some
            | _ -> None
        | _ -> None


    let Parse(tokens:string list) : Command option =
        match tokens with
        | [ "resume" ] -> 
            Resume 
            |> Some
        | [ "abandon" ] -> 
            Abandon 
            |> Some
        | [ "quit" ] -> 
            Quit 
            |> Some
        | [ "yes" ] ->
            Yes
            |> Some
        | [ "no" ] ->
            No
            |> Some
        | "set" :: tail ->
            tail
            |> ParseSet
        | [ "move" ] ->
            Move
            |> Some
        | [ "help" ] ->
            Command.Help
            |> Some
        | [ "start" ] ->
            Start
            |> Some
        | [ "dock" ] ->
            Dock
            |> Some
        | [ "undock" ] ->
            Undock
            |> Some
        | [ "menu" ] ->
            Menu
            |> Some
        | "islands" :: tail ->
            tail
            |> ParseIslands
        | _ -> 
            None

    let Read() : Command option =
        System.Console.Write ">"
        System.Console.ReadLine().ToLower().Split([|' '|]) 
        |> List.ofArray
        |> Parse

