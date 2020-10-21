namespace Splorr.Seafarers.Persistence

open System.Data.SQLite
open Splorr.Seafarers.Models

module IslandMarket = 
    let internal GetForIsland 
            (connection : SQLiteConnection) 
            (location   : Location) 
            : Result<Map<uint64, Market>, string> =
        let commandSideEffect (command: SQLiteCommand) =
            command.Parameters.AddWithValue("$islandX", location |> fst) |> ignore
            command.Parameters.AddWithValue("$islandY", location |> snd) |> ignore
        let convertor (reader:SQLiteDataReader) =
            (reader.GetInt64(0) |> uint64, {Supply=reader.GetDouble(1); Demand=reader.GetDouble(2)})
        connection
        |> Utility.GetList "SELECT [CommodityId], [Supply], [Demand] FROM [IslandMarkets] WHERE [IslandX]= $islandX AND [IslandY]= $islandY;" commandSideEffect convertor
        |> Result.bind
            (Map.ofList >> Ok)

    let internal GetMarketForIsland 
            (connection  : SQLiteConnection) 
            (location    : Location) 
            (commodityId : uint64) 
            : Result<Market option,string> =
        let commandSideEffect (command: SQLiteCommand) =
            command.Parameters.AddWithValue("$islandX", location |> fst) |> ignore
            command.Parameters.AddWithValue("$islandY", location |> snd) |> ignore
            command.Parameters.AddWithValue("$commodityId", commodityId) |> ignore
        let convertor (reader:SQLiteDataReader) =
            {Supply=reader.GetDouble(1); Demand=reader.GetDouble(2)}
        connection
        |> Utility.GetList "SELECT [CommodityId], [Supply], [Demand] FROM [IslandMarkets] WHERE [IslandX]= $islandX AND [IslandY] = $islandY AND [CommodityId] = $commodityId;" commandSideEffect convertor
        |> Result.bind
            (List.tryHead >> Ok)
   
    let internal SetForIsland 
            (connection      : SQLiteConnection) 
            (location        : Location) 
            (commodityMarket : uint64 * Market) 
            : Result<unit, string> =
        try
            let commodityId, market = commodityMarket
            use command = new SQLiteCommand("REPLACE INTO [IslandMarkets] ([IslandX], [IslandY], [CommodityId], [Supply], [Demand]) VALUES ($islandX, $islandY, $commodityId, $supply, $demand);", connection)
            command.Parameters.AddWithValue("$islandX", location |> fst) |> ignore
            command.Parameters.AddWithValue("$islandY", location |> snd) |> ignore
            command.Parameters.AddWithValue("$commodityId", commodityId |> int64) |> ignore
            command.Parameters.AddWithValue("$supply", market.Supply) |> ignore
            command.Parameters.AddWithValue("$demand", market.Demand) |> ignore
            command.ExecuteNonQuery() |> ignore
            () |> Ok
        with
        | ex -> ex.ToString() |> Error

    let internal CreateForIsland 
            (connection : SQLiteConnection) 
            (location   : Location) 
            (markets    : Map<uint64, Market>) 
            : Result<unit, string> =
        try
            use command : SQLiteCommand = new SQLiteCommand("DELETE FROM [IslandMarkets] WHERE [IslandX]= $islandX AND [IslandY]= $islandY;", connection)
            command.Parameters.AddWithValue("$islandX", location |> fst) |> ignore
            command.Parameters.AddWithValue("$islandY", location |> snd) |> ignore
            command.ExecuteNonQuery() |> ignore
            markets
            |> Map.iter
                (fun commodityId market ->
                    (commodityId, market)
                    |> SetForIsland connection location
                    |> ignore)
            () |> Ok
        with
        | ex ->
            ex.ToString()
            |> Error
