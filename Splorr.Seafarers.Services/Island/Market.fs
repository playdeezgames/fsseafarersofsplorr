namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open Splorr.Common


module IslandMarket =
    type RationItemSource = unit -> uint64 list
    type IslandMarketSource  = Location -> Map<uint64, Market>
    type private UnitPriceDeterminer = CommodityDescriptor * Market -> float
    
    type DeterminePriceContext =
        abstract member islandMarketSource : IslandMarketSource ref
    let private GetMarket
            (context : CommonContext)
            (location : Location)
            : Map<uint64, Market> =
        (context :?> DeterminePriceContext).islandMarketSource.Value location

    let private DeterminePrice 
            (context             : CommonContext)
            (unitPriceDeterminer : UnitPriceDeterminer)
            (itemIndex           : uint64) 
            (location            : Location)
            : float =
        Item.Get context itemIndex
        |> Option.fold
            (fun _ itemDescriptor->
                let commodities = Commodity.GetCommodities context
                let markets = GetMarket context location
                itemDescriptor.Commodities
                |> Map.map
                    (fun commodity amount -> 
                        amount * (unitPriceDeterminer (commodities.[commodity], markets.[commodity])))
                |> Map.toList
                |> List.map snd
                |> List.reduce (+)) System.Double.NaN

    let internal DetermineSalePrice 
            (context : CommonContext) =
        DeterminePrice context Market.DetermineSalePrice

    let internal DeterminePurchasePrice 
            (context : CommonContext) =
        DeterminePrice context Market.DeterminePurchasePrice
