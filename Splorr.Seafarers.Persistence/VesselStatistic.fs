namespace Splorr.Seafarers.Persistence

open System.Data.SQLite
open Splorr.Seafarers.Models

module VesselStatistic =
    let private convertor 
            (reader : SQLiteDataReader) 
            : VesselStatisticIdentifier * Statistic =
        (reader.GetInt32(0) |> enum<VesselStatisticIdentifier>, 
            {
                MinimumValue = reader.GetDouble(1)
                MaximumValue = reader.GetDouble(2)
                CurrentValue = reader.GetDouble(3)
            })

    
    let internal SetStatisticForAvatar 
            (connection          : SQLiteConnection) 
            (avatarId            : string) 
            (identifiedStatistic : VesselStatisticIdentifier * Statistic) 
            : Result<unit, string> =
        try
            let identifier, statistic = identifiedStatistic
            use command = new SQLiteCommand("REPLACE INTO [VesselStatistics] ([AvatarId],[StatisticId],[MinimumValue],[CurrentValue],[MaximumValue]) VALUES ($avatarId, $statisticId, $minimumValue, $currentValue, $maximumValue);", connection)
            command.Parameters.AddWithValue("$avatarId", avatarId) |> ignore
            command.Parameters.AddWithValue("$statisticId", identifier) |> ignore
            command.Parameters.AddWithValue("$minimumValue", statistic.MinimumValue) |> ignore
            command.Parameters.AddWithValue("$currentValue", statistic.CurrentValue) |> ignore
            command.Parameters.AddWithValue("$maximumValue", statistic.MaximumValue) |> ignore
            command.ExecuteNonQuery() |> ignore
            () |> Ok
        with
        | ex -> ex.ToString() |> Error

    let internal GetStatisticForAvatar 
            (connection : SQLiteConnection) 
            (avatarId   : string) 
            (identifier : VesselStatisticIdentifier) 
            : Result<Statistic option,string> =
        let commandFilter (command: SQLiteCommand) =
            command.Parameters.AddWithValue("$avatarId", avatarId) |> ignore
            command.Parameters.AddWithValue("$statisticId", identifier) |> ignore
        connection
        |> Utility.GetList 
            "SELECT [StatisticId], [MinimumValue], [MaximumValue], [CurrentValue] FROM [VesselStatistics] WHERE [AvatarId]=$avatarId AND [StatisticId]=$statisticId;" commandFilter convertor
        |> Result.map
            (fun items ->
                items
                |> List.map snd
                |> List.tryHead)

    let internal GetForAvatar 
            (connection : SQLiteConnection) 
            (avatarId   : string) 
            : Result<Map<VesselStatisticIdentifier, Statistic>, string> =
        let commandFilter (command: SQLiteCommand) =
            command.Parameters.AddWithValue("$avatarId", avatarId) |> ignore
        connection
        |> Utility.GetList 
            "SELECT [StatisticId], [MinimumValue], [MaximumValue], [CurrentValue] FROM [VesselStatistics] WHERE [AvatarId]=$avatarId;" commandFilter convertor
        |> Result.map
            (fun items ->
                items
                |> Map.ofList)

    let internal SetForAvatar 
            (connection : SQLiteConnection) 
            (avatarId   : string) 
            (statistics : Map<VesselStatisticIdentifier, Statistic>) 
            : Result<unit, string> =
        try
            statistics
            |> Map.iter 
                (fun statisticId statistic ->
                    use command = new SQLiteCommand("REPLACE INTO [VesselStatistics] ([AvatarId], [StatisticId], [MinimumValue], [MaximumValue], [CurrentValue]) VALUES ($avatarId, $statisticId, $minimumValue, $maximumValue, $currentValue);", connection)
                    command.Parameters.AddWithValue("$avatarId", avatarId) |> ignore
                    command.Parameters.AddWithValue("$statisticId", statisticId |> int64) |> ignore
                    command.Parameters.AddWithValue("$minimumValue", statistic.MinimumValue) |> ignore
                    command.Parameters.AddWithValue("$maximumValue", statistic.MaximumValue) |> ignore
                    command.Parameters.AddWithValue("$currentValue", statistic.CurrentValue) |> ignore
                    command.ExecuteNonQuery() |> ignore)
            |> Ok
        with
        | ex -> ex.ToString() |> Error
