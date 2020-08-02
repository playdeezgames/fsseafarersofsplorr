namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module ItemList = 
    let private RunWithIsland 
            (commoditySource    : unit -> Map<uint64, CommodityDescriptor>) 
            (itemSource         : unit -> Map<uint64, ItemDescriptor>) 
            (islandMarketSource : Location -> Map<uint64,Market>) 
            (islandItemSource   : Location -> Set<uint64>) 
            (messageSink        : MessageSink) 
            (location           : Location) 
            (world              : World) 
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
        let commodities = commoditySource()
        let items = itemSource()
        location
        |> islandItemSource
        |> Set.iter (fun item -> 
            let descriptor = items.[item]
            let markets = islandMarketSource location
            let sellPrice: float = descriptor |> Item.DetermineSalePrice commodities markets
            let buyPrice: float = descriptor |> Item.DeterminePurchasePrice commodities markets
            [
                (Hue.Value, descriptor.ItemName |> sprintf "%-20s" |> Text) |> Hued
                (Hue.Sublabel, " | " |> Text) |> Hued
                (Hue.Value, sellPrice |> sprintf "%8.3f" |> Text) |> Hued
                (Hue.Sublabel, " | " |> Text) |> Hued
                (Hue.Value, buyPrice |> sprintf "%8.3f" |> Line) |> Hued
            ]
            |> List.iter messageSink
            ())
        let avatar = world.Avatars.[world.AvatarId]
        [
            (Hue.Label, "Money: " |> Text) |> Hued
            (Hue.Value, avatar.Money |> sprintf "%f" |> Line) |> Hued
        ]
        |> List.iter messageSink
        (Dock, location, world)
        |> Gamestate.Docked
        |> Some

    let Run 
            (commoditySource    : unit -> Map<uint64, CommodityDescriptor>) 
            (itemSource         : unit -> Map<uint64, ItemDescriptor>) 
            (islandMarketSource : Location -> Map<uint64,Market>) 
            (islandItemSource   : Location -> Set<uint64>) 
            (messageSink        : MessageSink) =
        RunWithIsland commoditySource itemSource islandMarketSource islandItemSource messageSink
        |> Docked.RunBoilerplate 
    

