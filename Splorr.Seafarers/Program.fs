open Splorr.Seafarers
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System.Data.SQLite

let commodities :Map<Commodity, CommodityDescriptor> =
    [(Commodity.Grain,{Name="grain";BasePrice=0.1;SaleFactor=0.01;PurchaseFactor=0.01;Discount=0.1;Occurrence=1.0})]
    |> Map.ofList

let items: Map<Item, ItemDescriptor> =
    [(Item.Ration, 
        {
            DisplayName = "waffles"
            Commodities = 
                [(Commodity.Grain, 0.1)]
                |> Map.ofList
            Occurrence = 1.0
        })]
    |> Map.ofList
let connectionString = "Data Source=seafarers.db;Version=3;"
let avatarId = ""
[<EntryPoint>]
let main argv =
    use connection = new SQLiteConnection(connectionString)
    ({
        MinimumIslandDistance=10.0
        WorldSize=(100.0,100.0)
        MaximumGenerationTries=500u
        RewardRange=(1.0,10.0)
        Commodities = commodities
        Items = items
    }, avatarId)
    ||> Runner.Run connection
    0
