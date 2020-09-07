namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

type CommoditySource = unit -> Map<uint64, CommodityDescriptor>
type IslandMarketSource = Location -> Map<uint64, Market>
type ItemSingleSource = uint64 -> ItemDescriptor option

type ItemDetermineSalePriceContext =
    abstract member commoditySource    : CommoditySource
    abstract member islandMarketSource : IslandMarketSource
    abstract member itemSingleSource   : ItemSingleSource

type ItemDeterminePurchasePriceContext =
    abstract member commoditySource    : CommoditySource
    abstract member islandMarketSource : IslandMarketSource
    abstract member itemSingleSource   : ItemSingleSource

module Item =
    let DetermineSalePrice 
            (context   : ItemDetermineSalePriceContext)
            (itemIndex : uint64) 
            (location  : Location)
            : float =
        let commodities = context.commoditySource()
        let markets = context.islandMarketSource location
        context.itemSingleSource itemIndex
        |> Option.fold
            (fun _ itemDescriptor->
                itemDescriptor.Commodities
                |> Map.map
                    (fun commodity amount -> 
                        amount * (Market.DetermineSalePrice (commodities.[commodity], markets.[commodity])))
                |> Map.toList
                |> List.map snd
                |> List.reduce (+)) System.Double.NaN

    let DeterminePurchasePrice 
            (context : ItemDeterminePurchasePriceContext)
            (markets        : Map<uint64, Market>)  //TODO: refactor me to use source?
            (itemDescriptor : ItemDescriptor) 
            (location           : Location)
            : float =
        let commodities = context.commoditySource()
        itemDescriptor.Commodities
        |> Map.map
            (fun commodity amount -> 
                amount * (Market.DeterminePurchasePrice (commodities.[commodity], markets.[commodity])))
        |> Map.toList
        |> List.map snd
        |> List.reduce (+)

