namespace Splorr.Seafarers

open Splorr.Seafarers.Views

module CommandSource=
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
        | _ -> 
            None

    let Read() : Command option =
        System.Console.Write ">"
        System.Console.ReadLine().ToLower().Split([|' '|]) 
        |> List.ofArray
        |> Parse

