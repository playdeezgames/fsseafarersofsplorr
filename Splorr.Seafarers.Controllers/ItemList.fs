namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

type ItemListRunWithIslandContext =
    inherit ServiceContext
    abstract member islandItemSource   : IslandItemSource

module ItemList = 
    let private RunWithIsland 
            (context            : ServiceContext)
            (messageSink        : MessageSink) 
            (location           : Location) 
            (avatarId           : string) 
            : Gamestate option =
        let context = context :?> ItemListRunWithIslandContext
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
        let items = Item.GetList context
        location
        |> context.islandItemSource
        |> Set.iter (fun item -> 
            let descriptor = items.[item]
            let sellPrice: float = (item, location) ||> IslandMarket.DetermineSalePrice context
            let buyPrice: float = (item, location) ||> IslandMarket.DeterminePurchasePrice context
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
            (Hue.Value, avatarId |> Avatar.GetMoney context |> sprintf "%f" |> Line) |> Hued
        ]
        |> List.iter messageSink
        avatarId
        |> Gamestate.InPlay
        |> Some

    let Run 
            (context : ServiceContext)
            (messageSink                   : MessageSink) =
        RunWithIsland 
            context
            messageSink
        |> Docked.RunBoilerplate 
            context
    

