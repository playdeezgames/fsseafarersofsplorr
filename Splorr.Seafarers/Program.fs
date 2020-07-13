open Splorr.Seafarers
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System.Data.SQLite

let commodities :Map<uint, CommodityDescriptor> =
    [(1u,{Name="grain";BasePrice=1.0;SaleFactor=0.01;PurchaseFactor=0.01;Discount=0.5})]
    |> Map.ofList

let items: Map<uint, ItemDescriptor> =
    Map.empty
    |> Map.add 1u 
        {
            DisplayName = "waffles"
            Commodities = 
                [(1u, 0.01)]
                |> Map.ofList
            Occurrence = 1.0
        }
    |> Map.add 2u
        {
            DisplayName = "sacks of wheat"
            Commodities = 
                [(1u, 1.0)]
                |> Map.ofList
            Occurrence = 1.0
        }
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
