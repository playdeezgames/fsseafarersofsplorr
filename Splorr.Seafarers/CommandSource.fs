namespace Splorr.Seafarers

open Splorr.Seafarers.Views

module CommandSource=
    let private ParseSetSpeed(tokens: string list) : Command option =
        match tokens with
        | [ x ] ->
            match System.Double.TryParse x with
            | true, v ->
                v |> Speed |> Set |> Some
            | _ -> None
        | _ -> None

    let private ParseSet(tokens: string list) : Command option =
        match tokens with
        | "speed" :: tail ->
            tail
            |> ParseSetSpeed 
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
        | _ -> 
            None

    let Read() : Command option =
        System.Console.Write ">"
        System.Console.ReadLine().ToLower().Split([|' '|]) 
        |> List.ofArray
        |> Parse

