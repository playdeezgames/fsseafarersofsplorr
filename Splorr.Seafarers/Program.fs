open Splorr.Seafarers
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models

let commodities :Map<Commodity, CommodityDescriptor> =
    [(Grain,{Name="grain";BasePrice=1.0;SaleFactor=0.01;PurchaseFactor=0.01;Discount=0.1;Occurrence=1.0})]
    |> Map.ofList

[<EntryPoint>]
let main argv =
    {
        MinimumIslandDistance=10.0
        WorldSize=(100.0,100.0)
        MaximumGenerationTries=500u
        RewardRange=(1.0,10.0)
        Commodities = commodities
    }
    |> Runner.Run
    0
