namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

type CommoditySource = unit -> Map<uint64, CommodityDescriptor>
type IslandMarketSource = Location -> Map<uint64, Market>

module Item =
    let DetermineSalePrice 
            (commoditySource    : CommoditySource)
            (markets            : Map<uint64, Market>)  //TODO: refactor me to use source?
            (itemDescriptor     : ItemDescriptor) 
            : float =
        let commodities = commoditySource()
        itemDescriptor.Commodities
        |> Map.map
            (fun commodity amount -> 
                amount * (Market.DetermineSalePrice (commodities.[commodity], markets.[commodity])))
        |> Map.toList
        |> List.map snd
        |> List.reduce (+)

    let DeterminePurchasePrice 
            (commoditySource    : CommoditySource)
            (markets        : Map<uint64, Market>)  //TODO: refactor me to use source?
            (itemDescriptor : ItemDescriptor) 
            : float =
        let commodities = commoditySource()
        itemDescriptor.Commodities
        |> Map.map
            (fun commodity amount -> 
                amount * (Market.DeterminePurchasePrice (commodities.[commodity], markets.[commodity])))
        |> Map.toList
        |> List.map snd
        |> List.reduce (+)

