namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

type RationItemSource = unit -> uint64 list

type IslandMarketSource  = Location -> Map<uint64, Market>

type private UnitPriceDeterminer = CommodityDescriptor * Market -> float

module IslandMarket =
    type DeterminePriceContext =
        inherit ServiceContext
        abstract member islandMarketSource : IslandMarketSource
    let private DeterminePrice 
            (context             : ServiceContext)
            (unitPriceDeterminer : UnitPriceDeterminer)
            (itemIndex           : uint64) 
            (location            : Location)
            : float =
        Item.Get context itemIndex
        |> Option.fold
            (fun _ itemDescriptor->
                let commodities = Commodity.GetCommodities context
                let markets = (context :?> DeterminePriceContext).islandMarketSource location
                itemDescriptor.Commodities
                |> Map.map
                    (fun commodity amount -> 
                        amount * (unitPriceDeterminer (commodities.[commodity], markets.[commodity])))
                |> Map.toList
                |> List.map snd
                |> List.reduce (+)) System.Double.NaN

    let DetermineSalePrice 
            (context : ServiceContext) =
        DeterminePrice context Market.DetermineSalePrice

    let DeterminePurchasePrice 
            (context : ServiceContext) =
        DeterminePrice context Market.DeterminePurchasePrice
