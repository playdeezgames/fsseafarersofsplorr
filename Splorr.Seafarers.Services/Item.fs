namespace Splorr.Seafarers.Services
open Splorr.Seafarers.Models

module Item =
    let DetermineSalePrice 
            (commodities:Map<uint64, CommodityDescriptor>) 
            (markets:Map<uint64, Market>) 
            (itemDescriptor:ItemDescriptor) : float =
        itemDescriptor.Commodities
        |> Map.map
            (fun commodity amount -> 
                amount * (Market.DetermineSalePrice commodities.[commodity] markets.[commodity]))
        |> Map.toList
        |> List.map snd
        |> List.reduce (+)

    let DeterminePurchasePrice 
            (commodities:Map<uint64, CommodityDescriptor>) 
            (markets:Map<uint64, Market>) 
            (itemDescriptor:ItemDescriptor) : float =
        itemDescriptor.Commodities
        |> Map.map
            (fun commodity amount -> 
                amount * (Market.DeterminePurchasePrice commodities.[commodity] markets.[commodity]))
        |> Map.toList
        |> List.map snd
        |> List.reduce (+)

