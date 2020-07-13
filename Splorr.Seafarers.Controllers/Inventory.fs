﻿namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module Inventory =
    let private RunWorld (sink:MessageSink) (avatarId:string) (world:World) : unit =
        [
            "" |> Line
            (Heading, "Item" |> sprintf "%-20s" |> Text) |> Hued
            (Label, " | " |> Text) |> Hued
            (Heading, "Qty" |> sprintf "%6s" |> Text) |> Hued
            (Label, " | " |> Text) |> Hued
            (Heading, "Tonnage" |> sprintf "%8s" |> Line) |> Hued
            (Label, "---------------------+--------+---------" |> Line ) |> Hued
        ]
        |> List.iter sink
        let avatar = world.Avatars.[avatarId]
        let inventoryEmpty =
            avatar.Inventory
            |> Map.fold
                (fun _ item quantity -> 
                    let descriptor = world.Items.[item]
                    let tonnage = descriptor.Tonnage * (quantity |> float)
                    [
                        (Value, descriptor.DisplayName |> sprintf "%-20s" |> Text) |> Hued
                        (Label, " | " |> Text) |> Hued
                        (Value, quantity |> sprintf "%6u" |> Text) |> Hued
                        (Label, " | " |> Text) |> Hued
                        (Value, tonnage |> sprintf "%8.1f" |> Line) |> Hued
                    ]
                    |> List.iter sink
                    false) true
        if inventoryEmpty then 
            (Usage, "(none)"  |> Line) |> Hued
            |> sink
        let availableTonnage = avatar.Vessel.Tonnage
        let usedTonnage = avatar |> Avatar.GetUsedTonnage world.Items
        [
            (Sublabel, "Cargo Limit: " |> Text) |> Hued
            (Value, usedTonnage |> sprintf "%.1f" |> Text) |> Hued
            (Sublabel, "/" |> Text) |> Hued
            (Value, availableTonnage |> sprintf "%.1f" |> Line) |> Hued
        ]
        |> List.iter sink

    let Run (sink:MessageSink) (avatarId:string) (gamestate:Gamestate) : Gamestate option =
        gamestate 
        |> Gamestate.GetWorld
        |> Option.iter (RunWorld sink avatarId)
        gamestate
        |> Some
