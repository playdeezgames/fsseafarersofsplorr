open Splorr.Seafarers
open Splorr.Seafarers.Services
open Splorr.Seafarers.Models
open System.Data.SQLite

let items: Map<uint64, ItemDescriptor> =
    Map.empty
    |> Map.add 1UL 
        {
            DisplayName = "waffles"
            Commodities = 
                [(1UL, 0.01)]
                |> Map.ofList
            Occurrence = 1.0
            Tonnage = 0.01
        }
    |> Map.add 2UL
        {
            DisplayName = "sacks of wheat"
            Commodities = 
                [(1UL, 1.0)]
                |> Map.ofList
            Occurrence = 1.0
            Tonnage = 1.0
        }
let connectionString = "Data Source=seafarers.db;Version=3;"
let avatarId = ""
[<EntryPoint>]
let main argv =
    use connection = new SQLiteConnection(connectionString)
    try
        connection.Open()
        ({
            MinimumIslandDistance=10.0
            WorldSize=(100.0,100.0)
            MaximumGenerationTries=500u
            RewardRange=(1.0,10.0)
            Items = items
        }, avatarId)
        ||> Runner.Run connection
    finally
        connection.Close()
    0
