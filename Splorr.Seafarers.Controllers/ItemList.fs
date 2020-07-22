namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module ItemList = 
    let private RunWithIsland (islandMarketSource:Location->Map<uint64,Market>) (islandItemSource:Location->Set<uint64>) (commodities:Map<uint64, CommodityDescriptor>) (items:Map<uint64, ItemDescriptor>) (sink:MessageSink) (location:Location) (island:Island) (world: World) : Gamestate option =
        let avatar = world.Avatars.[world.AvatarId]
        [
            "" |> Line
            (Heading, "Item" |> sprintf "%-20s" |> Text) |> Hued
            (Sublabel, " | " |> Text) |> Hued
            (Heading, "Selling" |> sprintf "%8s" |> Text) |> Hued
            (Sublabel, " | " |> Text) |> Hued
            (Heading, "Offering" |> sprintf "%8s" |> Line) |> Hued
            (Sublabel, "---------------------+----------+----------" |> Line) |> Hued
        ]
        |> List.iter sink
        location
        |> islandItemSource
        |> Set.iter (fun item -> 
            let descriptor = items.[item]
            let markets = islandMarketSource location
            let sellPrice: float = descriptor |> Item.DetermineSalePrice commodities markets
            let buyPrice: float = descriptor |> Item.DeterminePurchasePrice commodities markets
            [
                (Value, descriptor.ItemName |> sprintf "%-20s" |> Text) |> Hued
                (Sublabel, " | " |> Text) |> Hued
                (Value, sellPrice |> sprintf "%8.3f" |> Text) |> Hued
                (Sublabel, " | " |> Text) |> Hued
                (Value, buyPrice |> sprintf "%8.3f" |> Line) |> Hued
            ]
            |> List.iter sink
            ())
        [
            (Label, "Money: " |> Text) |> Hued
            (Value, avatar.Money |> sprintf "%f" |> Line) |> Hued
        ]
        |> List.iter sink
        (Dock, location, world)
        |> Gamestate.Docked
        |> Some

    let Run (islandMarketSource:Location->Map<uint64,Market>) (islandItemSource:Location->Set<uint64>) (commodities:Map<uint64, CommodityDescriptor>) (items:Map<uint64, ItemDescriptor>) (sink:MessageSink) =
        Docked.RunBoilerplate (RunWithIsland islandMarketSource islandItemSource commodities items sink)
    

