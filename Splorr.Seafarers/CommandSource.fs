namespace Splorr.Seafarers

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

    let private ParseBet: 
            string list -> Command option =
        function
        | [ number ] ->
            match Double.TryParse(number) with
            | true, amount when amount > 0.0->
                amount
                |> Some
                |> Command.Bet
                |> Some
            | _ -> None
        | _ -> 
            None


    let ParseBuy: 
            string list -> Command option =
        function
        | [ _ ] ->
            None
        | "maximum" :: tail ->
            let itemName = String.Join(" ", tail)
            (Maximum, itemName)
            |> Command.Buy
            |> Some
        | "0" :: _ ->
            None
        | number :: tail ->
            match UInt64.TryParse(number) with
            | true, quantity ->
                let itemName = String.Join(" ", tail)
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
        | "0" :: _ ->
            None
        | "all" :: tail ->
            let itemName = String.Join(" ", tail)
            (Maximum, itemName)
            |> Command.Sell
            |> Some
        | number :: tail ->
            match UInt64.TryParse(number) with
            | true, quantity ->
                let itemName = String.Join(" ", tail)
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

    let private simpleCommandMap : Map<string list, Command> =
        Map.empty
        |> Map.add [ "clean"; "hull"] Command.CleanHull
        |> Map.add [ "clean"; "the"; "hull"] Command.CleanHull
        |> Map.add [ "deal" ] Command.Gamble
        |> Map.add [ "dock" ] Command.Dock
        |> Map.add [ "enter" ; "alley" ] (IslandFeatureIdentifier.DarkAlley |> Command.GoTo)
        |> Map.add [ "enter" ; "dark"; "alley" ] (IslandFeatureIdentifier.DarkAlley |> Command.GoTo)
        |> Map.add [ "enter" ; "the"; "alley" ] (IslandFeatureIdentifier.DarkAlley |> Command.GoTo)
        |> Map.add [ "enter" ; "the"; "dark"; "alley" ] (IslandFeatureIdentifier.DarkAlley |> Command.GoTo)
        |> Map.add [ "gamble" ] Command.Gamble
        |> Map.add [ "items" ] Command.Items
        |> Map.add [ "help"  ] Command.Help
        |> Map.add [ "inventory" ] Command.Inventory
        |> Map.add [ "jobs" ] Command.Jobs
        |> Map.add [ "leave" ] Command.Leave
        |> Map.add [ "menu" ] Command.Menu
        |> Map.add [ "metrics" ] Command.Metrics
        |> Map.add [ "no" ] Command.No
        |> Map.add [ "no"; "bet" ] (Command.Bet None)
        |> Map.add [ "quit" ] Command.Quit
        |> Map.add [ "resume" ] Command.Resume
        |> Map.add [ "rules" ] Command.Rules
        |> Map.add [ "undock" ] Command.Undock
        |> Map.add [ "status" ] Command.Status
        |> Map.add [ "weigh"; "anchor" ] Command.WeighAnchor
        |> Map.add [ "yes" ] Command.Yes

    let Parse 
            (input : string list) 
            : Command option =
        match input with
        | "chart" :: tail ->
            String.Join(" ", tail) 
            |> Command.Chart 
            |> Some

        | ["save"] ->
            None
            |> Command.Save
            |> Some

        | "save" :: tail ->
            String.Join(" ", tail) 
            |> Some
            |> Command.Save
            |> Some


        | "careen" :: tail ->
            tail
            |> ParseCareen 

        | "bet" :: tail ->
            tail
            |> ParseBet 


        | [ "start" ] ->
            Guid.NewGuid().ToString()
            |> Command.Start
            |> Some

        | "abandon" :: tail -> 
            tail
            |> ParseAbandon

        | "set" :: tail ->
            tail
            |> ParseSet

        | "move" :: tail ->
            tail
            |> ParseMove

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

        | _ -> 
            simpleCommandMap
            |> Map.tryFind input

    let Read 
            (lineReader: unit -> string) 
            : Command option =
        Console.Write ">"
        lineReader().ToLower().Split([|' '|]) 
        |> List.ofArray
        |> Parse

