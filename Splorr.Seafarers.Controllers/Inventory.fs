namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models

module Inventory =
    let private RunWorld (sink:MessageSink) (avatarId:string) (world:World) : unit =
        [
            "" |> Line
            (Heading, "Inventory:" |> Line) |> Hued
        ]
        |> List.iter sink
        let inventoryEmpty =
            world.Avatars.[avatarId].Inventory
            |> Map.fold
                (fun _ item quantity -> 
                    let descriptor = world.Items.[item]
                    sprintf "%s - %u" descriptor.DisplayName quantity |> Line |> sink
                    false) true
        if inventoryEmpty then 
            "\t(none)"  |> Line
            |> sink

    let Run (sink:MessageSink) (avatarId:string) (gamestate:Gamestate) : Gamestate option =
        gamestate 
        |> Gamestate.GetWorld
        |> Option.iter (RunWorld sink avatarId)
        gamestate
        |> Some
