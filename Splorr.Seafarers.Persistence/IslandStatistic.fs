namespace Splorr.Seafarers.Persistence

open System.Data.SQLite
open Splorr.Seafarers.Models

module IslandStatistic = 
    let private statisticConvertor 
            (reader : SQLiteDataReader) 
            : ShipmateStatisticIdentifier * Statistic =
        (reader.GetInt32(0) |> enum<ShipmateStatisticIdentifier>, 
            {
                MinimumValue = reader.GetDouble(1)
                MaximumValue = reader.GetDouble(2)
                CurrentValue = reader.GetDouble(3)
            })

    let GetStatisticForIsland
            (connection : SQLiteConnection) 
            (location   : Location) 
            (identifier : IslandStatisticIdentifier) 
            : Result<Statistic option,string> =
        let commandFilter (command: SQLiteCommand) =
            command.Parameters.AddWithValue("$islandX", location |> fst) |> ignore
            command.Parameters.AddWithValue("$islandY", location |> snd) |> ignore
            command.Parameters.AddWithValue("$statisticId", identifier) |> ignore
        connection
        |> Utility.GetList 
            "SELECT [StatisticId], [MinimumValue], [MaximumValue], [CurrentValue] FROM [IslandStatistics] WHERE [IslandX]=$islandX AND [IslandY]=$IslandY;" commandFilter statisticConvertor
        |> Result.map
            (fun items ->
                items
                |> List.map snd
                |> List.tryHead)

    let SetStatisticForIsland
            (connection : SQLiteConnection) 
            (location   : Location)
            (identifier : IslandStatisticIdentifier, statistic:Statistic option)
            : Result<unit, string> =
        try
            use command = new SQLiteCommand ("DELETE FROM [IslandStatistics] WHERE [IslandX] = $islandX AND [IslandY] = $islandY AND [StatisticId] = $statisticId;", connection)
            command.Parameters.AddWithValue("$islandX", location |> fst) |> ignore
            command.Parameters.AddWithValue("$islandY", location |> snd) |> ignore
            command.Parameters.AddWithValue("$statisticId", identifier) |> ignore
            command.ExecuteNonQuery() |> ignore
            statistic
            |> Option.iter
                (fun stat ->
                    use command = new SQLiteCommand("
                        REPLACE INTO [IslandStatistics] 
                            ([IslandX],[islandY],[StatisticId],[MinimumValue],[CurrentValue],[MaximumValue]) 
                            VALUES 
                            ($islandX, $islandY, $statisticId, $minimumValue, $currentValue, $maximumValue);", connection)
                    command.Parameters.AddWithValue("$islandX", location |> fst) |> ignore
                    command.Parameters.AddWithValue("$islandY", location |> snd) |> ignore
                    command.Parameters.AddWithValue("$statisticId", identifier) |> ignore
                    command.Parameters.AddWithValue("$minimumValue", stat.MinimumValue) |> ignore
                    command.Parameters.AddWithValue("$currentValue", stat.CurrentValue) |> ignore
                    command.Parameters.AddWithValue("$maximumValue", stat.MaximumValue) |> ignore
                    command.ExecuteNonQuery() |> ignore)
            () |> Ok
        with
        | ex -> ex.ToString() |> Error

