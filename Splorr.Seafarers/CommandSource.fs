namespace Splorr.Seafarers

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
        | "0" :: tail ->
            None
        | number :: tail ->
            match System.UInt32.TryParse(number) with
            | true, quantity ->
                let itemName = System.String.Join(" ", tail)
                (quantity, itemName)
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
        | number :: tail ->
            match System.UInt32.TryParse(number) with
            | true, quantity ->
                let itemName = System.String.Join(" ", tail)
                (quantity, itemName)
                |> Command.Sell
                |> Some
            | _ -> None
        | _ -> None

    let private ParseSave (tokens:string list) : Command option =
        if tokens.IsEmpty then
            None
        else
            System.String.Join(' ',tokens |> List.toArray)
            |> Command.Save
            |> Some

    let Parse(tokens:string list) : Command option =
        match tokens with
        | [ "resume" ] -> 
            Command.Resume 
            |> Some
        | "abandon" :: tail -> 
            tail
            |> ParseAbandon
        | [ "shop" ] -> 
            Command.Shop 
            |> Some
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
        | [ "inventory" ] ->
            Command.Inventory
            |> Some
        | [ "prices" ] ->
            Command.Prices
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
        | "accept" :: tail ->
            tail
            |> ParseAccept
        | "buy" :: tail ->
            tail
            |> ParseBuy
        | "sell" :: tail ->
            tail
            |> ParseSell
        | "save" :: tail ->
            tail
            |> ParseSave
        | _ -> 
            None

    let Read() : Command option =
        System.Console.Write ">"
        System.Console.ReadLine().ToLower().Split([|' '|]) 
        |> List.ofArray
        |> Parse

