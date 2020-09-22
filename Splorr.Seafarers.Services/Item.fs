namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

type CommoditySource     = unit -> Map<uint64, CommodityDescriptor>
type IslandMarketSource  = Location -> Map<uint64, Market>
type ItemSingleSource    = uint64 -> ItemDescriptor option
type private UnitPriceDeterminer = CommodityDescriptor * Market -> float

module Item =
    type DeterminePriceContext =
        inherit OperatingContext
        abstract member commoditySource    : CommoditySource
        abstract member islandMarketSource : IslandMarketSource
        abstract member itemSingleSource   : ItemSingleSource
    
    let private DeterminePrice 
            (context             : OperatingContext)
            (unitPriceDeterminer : UnitPriceDeterminer)
            (itemIndex           : uint64) 
            (location            : Location)
            : float =
        let context = context :?> DeterminePriceContext
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
            (context : OperatingContext) =
        DeterminePrice context Market.DetermineSalePrice

    let DeterminePurchasePrice 
            (context : OperatingContext) =
        DeterminePrice context Market.DeterminePurchasePrice
