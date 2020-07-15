namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

module ItemList = 
    let private RunWithIsland (commodities:Map<uint64, CommodityDescriptor>) (sink:MessageSink) (location:Location) (island:Island) (avatarId:string) (world: World) : Gamestate option =
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
        island.Items
        |> Set.iter (fun item -> 
            let descriptor = world.Items.[item]
            let sellPrice: float = descriptor |> Item.DetermineSalePrice commodities island.Markets
            let buyPrice: float = descriptor |> Item.DeterminePurchasePrice commodities island.Markets
            [
                (Value, descriptor.DisplayName |> sprintf "%-20s" |> Text) |> Hued
                (Sublabel, " | " |> Text) |> Hued
                (Value, sellPrice |> sprintf "%8.3f" |> Text) |> Hued
                (Sublabel, " | " |> Text) |> Hued
                (Value, buyPrice |> sprintf "%8.3f" |> Line) |> Hued
            ]
            |> List.iter sink
            ())
        (Dock, location, world)
        |> Gamestate.Docked
        |> Some

    let Run (commodities:Map<uint64, CommodityDescriptor>) (sink:MessageSink) =
        Docked.RunBoilerplate (RunWithIsland commodities sink)
    

