namespace Splorr.Seafarers

open Splorr.Seafarers.Views
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

    let private Parse(tokens:string list) : Command option =
        match tokens with
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
        | _ -> 
            None

    let Read() : Command option =
        System.Console.Write ">"
        System.Console.ReadLine().ToLower().Split([|' '|]) 
        |> List.ofArray
        |> Parse

