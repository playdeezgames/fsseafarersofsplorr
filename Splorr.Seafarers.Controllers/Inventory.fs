namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module Inventory =
    let private RunWorld
            (context : ServiceContext)
            (messageSink                 : MessageSink) 
            (avatarId                    : string) 
            : unit =
        [
            "" |> Line
            (Hue.Heading, "Item" |> sprintf "%-20s" |> Text) |> Hued
            (Hue.Label, " | " |> Text) |> Hued
            (Hue.Heading, "Qty" |> sprintf "%6s" |> Text) |> Hued
            (Hue.Label, " | " |> Text) |> Hued
            (Hue.Heading, "Tonnage" |> sprintf "%8s" |> Line) |> Hued
            (Hue.Label, "---------------------+--------+---------" |> Line ) |> Hued
        ]
        |> List.iter messageSink
        let items = Item.GetList context
        let inventoryEmpty =
            avatarId
            |> AvatarInventory.GetInventory context
            |> Map.fold
                (fun _ item quantity -> 
                    let descriptor = items.[item]
                    let tonnage = descriptor.Tonnage * (quantity |> float)
                    [
                        (Hue.Value, descriptor.ItemName |> sprintf "%-20s" |> Text) |> Hued
                        (Hue.Label, " | " |> Text) |> Hued
                        (Hue.Value, quantity |> sprintf "%6u" |> Text) |> Hued
                        (Hue.Label, " | " |> Text) |> Hued
                        (Hue.Value, tonnage |> sprintf "%8.1f" |> Line) |> Hued
                    ]
                    |> List.iter messageSink
                    false) true
        if inventoryEmpty then 
            (Hue.Usage, "(none)"  |> Line) |> Hued
            |> messageSink
        let availableTonnage = 
            Vessel.GetStatistic context avatarId VesselStatisticIdentifier.Tonnage
            |> Option.map Statistic.GetCurrentValue
            |> Option.get
        let usedTonnage = 
            avatarId 
            |> AvatarInventory.GetUsedTonnage 
                context
                items
        [
            (Hue.Sublabel, "Cargo Limit: " |> Text) |> Hued
            (Hue.Value, usedTonnage |> sprintf "%.1f" |> Text) |> Hued
            (Hue.Sublabel, "/" |> Text) |> Hued
            (Hue.Value, availableTonnage |> sprintf "%.1f" |> Line) |> Hued
        ]
        |> List.iter messageSink

    let Run
            (context : ServiceContext)
            (messageSink                 : MessageSink) 
            (gamestate                   : Gamestate) 
            : Gamestate option =
        gamestate 
        |> Gamestate.GetWorld
        |> Option.iter 
            (RunWorld
                context
                messageSink)
        gamestate
        |> Some
