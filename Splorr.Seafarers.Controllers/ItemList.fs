﻿namespace Splorr.Seafarers.Controllers

open Splorr.Seafarers.Models
open Splorr.Seafarers.Services

type ItemListRunWithIslandContext =
    inherit ItemDetermineSalePriceContext
    inherit ItemDeterminePurchasePriceContext

type ItemListRunContext =
    inherit ItemListRunWithIslandContext

module ItemList = 
    let private RunWithIsland 
            (context            : ItemListRunWithIslandContext)
            (commoditySource    : unit -> Map<uint64, CommodityDescriptor>) 
            (itemSource         : unit -> Map<uint64, ItemDescriptor>) 
            (islandMarketSource : Location -> Map<uint64,Market>) 
            (islandItemSource   : Location -> Set<uint64>) 
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
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
        let items = itemSource()
        location
        |> islandItemSource
        |> Set.iter (fun item -> 
            let descriptor = items.[item]
            let markets = islandMarketSource location
            let sellPrice: float = (item, location) ||> Item.DetermineSalePrice context
            let buyPrice: float = (item, location) ||> Item.DeterminePurchasePrice context
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
            (Hue.Value, avatarId |> Avatar.GetMoney shipmateSingleStatisticSource |> sprintf "%f" |> Line) |> Hued
        ]
        |> List.iter messageSink
        avatarId
        |> Gamestate.InPlay
        |> Some

    let Run 
            (context : ItemListRunContext)
            (avatarMessageSource           : AvatarMessageSource)
            (commoditySource               : CommoditySource) 
            (islandItemSource              : IslandItemSource)
            (islandMarketSource            : IslandMarketSource) 
            (islandSource                  : IslandSource)
            (itemSource                    : ItemSource) 
            (shipmateSingleStatisticSource : ShipmateSingleStatisticSource)
            (messageSink                   : MessageSink) =
        RunWithIsland 
            context
            commoditySource 
            itemSource 
            islandMarketSource 
            islandItemSource 
            shipmateSingleStatisticSource 
            messageSink
        |> Docked.RunBoilerplate 
            avatarMessageSource
            islandSource
            shipmateSingleStatisticSource 
    

