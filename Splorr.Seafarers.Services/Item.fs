namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

type CommoditySource     = unit -> Map<uint64, CommodityDescriptor>
type IslandMarketSource  = Location -> Map<uint64, Market>
type ItemSingleSource    = uint64 -> ItemDescriptor option
type UnitPriceDeterminer = CommodityDescriptor * Market -> float

type ItemDeterminePriceContext =
    abstract member commoditySource    : CommoditySource
    abstract member islandMarketSource : IslandMarketSource
    abstract member itemSingleSource   : ItemSingleSource

type ItemDetermineSalePriceContext =
    inherit ItemDeterminePriceContext

type ItemDeterminePurchasePriceContext =
    inherit ItemDeterminePriceContext

module Item =
    let private DeterminePrice 
            (context             : ItemDeterminePriceContext)
            (unitPriceDeterminer : UnitPriceDeterminer)
            (itemIndex           : uint64) 
            (location            : Location)
            : float =
        context.itemSingleSource itemIndex
        |> Option.fold
            (fun _ itemDescriptor->
                let commodities = context.commoditySource()
                let markets = context.islandMarketSource location
                itemDescriptor.Commodities
                |> Map.map
                    (fun commodity amount -> 
                        amount * (unitPriceDeterminer (commodities.[commodity], markets.[commodity])))
                |> Map.toList
                |> List.map snd
                |> List.reduce (+)) System.Double.NaN

    let DetermineSalePrice 
            (context : ItemDetermineSalePriceContext) =
        DeterminePrice context Market.DetermineSalePrice

    let DeterminePurchasePrice 
            (context : ItemDeterminePurchasePriceContext) =
        DeterminePrice context Market.DeterminePurchasePrice
