namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services
open Splorr.Common

module ItemList = 
    let private RunWithIsland 
            (context            : CommonContext)
            (messageSink        : MessageSink) 
            (location           : Location) 
            (avatarId           : string) 
            : Gamestate option =
        [
            "" |> Line
            (Hue.Heading, "Item" |> sprintf "%-20s" |> Text) |> Hued
            (Hue.Sublabel, " | " |> Text) |> Hued
            (Hue.Heading, "Selling" |> sprintf "%8s" |> Text) |> Hued
            (Hue.Sublabel, " | " |> Text) |> Hued
            (Hue.Heading, "Offering" |> sprintf "%8s" |> Line) |> Hued
            (Hue.Sublabel, "---------------------+----------+----------" |> Line) |> Hued
        ]
        |> List.iter messageSink
        let items = World.GetItemList context
        location
        |> World.GetIslandItems context
        |> Set.iter (fun item -> 
            let descriptor = items.[item]
            let sellPrice: float = (item, location) ||> World.DetermineIslandMarketSalePrice context
            let buyPrice: float = (item, location) ||> World.DetermineIslandMarketPurchasePrice context
            [
                (Hue.Value, descriptor.ItemName |> sprintf "%-20s" |> Text) |> Hued
                (Hue.Sublabel, " | " |> Text) |> Hued
                (Hue.Value, sellPrice |> sprintf "%8.3f" |> Text) |> Hued
                (Hue.Sublabel, " | " |> Text) |> Hued
                (Hue.Value, buyPrice |> sprintf "%8.3f" |> Line) |> Hued
            ]
            |> List.iter messageSink
            ())
        [
            (Hue.Label, "Money: " |> Text) |> Hued
            (Hue.Value, avatarId |> World.GetAvatarMoney context |> sprintf "%f" |> Line) |> Hued
        ]
        |> List.iter messageSink
        avatarId
        |> Gamestate.InPlay
        |> Some

    let Run 
            (context : CommonContext)
            (messageSink                   : MessageSink) =
        RunWithIsland 
            context
            messageSink
        |> Docked.RunBoilerplate 
            context
    

